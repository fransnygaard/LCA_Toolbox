using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using LAC_ClassLibrary;
using System;

namespace GH_LCA
{
    public class LCA_ElementFromSolid_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the LCA_CreateElement_Component class.
        /// </summary>
        public LCA_ElementFromSolid_Component()
          : base("LCA Create Element from Solid", "LCA Create Element from Solid",
              "Description",
              "LCA", "02 Element")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "Geometry", "", GH_ParamAccess.item); //0
            pManager.AddGenericParameter("Material", "Material", "", GH_ParamAccess.item); //1
            pManager.AddIntegerParameter("Expected lifetime [years]", "Expected lifetime [years]", "Set to -1 to make it last forever", GH_ParamAccess.item, -1);//2


            pManager.AddTextParameter("ElementName", "ElementName", "", GH_ParamAccess.item); //3
            Params.Input[3].Optional = true;

            pManager.AddTextParameter("ElementGroup", "ElementGroup", "", GH_ParamAccess.item); //4
            Params.Input[4].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Element", "Element", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Volume[m3]", "Volume[m3]", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Weight[kg]", "Weight[kg}", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("GWP[kg CO2eq]", "GWP[kg CO2eq]", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IGH_GeometricGoo geoGoo = null;
            LCA_Material material = new LCA_Material();

            if (!DA.GetData<IGH_GeometricGoo>(0, ref geoGoo)) return;

            double volume = LCA_HelperCalss.CalculateVolumeFromGeoGoo(geoGoo, this);
            if (volume < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Was not able to calculate volume");
                return;
            }

            //Convert volume to meters
            volume = LCA_HelperCalss.convertCubedValueToMeters(volume);
            if (volume == double.NaN ){ AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Rhino units needs to be mm, cm or m"); return; }


            if (!DA.GetData<LCA_Material>(1, ref material))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid material input");
                return;
            }

            int expectedLifetime = -1;
            DA.GetData<int>(2, ref expectedLifetime);

            LCA_Element element = new LCA_Element(material, volume, expectedLifetime);

            string _tempStr = string.Empty;
            if (DA.GetData(3, ref _tempStr)) { element.Element_Name = _tempStr; }
            if (DA.GetData(4, ref _tempStr)) { element.Element_Group = _tempStr; }



            DA.SetData(0, element);
            DA.SetData(1, element.Element_Volume);
            DA.SetData(2, element.Element_Weight);
            DA.SetData(3, element.Element_GWP);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C3C03D90-EFB5-4675-B2EE-E57D0C382FA3"); }
        }
    }
}