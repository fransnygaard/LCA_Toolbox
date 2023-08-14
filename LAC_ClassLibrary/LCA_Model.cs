using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;

namespace GH_LCA
{

    public class LCA_Model
    {

        public DataTable elementsDataTable;
        int calculatedLifetime { get; set; }



        public LCA_Model(List<LCA_Element> elements,int _lifetime)
        {
            calculatedLifetime = _lifetime;
            if (elements[0] != null)
                CalculateB4_allElements(ref elements);
                CreateDataTableFromListOfElements(elements);
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
            if (calculatedLifetime <= 0)
                return;

            foreach (LCA_Element element in elements)
            {
                if (element.Element_ExpectedLifetime > 1 && element.Element_ExpectedLifetime < calculatedLifetime)
                {

                    // 40 / 40 = 1
                    // 50 / 25 = 2
                    // 50 / 30 = 2

                    element.Element_B4_Nreplacements = (calculatedLifetime) / element.Element_ExpectedLifetime;
                    double b4_perTime = element.Element_GWP + element.Element_A4 + element.Element_C + element.Element_D;
                    element.Element_B4 = b4_perTime * (element.Element_B4_Nreplacements-1);
                }
            }
        }


        public List<string> ListUniqueMaterialNames()
        {
            var uniqueNames = elementsDataTable.AsEnumerable().Select(row => row["MaterialName"]).Distinct().ToList();

            return uniqueNames.Cast<string>().ToList();
        }
        public double GetCollumnSum(string collumn)
        {
            return Convert.ToDouble(elementsDataTable.Compute($"SUM({collumn})", string.Empty));
        }

        public List<double> GetCollumnSum_ListByMaterial(string collumn)
        {
            List<double> rtnList = new List<double>();
            List<string> materialNames = ListUniqueMaterialNames();

            foreach (string materialName in materialNames)
            {
                rtnList.Add(Convert.ToDouble(elementsDataTable.Compute($"SUM({collumn})", $"MaterialName = '{materialName}' ")));
            }
            return rtnList;

        }
        public List<double> GetCollumnPercentage_ListByMaterial(string collumn, double collumnSum)
        {
            List<double> rtnList = new List<double>();
            List<string> materialNames = ListUniqueMaterialNames();

            foreach (string materialName in materialNames)
            {
                double _value = (Convert.ToDouble(elementsDataTable.Compute($"SUM({collumn})", $" MaterialName = '{materialName}'")));
                rtnList.Add(_value / collumnSum * 100);
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
