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
              Constants.PluginName, Constants.SubElements)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Material.Name, Constants.Material.NickName, Constants.Material.Discription, GH_ParamAccess.item);
            
            pManager.AddNumberParameter(Constants.Volume.Name, Constants.Volume.NickName, Constants.Volume.Discription, GH_ParamAccess.item);
            
            pManager.AddIntegerParameter(Constants.Lifetime.Name,Constants.Lifetime.NickName, Constants.Lifetime.Discription, GH_ParamAccess.item,-1);

            pManager.AddTextParameter(Constants.Element_Name.Name, Constants.Element_Name.NickName, Constants.Element_Name.Discription, GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true; 
            
            pManager.AddTextParameter(Constants.Element_Group.Name, Constants.Element_Group.NickName, Constants.Element_Group.Discription, GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;



        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Element.Name, Constants.Element.NickName, Constants.Element.Discription, GH_ParamAccess.item);
            
            pManager.AddNumberParameter(Constants.Weight.Name, Constants.Weight.NickName, Constants.Weight.Discription, GH_ParamAccess.item);
           
            pManager.AddNumberParameter(Constants.GWP_A1_A3.Name, Constants.GWP_A1_A3.NickName, Constants.GWP_A1_A3.Discription, GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Surface surface = null;
            //LCA_Material material = new LCA_Material();

            //if (!DA.GetData<Surface>(0, ref surface)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "no input Surface"); return; }
            //if (surface == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Surface can not be NULL"); return; }
            
            //if (!DA.GetData<LCA_Material>(1, ref material)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid material input"); return;}

            //double thickness = 0;

            //DA.GetData<double>(2, ref thickness);

            //if(thickness <= 0) {  AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Thickness less than or equal to 0"); return; }

            //thickness = LCA_HelperCalss.convertValueToMeters(thickness);
            //double surfaceArea  = LCA_HelperCalss.convertSquaredValueToMeters(AreaMassProperties.Compute(surface).Area);
            //if (surfaceArea == double.NaN || thickness == double.NaN) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Rhino units needs to be mm, cm or m"); return; }

            //double volume = surfaceArea *  thickness;


            //int expectedLifetime = -1;
            //DA.GetData<int>(3, ref expectedLifetime);
            //LCA_Element element = new LCA_Element(material, volume, expectedLifetime);

            //string _tempStr = string.Empty;
            //if (DA.GetData(4, ref _tempStr)) { element.Element_Name = _tempStr; }
            //if (DA.GetData(5, ref _tempStr)) { element.Element_Group = _tempStr; }

            //DA.SetData(0, element);
            //DA.SetData(1, element.Element_Volume);
            //DA.SetData(2, element.Element_Weight);
            //DA.SetData(3, element.Element_GWP);


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