using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace LCA_Toolbox
{
    public class LCA_Element
    {
        public IGH_GeometricGoo geoGoo { get; set; }
        public LCA_Material Material { get; set; }
        public int Element_Lifetime { get; set; }

        public double Element_A1toA3
        {
            get
            {
                return Element_Volume * Material.A1toA3;
            }
        }
        public double Element_A1toA3_noSeq
        {
            get
            {
                return Math.Max(Element_A1toA3, 0);
            }
        }

        public double Element_A4_perKG = 0;
        public double Element_A4
        {
            get
            {
                double cost = Element_A4_perKG * Element_Weight;
                if (cost > 0)
                    return cost;
                else
                    return 0;
            }
        }

        public List<int> Element_B4years { get; set; }
        public int Element_B4_Nreplacements { get; set; }
        public double Element_B4_perTime { get; set; }
        public double Element_B4_perTime_noSeq { get; set; }
        public double Element_B4_Sum { get; set; }
        public double Element_B4_Sum_noSeq { get; set; }
        public double Element_C1toC4_perTime
        {
            get
            {
                return Element_Volume* Material.C1toC4;
            }
        }
        public double Element_C1toC4
        {
            get
            {
                return Element_C1toC4_perTime * (Element_B4_Nreplacements + 1);
            }
        }
        public double Element_D_ReusePercent { get; set; }
        public string Element_Name { get; set; }
        public string Element_Group { get; set; }


        public LCA_Element(LCA_Material material, double volume, int expectedLifetime)
        {
            Element_Volume = volume;
            Material = material;
            Element_Lifetime = expectedLifetime;

            Element_B4_Nreplacements = 0;
            Element_B4_Sum = 0;
            Element_B4_perTime = 0;
            Element_B4years = new List<int>();
        }

        public LCA_Element()
        {
        }

        public bool isValid()
        {
            if(Material == null) return false;
            if(Element_Volume <= 0) return false;

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

        public double Element_Weight
        {
            get
            {
                return Element_Volume * Material.Density;
            }
        }

    }
}
