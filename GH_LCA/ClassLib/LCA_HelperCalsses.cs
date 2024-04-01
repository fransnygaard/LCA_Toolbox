using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LAC_ClassLibrary
{

    static class ColorInterpolator
    {
        delegate byte ComponentSelector(Color color);
        static ComponentSelector _redSelector = color => color.R;
        static ComponentSelector _greenSelector = color => color.G;
        static ComponentSelector _blueSelector = color => color.B;

        public static Color InterpolateBetween(
            Color endPoint1,
            Color endPoint2,
            double lambda)
        {
            if (lambda < 0 || lambda > 1)
            {
                throw new ArgumentOutOfRangeException("lambda");
            }
            Color color = Color.FromArgb(
                InterpolateComponent(endPoint1, endPoint2, lambda, _redSelector),
                InterpolateComponent(endPoint1, endPoint2, lambda, _greenSelector),
                InterpolateComponent(endPoint1, endPoint2, lambda, _blueSelector)
            );

            return color;
        }

        static byte InterpolateComponent(
            Color endPoint1,
            Color endPoint2,
            double lambda,
            ComponentSelector selector)
        {
            return (byte)(selector(endPoint1)
                + (selector(endPoint2) - selector(endPoint1)) * lambda);
        }
    }


    public static class LCA_HelperCalss
    {

        public static List<double> NormalizeValues(this List<double> values)
        {
            List<double> result = values;
            double Min = values.Min();
            double Max = values.Max();
            for (int i = 0; i < values.Count; i++)
            {
                result[i] = (result[i] - Min) / (Max - Min);

            }

            return result;

        }
        public static double Interpolate(double value, double oldMin, double oldMax, double newMin, double newMax)
        {
            return (double)(newMin + (newMax - newMin) * (value - oldMin) / (oldMax - oldMin));
        }


        //Return number in meters, scaled from current rhino units
        public static double convertValueToMeters(double value)
        {

            double scaleFactor = Rhino.RhinoMath.UnitScale(Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem, Rhino.UnitSystem.Meters);
            
            return value * scaleFactor;

            //Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;
            //Rhino.UnitSystem system = doc.ModelUnitSystem;
            //switch (system.ToString())
            //{
            //    case "Meters":
            //        return value;
            //    case "Millimeters":
            //        return value * 1e-3;
            //    case "Centimeters":
            //        return value * 1e-2;
            //    default:
            //        return double.NaN;
            //}
        }

        //Return number in meters, scaled from current rhino units
        public static double convertSquaredValueToMeters(double value)
        {

            double scaleFactor = Rhino.RhinoMath.UnitScale(Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem, Rhino.UnitSystem.Meters);
            double scaleFactor2 = scaleFactor * scaleFactor;
            return value * scaleFactor2;
            //''
            //Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;
            //Rhino.UnitSystem system = doc.ModelUnitSystem;
            //switch (system.ToString())
            //{
            //    case "Meters":
            //        return value;
            //    case "Millimeters":
            //        return value * 1e-6;
            //    case "Centimeters":
            //        return value * 1e-4;
            //    default:
            //        return double.NaN;
            //}
        }


        public static double convertCubedValueToMeters(double value)
        {
            double scaleFactor = Rhino.RhinoMath.UnitScale(Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem, Rhino.UnitSystem.Meters);
            double scaleFactor3 = scaleFactor * scaleFactor * scaleFactor;
            return value * scaleFactor3;

            //Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;
            //Rhino.UnitSystem system = doc.ModelUnitSystem;
            //switch (system.ToString())
            //{
            //    case "Meters":
            //        return value;
            //    case "Millimeters":
            //        return value * 1e-9;
            //    case "Centimeters":
            //        return value * 1e-6;
            //    default:
            //        return double.NaN;
            //}
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
