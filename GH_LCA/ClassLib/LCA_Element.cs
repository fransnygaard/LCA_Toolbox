using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace LCA_Toolbox
{
    public class LCA_Element
    {

        public string ElementGUID { get; set; }
        public LCA_Material Material { get; set; }

        //Used to calculate B4 (Replacement)
        public int Element_ExpectedLifetime { get; set; }

        //Number of B4 replacements , caclulated at model level
        public int Element_B4_Nreplacements { get; set; }
        public double Element_B4 { get; set; }

        public List<int> Element_B4years { get; set; }
        public double Element_B4perTime { get; set; }
        


        public string Element_Name { get; set; }
        public string Element_Group { get; set; }

        public LCA_Element(LCA_Material material, double volume, int expectedLifetime)
        {
            Element_Volume = volume;
            Material = material;
            Element_ExpectedLifetime = expectedLifetime;

            Element_B4_Nreplacements = 0;
            Element_B4 = 0;
            Element_B4perTime = 0;
            Element_B4years = new List<int>();

            ElementGUID = Guid.NewGuid().ToString();
        }

        public LCA_Element()
        {
        }

        public bool isValid()
        {
            if(Material == null) return false;
            if(Element_Volume == 0) return false;

            return true;
        }

        public string MaterialName
        {
            get
            {
                return Material.Name;
            }
        }

        public double Element_Volume { get; set; }

        //A1 - A3
        public double Element_GWP_A13
        {
            get
            {
                return Element_Volume * Material.A1toA3;
            }
        }
        public double Element_GWP_A13_noSeq
        {
            get
            {
                return Math.Max(Element_GWP_A13, 0);
            }
        }
        public double Element_ODP
        {
            get
            {
                return Element_Volume * Material.ODP;
            }
        }
        public double Element_POCP
        {
            get
            {
                return Element_Volume * Material.POCP;
            }
        }
        public double Element_EP
        {
            get
            {
                return Element_Volume * Material.EP;
            }
        }
        public double Element_AP
        {
            get
            {
                return Element_Volume * Material.AP;
            }
        }

        public double Element_A4
        {
            get
            {
                double cost = Material.A4 * Element_Weight;
                if (cost > 0)
                    return cost;
                else
                    return 0;
            }
        }

        public double Element_C
        {
            get
            {
                double cost = Material.C1toC4 * Element_Weight;
                if (cost > 0)
                    return cost;
                else
                    return 0;
            }
        }
        public double Element_D
        {
            get
            {
                double cost = Material.D * Element_Weight;
                if (cost > 0)
                    return cost;
                else
                    return 0;
            }
        }

        public double Element_Weight
        {
            get
            {
                return Element_Volume * Material.Density;
            }
        }

    }
}
