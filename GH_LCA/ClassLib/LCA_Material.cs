using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LCA_Toolbox
{

    public class LCA_Material_List
    {
        public List<LCA_Material> list { get; set; }

        public LCA_Material_List() { 
        list = new List<LCA_Material>();
        }

        public LCA_Material_List(List<LCA_Material> list)
        {
            this.list = list;
        }
    }



    public class LCA_Material
    {
        DataTable dt;
        public LCA_Material(LCA_Material other)
        {
            this.dt = other.dt.Copy();
            Name = other.Name;
            Category = other.Category;

            Description = other.Description;
            Density = other.Density;
            Insulation = other.Insulation;

            ODP = other.ODP;
            POCP = other.POCP;
            EP = other.EP;
            AP = other.AP;

            DataSource = other.DataSource;
            Notes = other.Notes;
        }


        public LCA_Material()
        {
            dt = getNewMaterialDataTable();
        }


        private DataTable getNewMaterialDataTable()
        {
            DataTable rtnD = new DataTable();
            rtnD.Columns.Add(new DataColumn("Mat_A1_A3", typeof(double))); //Production phase
            rtnD.Columns.Add(new DataColumn("Mat_A4", typeof(double))); //Trasport to site per kg
            //rtnD.Columns.Add(new DataColumn("A5", typeof(double))); //Trasport to site
            //rtnD.Columns.Add(new DataColumn("B1", typeof(double))); //Use
            //rtnD.Columns.Add(new DataColumn("B2", typeof(double))); //Maintenence
            //rtnD.Columns.Add(new DataColumn("B3", typeof(double))); //Repair
            //rtnD.Columns.Add(new DataColumn("B4", typeof(double))); //Replacement
            //rtnD.Columns.Add(new DataColumn("B5", typeof(double))); //Refurbishment
            //rtnD.Columns.Add(new DataColumn("B6", typeof(double))); //operational energy
            rtnD.Columns.Add(new DataColumn("Mat_C1_C4", typeof(double))); // End of life
            rtnD.Columns.Add(new DataColumn("Mat_D", typeof(double))); //Beyond lifetime

            foreach (DataColumn col in rtnD.Columns)
            {
                col.DefaultValue = 0;  
            }

            rtnD.Rows.Add(rtnD.NewRow());

            return rtnD;

        }

        private string name;
        public string Name { get { return this.name; } set { this.name = value; } }

        private string category; 
        public string Category
        {
            get
            {
                if (this.category == string.Empty || this.category == null)
                    return "unknown category";
                else
                    return this.category;
            }
            set 
            {
                this.category = value;
            }
        }

        public string Description { get; set; }
        public double Density { get; set; }
        public double Insulation { get; set; }
        public double A1toA3 { get { return (double)dt.Rows[0]["Mat_A1_A3"]; } set { dt.Rows[0]["Mat_A1_A3"] = value; } }
        public double A4 { get { return (double)dt.Rows[0]["Mat_A1_A3"]; } set { dt.Rows[0]["Mat_A1_A3"] = value; } }
        public double ODP { get; set; }
        public double POCP { get; set; }
        public double EP { get; set; }
        public double AP { get; set; }
        public double C1toC4 { get { return (double)dt.Rows[0]["Mat_C1_C4"]; } set { dt.Rows[0]["Mat_C1_C4"] = value; } }
        public double D { get { return (double)dt.Rows[0]["Mat_D"]; } set { dt.Rows[0]["Mat_D"] = value; } }
        public string DataSource { get; set; }
        public string Notes { get; set; }


    }
}
