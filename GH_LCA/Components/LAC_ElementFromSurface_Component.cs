using System;
using System.Collections.Generic;
using LAC_ClassLibrary;
using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace GH_LCA
{
    public class LAC_ElementFromSurface_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the LAC_CreateElementFromSurface_Component class.
        /// </summary>
        public LAC_ElementFromSurface_Component()
          : base("LCA Create Element from Surface", "LCA Create Element from Surface",
              "Description",
              Constants.ShortName, Constants.SubElements)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "Surface", "", GH_ParamAccess.item); //0
            pManager.AddGenericParameter("Material", "Material", "", GH_ParamAccess.item); //1
            pManager.AddNumberParameter("Thickness [Rhino Units^2]", "Thickness", "", GH_ParamAccess.item);//2
            pManager.AddIntegerParameter("Expected lifetime [years]", "Expected lifetime [years]", "", GH_ParamAccess.item,-1);//3

            pManager.AddTextParameter("ElementName", "ElementName", "", GH_ParamAccess.item); //4
            Params.Input[4].Optional = true;

            pManager.AddTextParameter("ElementGroup", "ElementGroup", "", GH_ParamAccess.item); //5
            Params.Input[5].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Element", "Element", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Volume[m3]", "Volume[m3]", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Weight[kg]", "Weight[kg}", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("GWP A1-A3 [kg CO2eq]", "GWP A1-A3 [kg CO2eq]", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            LCA_Material material = new LCA_Material();

            if (!DA.GetData<Surface>(0, ref surface)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "no input Surface"); return; }
            if (surface == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Surface can not be NULL"); return; }
            
            if (!DA.GetData<LCA_Material>(1, ref material)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid material input"); return;}

            double thickness = 0;

            DA.GetData<double>(2, ref thickness);

            if(thickness <= 0) {  AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Thickness less than or equal to 0"); return; }

            thickness = LCA_HelperCalss.convertValueToMeters(thickness);
            double surfaceArea  = LCA_HelperCalss.convertSquaredValueToMeters(AreaMassProperties.Compute(surface).Area);
            if (surfaceArea == double.NaN || thickness == double.NaN) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Rhino units needs to be mm, cm or m"); return; }

            double volume = surfaceArea *  thickness;


            int expectedLifetime = -1;
            DA.GetData<int>(3, ref expectedLifetime);
            LCA_Element element = new LCA_Element(material, volume, expectedLifetime);

            string _tempStr = string.Empty;
            if (DA.GetData(4, ref _tempStr)) { element.Element_Name = _tempStr; }
            if (DA.GetData(5, ref _tempStr)) { element.Element_Group = _tempStr; }

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
            get { return new Guid("B3EE58AB-C735-485A-BC25-7536E4815EF5"); }
        }
    }
}