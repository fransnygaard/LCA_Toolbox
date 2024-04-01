using Eto.Forms;
using Grasshopper.GUI;
using LAC_ClassLibrary;
using Rhino.ApplicationSettings;
using Rhino.Geometry;
using Rhino.Render;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LCA_Toolbox
{

    public class LCA_Model
    {

        public DataTable elementsDT;
        private DataTable timelineDT;

        public bool AllowSequestration = true;
        public bool TimeWeighting = false;
        int modelLifetime { get; set; }
        public List<double> model_B6_perYear { get; set; }

        List<string> EmbodiedCarbonStages {
            get 
            { 
                return new List<string>() { "Element_A1toA3", "Element_A4", "Element_B4_Sum", "Element_C1toC4" }; }
            }

        protected LCA_Model(LCA_Model other)
        {
            // Cloning code goes here...
            this.elementsDT = other.elementsDT.Copy();
            this.timelineDT = other.timelineDT.Copy();

            this.AllowSequestration = other.AllowSequestration;
            this.TimeWeighting = other.TimeWeighting;
            this.modelLifetime = other.modelLifetime;
            this.model_B6_perYear = other.model_B6_perYear;

        }

        public List<double> GetDataColumnFromTimeline(string column)
        {
            return timelineDT.AsEnumerable().Select(x => (double)x[column]).ToList();
        }

        public LCA_Model Clone()
        {
            return new LCA_Model(this);
        }

        public LCA_Model(List<LCA_Element> elements, int _lifetime, List<double> B6_perYears,bool _AllowSequestration, bool _TimeWeighting)
        {
            modelLifetime = _lifetime;
            model_B6_perYear = B6_perYears;
            AllowSequestration = _AllowSequestration;
            TimeWeighting = _TimeWeighting;
            CreateElementsDataTable(elements);
            CreateTimelineDataTable();
  

        }


        private void CreateElementsDataTable(List<LCA_Element> elements)
        {

            //Create empty table
            elementsDT = new DataTable();

            //Fill with all properies from LCA_Element class
            foreach (PropertyInfo info in typeof(LCA_Element).GetProperties())
            {
                elementsDT.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }


            //Add aditional collumns
            elementsDT.Columns.Add(new DataColumn("Sum_Embodied", typeof(double)));



            //Fill table wit all values 
            foreach (LCA_Element element in elements)
            {
                if (element == null) continue;
                DataRow dr = elementsDT.NewRow();
                foreach (PropertyInfo info in typeof(LCA_Element).GetProperties())
                {
                    dr[info.Name] = info.GetValue(element, null);
                }
                elementsDT.Rows.Add(dr);
            }

            //Calculate new values based on model

            //Adjust for Sequestration
            UpdateElementsTableForSequestration();
            UpdateElementsTable_replacements();
            UpdateElementsEmbodiedCarbon();

        }

        private void UpdateElementsTableForSequestration()
        {
            if (!AllowSequestration)
            {
                foreach (DataRow row in elementsDT.Rows)
                {
                    row["Element_A1toA3"] = Math.Max((double)row["Element_A1toA3"], 0.0);
                }
            }
        }
        public void CreateTimelineDataTable()
        {
            timelineDT = new DataTable();
            timelineDT.Columns.Add(new DataColumn("Year", typeof(double))); //year

            timelineDT.Columns.Add(new DataColumn("A1_A3", typeof(double))); //Production phase
            timelineDT.Columns.Add(new DataColumn("A4", typeof(double))); //Trasport to site
            timelineDT.Columns.Add(new DataColumn("A5", typeof(double))); //Assembly     // NOT IMPLEMENTED
            timelineDT.Columns.Add(new DataColumn("B1", typeof(double))); //Use          // NOT IMPLEMENTED
            timelineDT.Columns.Add(new DataColumn("B2", typeof(double))); //Maintenence  // NOT IMPLEMENTED
            timelineDT.Columns.Add(new DataColumn("B3", typeof(double))); //Repair       // NOT IMPLEMENTED
            timelineDT.Columns.Add(new DataColumn("B4", typeof(double))); //Replacement 
            timelineDT.Columns.Add(new DataColumn("B5", typeof(double))); //Refurbishment  // NOT IMPLEMENTED
            timelineDT.Columns.Add(new DataColumn("B6", typeof(double))); //operational energy
            timelineDT.Columns.Add(new DataColumn("B7", typeof(double))); //operational water // NOT IMPLEMENTED
            timelineDT.Columns.Add(new DataColumn("C1_C4", typeof(double))); // End of life
            timelineDT.Columns.Add(new DataColumn("D", typeof(double))); //Beyond lifetime  // NOT IMPLEMENTED


            timelineDT.Columns.Add(new DataColumn("Sum_Embodied", typeof(double))); //Sum_Embodied carbon per year 
            timelineDT.Columns.Add(new DataColumn("Sum_Operational", typeof(double))); //Sum_Operational carbon per year
            timelineDT.Columns.Add(new DataColumn("Sum_Carbon", typeof(double))); //Sum_ carbon per year
            timelineDT.Columns.Add(new DataColumn("Cumulative_Carbon", typeof(double))); //Cumulative per year


            //init all values to 0
            foreach (DataColumn col in timelineDT.Columns)
            { col.DefaultValue = 0.0; }


            // and and add B6 every year. 
            for (int i = 0; i < modelLifetime; i++)
            {
                
                DataRow row = timelineDT.NewRow();

                int year_index = i < model_B6_perYear.Count() ? i : model_B6_perYear.Count() - 1; //repate last of not enoung b6 values.

                row.SetField("Year", DateTime.Now.Year + i);
                row.SetField("B6", model_B6_perYear[year_index] * GetTimeWeight(i));

                timelineDT.Rows.Add(row);
            }

            // Set Year 0
            timelineDT.Rows[0].SetField("A1_A3", GetColumnSum("Element_A1toA3"));
            timelineDT.Rows[0].SetField("A4", GetColumnSum("Element_A4"));

            //Set B4 replacements

            foreach (DataRow row in elementsDT.Rows)
            {
                foreach (int year in (List<int>)row["Element_B4years"])
                {
                    if (year >= timelineDT.Rows.Count) { continue; }
                    timelineDT.Rows[year].SetField("B4", (double)timelineDT.Rows[year]["B4"] + ((double)row["Element_B4_perTime"]*GetCombinedTimeAndTechWeight(year)));
                    timelineDT.Rows[year].SetField("C1_C4", (double)timelineDT.Rows[year]["C1_C4"] + ((double)row["Element_C1toC4_perTime"]*GetCombinedTimeAndTechWeight(year)));
                }
            }


            // Set Year n

            timelineDT.Rows[modelLifetime - 1].SetField("C1_C4", GetColumnSum("Element_C1toC4_perTime")*GetCombinedTimeAndTechWeight(modelLifetime));

            

            UpdatetimelineSumAndCumulativeCarbon();


        }
        private void UpdatetimelineSumAndCumulativeCarbon()
        {
            double runningSumTotal = 0;


            foreach (DataRow row in timelineDT.Rows)
            {
                double runningSumEmbodied = 0;
                foreach (string s in GetListOfEmbodiedStages())
                {
                    runningSumEmbodied += (double)row[s];
                }
                row.SetField("Sum_Embodied", runningSumEmbodied);


                double runningSumOperational = 0;
                foreach (string s in GetListOfOperationalStages())
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

        private List<string> GetListOfEmbodiedStages()
        {

            return new List<string>
            {
                "A1_A3",
                "A4",
                "A5",
                "B1",
                "B2",
                "B3",
                "B4",
                "B5",
                "C1_C4",
                "D"
            };
        }

        private List<string> GetListOfOperationalStages()
        {
           return new List<string>
            {
                "B6",
                "B7"
            };
        }


        private void UpdateElementsEmbodiedCarbon()
        {
            foreach (DataRow row in elementsDT.Rows)
            {
                double runningSumEmbodied = 0;
                foreach (string s in EmbodiedCarbonStages)
                {
                    runningSumEmbodied += (double)row[s];
                }
                row.SetField("Sum_Embodied", runningSumEmbodied);
            }
        }




        //REMOVE WHEN TESTED.
        [Obsolete]    
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
                    //element.Element_B4_perTime_noSeq = element.Element_A1toA3_noSeq + element.Element_A4; //+ element.Element_D;
                    //element.Element_B4_Sum_noSeq = element.Element_B4_perTime_noSeq * element.Element_B4_Nreplacements;



                }
            }
        }

        private void UpdateElementsTable_replacements()
        {
            if (modelLifetime <= 0)
                return;

            foreach (DataRow row in elementsDT.Rows)
            {
                int rowLifetime = (int)row["Element_Lifetime"];
                if (rowLifetime > 0 && rowLifetime < modelLifetime)
                {

                    // 40 / 40 = 1 (-1 = 0replacements)
                    // 50 / 25 = 2 (-1 = 1replacements)
                    // 50 / 30 = 2 (-1 = 1replacements)
                    //find years to replace element
                    List<int> rowB4Years = new List<int>();

                    for (int i = rowLifetime; i < modelLifetime; i += rowLifetime)
                    {
                        rowB4Years.Add(i);
                    }
                    row["Element_B4years"] = rowB4Years;

                    row["Element_B4_Nreplacements"] = rowB4Years.Count();

                    row["Element_B4_perTime"] = (double)row["Element_A1toA3"] + (double)row["Element_A4"]; //+ element.Element_D;
                    row["Element_B4_Sum"] = (double)row["Element_B4_perTime"] * (int)row["Element_B4_Nreplacements"];

                    row["Element_C1toC4"] =   (double)row["Element_C1toC4_perTime"] * ((int)row["Element_B4_Nreplacements"] + 1);

                }
            }
        }


        //REMOVE WHEN TESTED.
        [Obsolete]
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
            elementsDT = elementsDT.Select($"{filterColumn} = '{filter}'").CopyToDataTable();
        }

        //UNTESTED !!
        public void FiterDataTableByMaterialName(string filter)
        {
            FilterDataTable(filter, "MaterialName");
        }
        //UNTESTED !!
        public void FiterDataTableByElementName(string filter)
        {
            elementsDT = elementsDT.Select($"Element_Name = '{filter}'").CopyToDataTable();
        }

        //UNTESTED !!
        public void FiterDataTableByElementGroup(string filter)
        {
            elementsDT = elementsDT.Select($"Element_Group = '{filter}'").CopyToDataTable();
        }



        public List<string> ListUniqueMaterialNames()
        {
            var uniqueItems = elementsDT.AsEnumerable().Select(row => row["MaterialName"]).Distinct().ToList();

            return uniqueItems.Cast<string>().ToList();
        }


        //UNTESTED !!
        public List<string> ListUniqueElement_Groups()
        {
            var uniqueItems = elementsDT.AsEnumerable().Select(row => row["Element_Group"]).Distinct().ToList();

            return uniqueItems.Cast<string>().ToList();
        }
        //UNTESTED !!
        public List<string> ListUniqueElement_Names()
        {
            var uniqueItems = elementsDT.AsEnumerable().Select(row => row["Element_Name"]).Distinct().ToList();

            return uniqueItems.Cast<string>().ToList();
        }


        public double GetColumnSum(string column)
        {
            //AdjustColumnForAllowSequestration(ref column);

            return Convert.ToDouble(elementsDT.Compute($"SUM({column})", string.Empty));

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




        public double GetEmbodied_SumModel()
        {
            return Convert.ToDouble(timelineDT.Compute("SUM(Sum_Embodied)",string.Empty));
        }
        public double GetOperational_SumModel()
        {
            return Convert.ToDouble(timelineDT.Compute("SUM(Sum_Operational)", string.Empty));
        }
        public double GetTotalGWP_SumModel()
        {
            return Convert.ToDouble(timelineDT.Compute("SUM(Sum_Carbon)", string.Empty));
        }





        //GET EMBODIED  , get collumn sum of "Sum_Embodied" with diffrent filtering.
        //  BY MATERIAL NAME
        public double GetEmbodied_SumByMaterialName(string materialName)
        {
            return Convert.ToDouble(elementsDT.Compute($"SUM(Sum_Embodied)", $"Material_Name = '{materialName}' "));
        }
        //  BY ELEMENT NAME
        public double GetEmbodied_SumByElementName(string element_Name)
        {
            return Convert.ToDouble(elementsDT.Compute($"SUM(Sum_Embodied)", $"Element_Name = '{element_Name}' "));
        }
        //  BY ELEMENT GROUP
        public double GetEmbodied_SumByElementGroup(string element_gorup)
        {
            return Convert.ToDouble(elementsDT.Compute($"SUM(Sum_Embodied)", $"Element_Group = '{element_gorup}' "));
        }


        #region USED IN DETAILED RESUALT
        //USED IN DETAILED RESUALT.

        public List<double> GetColumnSum_ListByMaterial(string column)
        {

            //AdjustColumnForAllowSequestration(ref column);

            List<double> rtnList = new List<double>();
            List<string> materialNames = ListUniqueMaterialNames();

            foreach (string materialName in materialNames)
            {
                rtnList.Add(Convert.ToDouble(elementsDT.Compute($"SUM({column})", $"MaterialName = '{materialName}' ")));
            }
            return rtnList;

        }



        //UNTESTED !!
        public List<double> GetColumnSum_ListByElement_Name(string column)
        {

            //AdjustColumnForAllowSequestration(ref column);

            List<double> rtnList = new List<double>();
            List<string> element_Names = ListUniqueElement_Names();

            foreach (string element_Name in element_Names)
            {
                rtnList.Add(Convert.ToDouble(elementsDT.Compute($"SUM({column})", $"Element_Name = '{element_Name}' ")));
            }
            return rtnList;

        }
        //UNTESTED !!
        public List<double> GetColumnSum_ListByElement_Group(string column)
        {

            //AdjustColumnForAllowSequestration(ref column);

            List<double> rtnList = new List<double>();
            List<string> element_gorups = ListUniqueElement_Groups();

            foreach (string element_gorup in element_gorups)
            {
                rtnList.Add(Convert.ToDouble(elementsDT.Compute($"SUM({column})", $"Element_Group = '{element_gorup}' ")));
            }
            return rtnList;

        }

        public List<double> GetColumnPercentage_ListByMaterial(string column)
        {

            //AdjustColumnForAllowSequestration(ref column);

            List<double> rtnList = new List<double>();
            List<string> materialNames = ListUniqueMaterialNames();

            foreach (string materialName in materialNames)
            {
                double _value = (Convert.ToDouble(elementsDT.Compute($"SUM({column})", $" MaterialName = '{materialName}'")));
                rtnList.Add(_value / this.GetColumnSum(column) * 100);
            }
            return rtnList;

        }

        public List<double> GetColumnPercentage_ListByElement(string column, double columnSum)
        {

            //AdjustColumnForAllowSequestration(ref column);

            List<double> rtnList = new List<double>();
            List<string> materialNames = ListUniqueMaterialNames();

            foreach (string materialName in materialNames)
            {
                double _value = (Convert.ToDouble(elementsDT.Compute($"SUM({column})", $" MaterialName = '{materialName}'")));
                rtnList.Add(_value / columnSum * 100);
            }
            return rtnList;

        }



        //Implementation of dynamic LCA https://futurebuilt-zero.web.app/regneregler/tidspunkt
        public double GetTimeWeight(int t)
        {
            if (t <= 0 || TimeWeighting == false)
            {
                return 1;
            }
            else
            {
                return 2 - (Math.Pow(Math.E, (0.00693 * t)));
            }
        }

        public double GetTechnologyDecay(int t, double decayFactor = 0.01)
        {
            if (t <= 0  || TimeWeighting == false)
            {
                return 1;
            }
            else
            {
                return Math.Pow(Math.E, (-decayFactor * t));
            }
        }

        public double GetCombinedTimeAndTechWeight(int t)
        {
            return GetTimeWeight(t) * GetTechnologyDecay(t);
        }

        #endregion USED IN DETAILED RESUALT


        #region NOT USED , CLEAN UP AT SOME POINT
        //NOT USED , CLEAN UP AT SOME POINT
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

        #endregion NOT USED , CLEAN UP AT SOME POINT
    }
}
