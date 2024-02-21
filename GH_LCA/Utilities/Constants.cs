using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace LCA_Toolbox
{
    public class Constants
    {
        #region naming 

        public static string PluginName
        {
            get { return "LCA_Toolbox"; }

        }

        public static string PluginDescription
        {
            get { return "Plugin to estimate the climate impact of your design"; }
        }
        public static string PluginLongName
        {
            get { return PluginName + " v" + Major + "." + Minor; }
        }
        private static string Minor
        {
            get { return typeof(Constants).Assembly.GetName().Version.Minor.ToString(); }
        }
        private static string Major
        {
            get { return typeof(Constants).Assembly.GetName().Version.Major.ToString(); }
        }
        public static string SubMaterials
        {
            get { return "01 Materials"; }
        }
        public static string SubElements
        {
            get { return "02 Elements"; }
        }
        public static string SubModel
        {
            get { return "03 Model"; }
        }
        public static string SubResults
        {
            get { return "04 Results"; }
        }
        public static string SubUtilities
        {
            get { return "04 Utilities"; }
        }

        public static string Tools
        {
            get { return "Misc Tools"; }

        }

        public static string SubCategory_Permutations
        {
            get { return "Combinations and Variations"; }

        }


        #endregion naming

        //Name, NickName, Input, Output, Outputs.

        #region inputOutput

        public static Descriptor JSON
        {
            get { return new Descriptor("JSON", "J", "plain text containing one or more materials coded as JSON objects."); }
        }

        public static Descriptor FilePath
        {
            get { return new Descriptor("FilePath", "/file", "use the filepath component to select a file"); }
        }

        public static Descriptor Folder
        {
            get { return new Descriptor("Folder", "/folder", "use the filepath component to select a local or network folder"); }
        }
        public static Descriptor Model
        {
            get { return new Descriptor("Model", "M", ""); }
        }
        public static Descriptor Element
        {
            get { return new Descriptor("Element", "E", ""); }
        }
        public static Descriptor Elements
        {
            get { return new Descriptor("Elements[]", "E[]", ""); }
        }
        public static Descriptor Element_Name
        {
            get { return new Descriptor("Element name", "N", "A discriptive name used for sorting and filtering. "); }
        }
        public static Descriptor Element_Group
        {
            get { return new Descriptor("Element group", "C", "A discriptive group used for sorting and filtering. ex. structural,slabs, cladding...."); }
        }
        public static Descriptor Material
        {
            get { return new Descriptor("Material", "Mat", ""); }
        }
        public static Descriptor Mat_Name
        {
            get { return new Descriptor("Material Name", "N", "Material name. Shows up in reports and can be used for filtering"); }
        }
        public static Descriptor Mat_Category
        {
            get { return new Descriptor("Category", "C", "Material Category. Shows up in reports and can be used for filtering"); }
        }

        public static Descriptor Mat_Description
        {
            get { return new Descriptor("Description", "D", "Material Description"); }
        }



        public static Descriptor Density
        {
            get { return new Descriptor("Density[kg / m3]", "D", "Material density messured in kg/m3. This must be a positive value"); }

        }

        public static Descriptor Insulation
        {
            get { return new Descriptor("Insulation[W / m2 / K]", "Iso", "Thermal conductivity (lambda) of the material, usage not yet implemented..."); }

        }
        public static Descriptor GWP_A1_A3
        {
            get { return new Descriptor("GWP[kg CO2eq / m3] A1-A3", "GWP A1-A3", "Global warming potential (GWP)\n is a measure of how much infrared thermal radiation a greenhouse gas added to the atmosphere would absorb over a given time frame, as a multiple of the radiation that would be absorbed by the same mass of added carbon dioxide (CO2).\n This is for stages A1 to A3"); }

        }

        public static Descriptor GWP_B6
        {
            get { return new Descriptor("B6 [kg CO2eq / year]", "B6", "Global warming potential (GWP)\n is a measure of how much infrared thermal radiation a greenhouse gas added to the atmosphere would absorb over a given time frame, as a multiple of the radiation that would be absorbed by the same mass of added carbon dioxide (CO2).\n This is for stage B6 Operational Energy"); }

        }
        public static Descriptor GWP_ELEMENT
        {
            get { return new Descriptor("Element A1-A3[kg CO2eq]", "Element A1-A3", "Global warming potential (GWP)\n is a measure of how much infrared thermal radiation a greenhouse gas added to the atmosphere would absorb over a given time frame, as a multiple of the radiation that would be absorbed by the same mass of added carbon dioxide (CO2)."); }

        }
        public static Descriptor GWP_TOTAL
        {
            get { return new Descriptor("GWP [kg CO2eq] Total", "GWP_Total", "Global warming potential (GWP)\n is a measure of how much infrared thermal radiation a greenhouse gas added to the atmosphere would absorb over a given time frame, as a multiple of the radiation that would be absorbed by the same mass of added carbon dioxide (CO2)."); }
             
        }
        public static Descriptor ODP
        {
            get { return new Descriptor("ODP[R11 eq / m3]", "ODP", "Ozone depletion potential (ODP)\n of a chemical compound is the relative amount of degradation to the ozone layer it can cause, with trichlorofluoromethane (R-11 or CFC-11) being fixed at an ODP of 1.0."); }

        }




        public static Descriptor Lifetime
        {
            get { return new Descriptor("Expected lifetime [years]", "Lifetime", "This is the expected lifetime of this element before it has to be replaced. At end of life ... D + new A1-4 \n a value og -1 means infinite life."); }

        }

        public static Descriptor Weight
        {
            get { return new Descriptor("Weight[kg]", "kg", "Total weight in kilograms [kg]"); }

        }
        public static Descriptor Volume
        {
            get { return new Descriptor("Volume[m3]", "m3", "Total volume in cubic meters [m3]."); }

        }

        public static Descriptor CrossSection
        {
            get { return new Descriptor("Cross section area [Rhino Units^2]", "Cs", "Cross section of element [Rhino Units^2]"); }

        }
        public static Descriptor Thickness
        {
            get { return new Descriptor("Thickness[Rhino Units]", "T", "Thickness of shell"); }

        }

        #region geo

        public static Descriptor ShellGeo
        {
            get { return new Descriptor("Shell Geomerty", "Geo", "Geomerty of an open shell , NURBS or mesh. For solids use LCA:Element from solid."); }
        }

        public static Descriptor Curve
        {
            get { return new Descriptor("Curve", "Crv", ""); }
        }

        public static Descriptor Curves
        {
            get { return new Descriptor("Curves[]", "Crv[]", "List of curves"); }
        }

        public static Descriptor SolidGeo
        {
            get { return new Descriptor("Solid Geomerty", "Geo", "Geomerty of an closed volume , NURBS or mesh. For open geomerty use LCA:Element from shell."); }
        }

        public static Descriptor Point
        {
            get { return new Descriptor("Point", "Pt", "Point"); }
        }
        public static Descriptor Points
        {
            get { return new Descriptor("Points[]", "Pt[]", "List of points"); }
        }
        public static Descriptor Plane
        {
            get { return new Descriptor("Plane", "P", "Plane"); }


        }

        #endregion geo

        public static Descriptor AllowSequestration
        {
            get
            {
                return new Descriptor("Allow Carbon Sequestration", "Seq", "if false negative GWP values are set to 0 to not account for Carbon Sequestration\n" +
                "default is False. if carbon Sequestration is to be accounted for stage D4 should be set so not to get unrealistic hopes :-) ");
            }
        }

        #endregion inputOutput

    }
    public class Descriptor
    {
        private string name = string.Empty;
        private string nickname = string.Empty;
        private string description = string.Empty;

        public Descriptor(string name, string nickname,string disctiption)
        {
            this.name = name;
            this.nickname = nickname;
            this.description = disctiption;
        }

        public static implicit operator string(Descriptor d)
        {
            return d.name;
        }

        public virtual string Name
        {
            get { return name; }
        }

        public virtual string NickName
        {
            get { return nickname; }
        }
              
       public virtual string Discription
        {
            get { return description; }
        }


    }
}
