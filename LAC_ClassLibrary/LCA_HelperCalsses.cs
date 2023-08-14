using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace LAC_ClassLibrary
{
    public static class LCA_HelperCalss
    {

        //Return number in meters, scaled from current rhino units
        public static double convertValueToMeters(double value)
        {
            Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;
            Rhino.UnitSystem system = doc.ModelUnitSystem;
            switch (system.ToString())
            {
                case "Meters":
                    return value;
                case "Millimeters":
                    return value * 1e-3;
                case "Centimeters":
                    return value * 1e-2;
                default:
                    return double.NaN;
            }
        }

        //Return number in meters, scaled from current rhino units
        public static double convertSquaredValueToMeters(double value)
        {
            Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;
            Rhino.UnitSystem system = doc.ModelUnitSystem;
            switch (system.ToString())
            {
                case "Meters":
                    return value;
                case "Millimeters":
                    return value * 1e-6;
                case "Centimeters":
                    return value * 1e-4;
                default:
                    return double.NaN;
            }
        }


        public static double convertCubedValueToMeters(double value)
        {
            Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;
            Rhino.UnitSystem system = doc.ModelUnitSystem;
            switch (system.ToString())
            {
                case "Meters":
                    return value;
                case "Millimeters":
                    return value * 1e-9;
                case "Centimeters":
                    return value * 1e-6;
                default:
                    return double.NaN;
            }
        }

        //This tests if the voulme can be calculated , returns -1 if calculation fails.
        public static double CalculateVolumeFromGeoGoo(IGH_GeometricGoo goo, GH_Component selfref)
        {
            double volume = -1;

            //IGH_GeometricGoo geoGoo = shape;
            if (goo is Mesh || goo is GH_Mesh)
            {
                var geobase = GH_Convert.ToGeometryBase(goo);

                Mesh mesh = geobase as Mesh;

                if (!mesh.IsClosed)
                {
                    selfref.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Mesh needs to be closed to calculate volume. Use the \"Element from shell\" component");
                    return -1;
                }
                volume = mesh.Volume();
            }
            else if (goo is Brep || goo is GH_Brep)
            {
                var geobase = GH_Convert.ToGeometryBase(goo);
                Brep brep = geobase as Brep;


                if (!brep.IsSolid)
                {
                    selfref.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Brep needs to be solid to calculate volume. Use the \"Element from shell\" component");
                    return -1;
                }
                volume = brep.GetVolume();
            }
            else if (goo is Surface || goo is GH_Surface || goo is GH_Box)
            {
                var geobase = GH_Convert.ToGeometryBase(goo);
                Brep surface = geobase as Brep;


                if (!surface.IsSolid)
                {
                    selfref.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Surface needs to be solid to calculate volume. Use the \"Element from shell\" component");
                    return -1;
                }
                volume = surface.GetVolume();
            }


            return volume;
        }


    }

}
