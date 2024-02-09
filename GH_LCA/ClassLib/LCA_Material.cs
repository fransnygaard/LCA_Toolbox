using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GH_LCA
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

            //A1toA3 = other.A1toA3;
            //A4 = other.A4;
            // C1toC4 = other.C1toC4;
            //D = other.D;

            DataSource = other.DataSource;
            MaterialGUID = Guid.NewGuid().ToString();

        }


        public LCA_Material()
        {
            dt = getNewMaterialDataTable();
            MaterialGUID = Guid.NewGuid().ToString();
        }


        private DataTable getNewMaterialDataTable()
        {
            DataTable rtnD = new DataTable();
            rtnD.Columns.Add(new DataColumn("A1-A3", typeof(double))); //Production phase
            rtnD.Columns.Add(new DataColumn("A4", typeof(double))); //Trasport to site
            rtnD.Columns.Add(new DataColumn("A5", typeof(double))); //Trasport to site
            rtnD.Columns.Add(new DataColumn("B1", typeof(double))); //Use
            rtnD.Columns.Add(new DataColumn("B2", typeof(double))); //Maintenence
            rtnD.Columns.Add(new DataColumn("B3", typeof(double))); //Repair
            rtnD.Columns.Add(new DataColumn("B4", typeof(double))); //Replacement
            rtnD.Columns.Add(new DataColumn("B5", typeof(double))); //Refurbishment
            rtnD.Columns.Add(new DataColumn("B6", typeof(double))); //operational energy
            rtnD.Columns.Add(new DataColumn("C1-C4", typeof(double))); // End of life
            rtnD.Columns.Add(new DataColumn("D", typeof(double))); //Beyond lifetime

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

        //Density [kg/m3]
        public double Density { get; set; }

        public double Insulation { get; set; }

        //GWP [kg CO2 eq / m3 ]
        public double A1toA3 { get { return (double)dt.Rows[0]["A1-A3"]; } set { dt.Rows[0]["A1-A3"] = value; } }
        public double GWP { get { return (double)dt.Rows[0]["A1-A3"]; } set { dt.Rows[0]["A1-A3"] = value; } }


        public double ODP { get; set; }
        public double POCP { get; set; }
        public double EP { get; set; }
        public double AP { get; set; }


        // TravelDistance to calculate A4 cost
        public double A4 { get { return (double)dt.Rows[0]["A4"]; } set { dt.Rows[0]["A4"] = value; } }

        public double C1toC4 { get { return (double)dt.Rows[0]["C1-C4"]; } set { dt.Rows[0]["C1-C4"] = value; } }

        public double D { get { return (double)dt.Rows[0]["D"]; } set { dt.Rows[0]["D"] = value; } }

        public string DataSource { get; set; }
        public string MaterialGUID{ get; set; }
    }
}
