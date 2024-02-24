using Eto.Forms;
using Grasshopper.GUI;
using Rhino.ApplicationSettings;
using Rhino.Geometry;
using Rhino.Render;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LCA_Toolbox
{

    public class LCA_Model
    {

        public DataTable elementsDataTable;
        private DataTable timeline;

        public bool AllowSequestration = true;
        int modelLifetime { get; set; }
        public List<double> model_B6_perYear { get; set; }

        List<string> EmbodiedCarbonStages = new List<string> { "Element_A1toA3", "Element_A4", "Element_B4_Sum", "Element_C1toC4" };

        protected LCA_Model(LCA_Model other)
        {
            // Cloning code goes here...
            this.elementsDataTable = other.elementsDataTable.Copy();
            this.timeline = other.timeline.Copy();

            this.AllowSequestration = other.AllowSequestration;
            this.modelLifetime = other.modelLifetime;
            this.model_B6_perYear = other.model_B6_perYear;

            UpdatetimelineSumAndCumulativeCarbon();

        }

        public List<double> GetDataColumnFromTimeline(string column)
        {
            return timeline.AsEnumerable().Select(x => (double)x[column]).ToList();
        }

        public LCA_Model Clone()
        {
            return new LCA_Model(this);
        }

        public LCA_Model(List<LCA_Element> elements, int _lifetime, List<double> B6_perYears)
        {
            modelLifetime = _lifetime;
            if (elements[0] != null)
                CalculateB4_allElements(ref elements);
            CreateDataTableFromListOfElements(elements);

            model_B6_perYear = B6_perYears;

            timeline = ConstructTimelineDataTable();
            UpdatetimelineSumAndCumulativeCarbon();
        }

        public DataTable ConstructTimelineDataTable()
        {
            DataTable rtnDT = new DataTable();
            rtnDT.Columns.Add(new DataColumn("Year", typeof(double))); //year

            rtnDT.Columns.Add(new DataColumn("A1_A3", typeof(double))); //Production phase
            rtnDT.Columns.Add(new DataColumn("A1_A3_noSeq", typeof(double))); //Production phase
            rtnDT.Columns.Add(new DataColumn("A4", typeof(double))); //Trasport to site
            rtnDT.Columns.Add(new DataColumn("A5", typeof(double))); //Assembly     // NOT IMPLEMENTED
            rtnDT.Columns.Add(new DataColumn("B1", typeof(double))); //Use          // NOT IMPLEMENTED
            rtnDT.Columns.Add(new DataColumn("B2", typeof(double))); //Maintenence  // NOT IMPLEMENTED
            rtnDT.Columns.Add(new DataColumn("B3", typeof(double))); //Repair       // NOT IMPLEMENTED
            rtnDT.Columns.Add(new DataColumn("B4", typeof(double))); //Replacement 
            rtnDT.Columns.Add(new DataColumn("B4_noSeq", typeof(double))); //Replacement 
            rtnDT.Columns.Add(new DataColumn("B5", typeof(double))); //Refurbishment  // NOT IMPLEMENTED
            rtnDT.Columns.Add(new DataColumn("B6", typeof(double))); //operational energy
            rtnDT.Columns.Add(new DataColumn("B7", typeof(double))); //operational water // NOT IMPLEMENTED
            rtnDT.Columns.Add(new DataColumn("C1_C4", typeof(double))); // End of life
            rtnDT.Columns.Add(new DataColumn("D", typeof(double))); //Beyond lifetime  // NOT IMPLEMENTED


            rtnDT.Columns.Add(new DataColumn("Sum_Embodied", typeof(double))); //Sum_Embodied carbon per year 
            rtnDT.Columns.Add(new DataColumn("Sum_Operational", typeof(double))); //Sum_Operational carbon per year
            rtnDT.Columns.Add(new DataColumn("Sum_Carbon", typeof(double))); //Sum_ carbon per year
            rtnDT.Columns.Add(new DataColumn("Cumulative_Carbon", typeof(double))); //Cumulative per year


            //init all values to 0
            foreach (DataColumn col in rtnDT.Columns)
            { col.DefaultValue = 0; }


            // and and add B6 every year. 
            for (int i = 0; i < modelLifetime; i++)
            {

                DataRow row = rtnDT.NewRow();

                int year_index = i < model_B6_perYear.Count() ? i : model_B6_perYear.Count() - 1; //repate last of not enoung b6 values.

                row.SetField("Year", DateTime.Now.Year + i);
                row.SetField("B6", model_B6_perYear[year_index]);

                rtnDT.Rows.Add(row);
            }

            // Set Year 0
            rtnDT.Rows[0].SetField("A1_A3", GetColumnSum("Element_A1toA3"));
            rtnDT.Rows[0].SetField("A1_A3_noSeq", GetColumnSum("Element_A1toA3_noSeq"));
            rtnDT.Rows[0].SetField("A4", GetColumnSum("Element_A4"));

            //Set B4 replacements

            foreach (DataRow element in elementsDataTable.Rows)
            {
                foreach (int year in (List<int>)element["Element_B4years"])
                {
                    if (year >= rtnDT.Rows.Count) { continue; }
                    rtnDT.Rows[year].SetField("B4", (double)rtnDT.Rows[year]["B4"] + (double)element["Element_B4_perTime"]);
                    rtnDT.Rows[year].SetField("B4_noSeq", (double)rtnDT.Rows[year]["B4_noSeq"] + (double)element["Element_B4_perTime_noSeq"]);
                    rtnDT.Rows[year].SetField("C1_C4", (double)rtnDT.Rows[year]["C1_C4"] + (double)element["Element_C1toC4"]);
                }
            }


            // Set Year n

            rtnDT.Rows[modelLifetime - 1].SetField("C1_C4", GetColumnSum("Element_C1toC4"));





            return rtnDT;


        }


        private void CreateDataTableFromListOfElements(List<LCA_Element> elements)
        {
            elementsDataTable = new DataTable();
            foreach (PropertyInfo info in typeof(LCA_Element).GetProperties())
            {
                elementsDataTable.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }


            foreach (LCA_Element element in elements)
            {
                if (element == null) continue;
                DataRow dr = elementsDataTable.NewRow();
                foreach (PropertyInfo info in typeof(LCA_Element).GetProperties())
                {
                    dr[info.Name] = info.GetValue(element, null);
                }
                elementsDataTable.Rows.Add(dr);
            }

        }

        private void UpdatetimelineSumAndCumulativeCarbon()
        {
            double runningSumTotal = 0;

            List<string> embodiedStages = new List<string>();
            embodiedStages.Add(AllowSequestration ? "A1_A3" : "A1-A3_noSeq");
            embodiedStages.Add("A4");
            embodiedStages.Add("A5");
            embodiedStages.Add("B1");
            embodiedStages.Add("B2");
            embodiedStages.Add("B3");
            embodiedStages.Add(AllowSequestration ? "B4" : "B4_noSeq");
            embodiedStages.Add("B5");
            embodiedStages.Add("C1_C4");
            embodiedStages.Add("D");


            List<string> operationalStages = new List<string>();
            operationalStages.Add("B6");
            operationalStages.Add("B7");


            foreach (DataRow row in timeline.Rows)
            {
                double runningSumEmbodied = 0;
                foreach (string s in embodiedStages)
                {
                    runningSumEmbodied += (double)row[s];
                }
                row.SetField("Sum_Embodied", runningSumEmbodied);


                double runningSumOperational = 0;
                foreach (string s in operationalStages)
                {
                    runningSumOperational += (double)row[s];
                }
                row.SetField("Sum_Operational", runningSumOperational);

                double sumCarbon = runningSumOperational + runningSumEmbodied;
                row.SetField("Sum_Carbon", sumCarbon);

                runningSumTotal += sumCarbon;
                row.SetField("Cumulative_Carbon", runningSumTotal);
            }

        }

        private void CalculateB4_allElements(ref List<LCA_Element> elements)
        {
            if (modelLifetime <= 0)
                return;

            foreach (LCA_Element element in elements)
            {
                if (element.Element_Lifetime > 0 && element.Element_Lifetime < modelLifetime)
                {

                    // 40 / 40 = 1 (-1 = 0replacements)
                    // 50 / 25 = 2 (-1 = 1replacements)
                    // 50 / 30 = 2 (-1 = 1replacements)
                    //find years to replace element
                    element.Element_B4years.Clear();
                    for (int i = element.Element_Lifetime; i < modelLifetime; i += element.Element_Lifetime)
                    {
                        element.Element_B4years.Add(i);
                    }

                    element.Element_B4_Nreplacements = element.Element_B4years.Count();

                    //WITH SEQ
                    element.Element_B4_perTime = element.Element_A1toA3 + element.Element_A4; //+ element.Element_D;
                    element.Element_B4_Sum = element.Element_B4_perTime * element.Element_B4_Nreplacements;
                    //Without SEQ
                    element.Element_B4_perTime_noSeq = element.Element_A1toA3_noSeq + element.Element_A4; //+ element.Element_D;
                    element.Element_B4_Sum_noSeq = element.Element_B4_perTime_noSeq * element.Element_B4_Nreplacements;



                }
            }
        }

        private void AdjustColumnForAllowSequestration(ref string column)
        {
            if (!AllowSequestration && column == "Element_A1toA3")
                column = "Element_A1toA3_noSeq";

            if (!AllowSequestration && column == "Element_B4_Sum")
                column = "Element_B4_Sum_noSeq";

            if (!AllowSequestration && column == "A1_A3")
                column = "A1_A3_noSeq";
            if (!AllowSequestration && column == "B4")
                column = "B4_noSeq";

        }

        public void FilterDataTable(string filter, string filterColumn)
        {
            elementsDataTable = elementsDataTable.Select($"{filterColumn} = '{filter}'").CopyToDataTable();
        }

        //UNTESTED !!
        public void FiterDataTableByMaterialName(string filter)
        {
            FilterDataTable(filter, "MaterialName");
        }
        //UNTESTED !!
        public void FiterDataTableByElementName(string filter)
        {
            elementsDataTable = elementsDataTable.Select($"Element_Name = '{filter}'").CopyToDataTable();
        }

        //UNTESTED !!
        public void FiterDataTableByElementGroup(string filter)
        {
            elementsDataTable = elementsDataTable.Select($"Element_Group = '{filter}'").CopyToDataTable();
        }



        public List<string> ListUniqueMaterialNames()
        {
            var uniqueItems = elementsDataTable.AsEnumerable().Select(row => row["MaterialName"]).Distinct().ToList();

            return uniqueItems.Cast<string>().ToList();
        }


        //UNTESTED !!
        public List<string> ListUniqueElement_Groups()
        {
            var uniqueItems = elementsDataTable.AsEnumerable().Select(row => row["Element_Group"]).Distinct().ToList();

            return uniqueItems.Cast<string>().ToList();
        }
        //UNTESTED !!
        public List<string> ListUniqueElement_Names()
        {
            var uniqueItems = elementsDataTable.AsEnumerable().Select(row => row["Element_Name"]).Distinct().ToList();

            return uniqueItems.Cast<string>().ToList();
        }


        public double GetColumnSum(string column)
        {
            AdjustColumnForAllowSequestration(ref column);

            return Convert.ToDouble(elementsDataTable.Compute($"SUM({column})", string.Empty));

        }

        public double GetColumnSum(List<string> columnNames)
        {
            double sum = 0;
            foreach (var column in columnNames)
            {
                sum += GetColumnSum(column);
            }
            return sum;
        }




        public double GetEmbodied_carbon()
        {
            return Convert.ToDouble(timeline.Compute("SUM(Sum_Embodied)",string.Empty));
        }
        public double GetOperational_carbon()
        {
            return Convert.ToDouble(timeline.Compute("SUM(Sum_Operational)", string.Empty));
        }
        public double GetGWP_total()
        {
            return Convert.ToDouble(timeline.Compute("SUM(Sum_Carbon)", string.Empty));
        }




        public List<double> GetColumnSum_ListByMaterial(string column)
        {

            AdjustColumnForAllowSequestration(ref column);

            List<double> rtnList = new List<double>();
            List<string> materialNames = ListUniqueMaterialNames();

            foreach (string materialName in materialNames)
            {
                rtnList.Add(Convert.ToDouble(elementsDataTable.Compute($"SUM({column})", $"MaterialName = '{materialName}' ")));
            }
            return rtnList;

        }



        //UNTESTED !!
        public List<double> GetColumnSum_ListByElement_Name(string column)
        {

            AdjustColumnForAllowSequestration(ref column);

            List<double> rtnList = new List<double>();
            List<string> element_Names = ListUniqueElement_Names();

            foreach (string element_Name in element_Names)
            {
                rtnList.Add(Convert.ToDouble(elementsDataTable.Compute($"SUM({column})", $"Element_Name = '{element_Name}' ")));
            }
            return rtnList;

        }
        //UNTESTED !!
        public List<double> GetColumnSum_ListByElement_Group(string column)
        {

            AdjustColumnForAllowSequestration(ref column);

            List<double> rtnList = new List<double>();
            List<string> element_gorups = ListUniqueElement_Groups();

            foreach (string element_gorup in element_gorups)
            {
                rtnList.Add(Convert.ToDouble(elementsDataTable.Compute($"SUM({column})", $"Element_Group = '{element_gorup}' ")));
            }
            return rtnList;

        }

        public List<double> GetColumnPercentage_ListByMaterial(string column)
        {

            AdjustColumnForAllowSequestration(ref column);

            List<double> rtnList = new List<double>();
            List<string> materialNames = ListUniqueMaterialNames();

            foreach (string materialName in materialNames)
            {
                double _value = (Convert.ToDouble(elementsDataTable.Compute($"SUM({column})", $" MaterialName = '{materialName}'")));
                rtnList.Add(_value / this.GetColumnSum(column) * 100);
            }
            return rtnList;

        }

        public List<double> GetColumnPercentage_ListByElement(string column, double columnSum)
        {

            AdjustColumnForAllowSequestration(ref column);

            List<double> rtnList = new List<double>();
            List<string> materialNames = ListUniqueMaterialNames();

            foreach (string materialName in materialNames)
            {
                double _value = (Convert.ToDouble(elementsDataTable.Compute($"SUM({column})", $" MaterialName = '{materialName}'")));
                rtnList.Add(_value / columnSum * 100);
            }
            return rtnList;

        }




        public int ListSum(List<int> list)
        {
            int sum = 0;

            foreach (int item in list)
            {
                sum += item;
            }

            return sum;

        }
        public double ListSum(List<double> list)
        {
            double sum = 0;

            foreach (double item in list)
            {
                sum += item;
            }

            return sum;

        }


    }
}
