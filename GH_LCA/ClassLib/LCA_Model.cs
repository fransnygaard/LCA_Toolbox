using Grasshopper.GUI;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GH_LCA
{

    public class LCA_Model
    {

        public DataTable elementsDataTable;
        private DataTable timeline;

        public bool AllowSequestration = true;
        int modelLifetime { get; set; }
        public double model_GWP_B6_perYear { get; set; }

        List<string> listofAllGWPstages = new List<string> { "Element_GWP_A13", "Element_A4", "Element_B4", "Element_C", "Element_D" };


        protected LCA_Model(LCA_Model other)
        {
            // Cloning code goes here...
            this.elementsDataTable = other.elementsDataTable.Copy();
            this.timeline = other.timeline.Copy();

            this.AllowSequestration = other.AllowSequestration;
            this.modelLifetime = other.modelLifetime;   
            this.model_GWP_B6_perYear = other.model_GWP_B6_perYear;

        }

        public LCA_Model Clone()
        {
            return new LCA_Model(this);
        }
        public LCA_Model(List<LCA_Element> elements,int _lifetime, double B6_perYear)
        {
            modelLifetime = _lifetime;
            if (elements[0] != null)
                CalculateB4_allElements(ref elements);
                CreateDataTableFromListOfElements(elements);

            model_GWP_B6_perYear = B6_perYear;

            timeline = ConstructTimelineDataTable();
        }

        public DataTable ConstructTimelineDataTable()
        {
            DataTable rtnDT = new DataTable();

            rtnDT.Columns.Add(new DataColumn("A1-A3", typeof(double))); //Production phase
            rtnDT.Columns.Add(new DataColumn("A4", typeof(double))); //Trasport to site
            rtnDT.Columns.Add(new DataColumn("A5", typeof(double))); //Assembly
            rtnDT.Columns.Add(new DataColumn("B1", typeof(double))); //Use
            rtnDT.Columns.Add(new DataColumn("B2", typeof(double))); //Maintenence
            rtnDT.Columns.Add(new DataColumn("B3", typeof(double))); //Repair
            rtnDT.Columns.Add(new DataColumn("B4", typeof(double))); //Replacement
            rtnDT.Columns.Add(new DataColumn("B5", typeof(double))); //Refurbishment
            rtnDT.Columns.Add(new DataColumn("B6", typeof(double))); //operational energy
            rtnDT.Columns.Add(new DataColumn("C1-C4", typeof(double))); // End of life
            rtnDT.Columns.Add(new DataColumn("D", typeof(double))); //Beyond lifetime

            foreach (DataColumn col in rtnDT.Columns)
            { col.DefaultValue = 0; }


            //init all values to 0 and and add B6 every year.
            for (int i = 0; i < modelLifetime; i++)
            {
                DataRow row = rtnDT.NewRow();

                row.SetField("B6", model_GWP_B6_perYear);

                rtnDT.Rows.Add(row);
            }

            // Set Year 0
            rtnDT.Rows[0].SetField("A1-A3", GetColumnSum("Element_GWP_A13"));
            rtnDT.Rows[0].SetField("A4", GetColumnSum("Element_A4"));

            //Set B4 replacements

            foreach (DataRow element in elementsDataTable.Rows)
            {
                foreach(int year in (List<int>)element["Element_B4years"])
                {
                    rtnDT.Rows[year].SetField("B4", (double)rtnDT.Rows[0]["B4"] + (double)element["Element_B4perTime"]);
                    rtnDT.Rows[year].SetField("C1-C4", (double)rtnDT.Rows[0]["C1-C4"] + (double)element["Element_C"]);
                    rtnDT.Rows[year].SetField("D", (double)rtnDT.Rows[0]["D"] + (double)element["Element_D"]);
                }
            }


            // Set Year n

            rtnDT.Rows[modelLifetime-1].SetField("C1-C4", GetColumnSum("Element_C"));
            rtnDT.Rows[modelLifetime-1].SetField("D", GetColumnSum("Element_D"));

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
            private void CalculateB4_allElements(ref List<LCA_Element> elements)
            {
                if (modelLifetime <= 0)
                    return;

                foreach (LCA_Element element in elements)
                {
                    if (element.Element_ExpectedLifetime > 0 && element.Element_ExpectedLifetime < modelLifetime)
                    {

                        // 40 / 40 = 1 (-1 = 0replacements)
                        // 50 / 25 = 2 (-1 = 1replacements)
                        // 50 / 30 = 2 (-1 = 1replacements)
                        //find years to replace element
                        element.Element_B4years.Clear();
                        for (int i = element.Element_ExpectedLifetime; i < modelLifetime; i += element.Element_ExpectedLifetime)
                        {
                            element.Element_B4years.Add(i);
                        }



                        double element_A13 = AllowSequestration ? element.Element_GWP_A13 : element.Element_GWP_A13_noSeq;
                        //element.Element_B4_Nreplacements = (int)Math.Ceiling((double)(modelLifetime) / element.Element_ExpectedLifetime)-1;
                        element.Element_B4_Nreplacements = element.Element_B4years.Count();
                    element.Element_B4perTime = element_A13 + element.Element_A4; // + element.Element_C + element.Element_D;
                        element.Element_B4 = element.Element_B4perTime * (element.Element_B4_Nreplacements);


                    }
                }
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
        
        public double GetGWPAllStages()
        {
            double sum = GetColumnSum(listofAllGWPstages);
            return sum; 
        }

        private void AdjustColumnForAllowSequestration(ref string column)
        {
            if (!AllowSequestration && column == "Element_GWP_A13")
                column = "Element_GWP_A13_noSeq";
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




        public int ListSum(List<int> list )
        {
            int sum = 0;

            foreach(int item in list)
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
