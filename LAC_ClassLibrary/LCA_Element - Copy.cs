namespace GH_LCA
{
    public class LCA_Element : LCA_Material
    {

        public LCA_Material Material { get; set; }

        //Used to calculate B4 (Replacement)
        public int Element_ExpectedLifetime { get; set; }

        public string Element_Group { get; set; }

        public LCA_Element(LCA_Material material, double volume) : base(material)
        {
            Element_Volume = volume;
            Material = material;
        }

        public double Element_Volume { get; set; }

        public double Element_GWP
        {
            get
            {
                return Element_Volume * GWP;
            }
        }
        public double Element_ODP
        {
            get
            {
                return Element_Volume * ODP;
            }
        }
        public double Element_POCP
        {
            get
            {
                return Element_Volume * POCP;
            }
        }
        public double Element_EP
        {
            get
            {
                return Element_Volume * EP;
            }
        }
        public double Element_AP
        {
            get
            {
                return Element_Volume * AP;
            }
        }

        public double Element_A4_Cost
        {
            get
            {
                return A4_Distance * A4_CostPerDisance;
            }
        }

        public double Element_Weight
        {
            get
            {
                return Element_Volume * density;
            }
        }

    }
}
