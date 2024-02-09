/*
 using System;
using System.Collections.Generic;

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
        public LCA_Material(LCA_Material material)
        {
            Name = material.Name;
            Category = material.Category;

            Description = material.Description;
            Density = material.Density;
            Insulation = material.Insulation;

            GWP = material.GWP;
            ODP = material.ODP;
            POCP = material.POCP;
            EP = material.EP;
            AP = material.AP;

            A4_A5 = material.A4_A5;
            C = material.C;
            D = material.D;

            DataSource = material.DataSource;
            MaterialGUID = Guid.NewGuid().ToString();

        }


        public LCA_Material()
        {
            MaterialGUID = Guid.NewGuid().ToString();
        }


        //

        //Name 
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
        public double GWP { get; set; }

        public double ODP { get; set; }
        public double POCP { get; set; }
        public double EP { get; set; }
        public double AP { get; set; }


        // TravelDistance to calculate A4 cost
        public double A4_A5 { get; set; }

        public double C { get; set; }

        public double D { get; set; }

        public string DataSource { get; set; }
        public string MaterialGUID{ get; set; }
    }
}
*/
