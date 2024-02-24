using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace LCA_Toolbox
{
    public class LCA_DeconstructElement_Component_OBSOLETE : GH_Component
    {
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }
        /// <summary>
        /// Initializes a new instance of the LCA_DeconstructElement_Component class.
        /// </summary>
        public LCA_DeconstructElement_Component_OBSOLETE()
          : base("LCA: Deconstruct Element", "LCA: Deconstruct Element",
              "Description",
              Constants.PluginName, Constants.SubElements)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Element","Element","",GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //0
            pManager.AddGenericParameter("Material", "Material", "", GH_ParamAccess.item);
            //1
            pManager.AddTextParameter("Element Name", "Element Name", "", GH_ParamAccess.item);
            //2
            pManager.AddTextParameter("Element Group", "Element Group", "", GH_ParamAccess.item);
            //3
            pManager.AddTextParameter("ElementGUID", "ElementGUID", "", GH_ParamAccess.item);
            
            //4
            pManager.AddIntegerParameter("Element_ExpectedLifetime", "Element_ExpectedLifetime", "", GH_ParamAccess.item);
            
            //5
            pManager.AddNumberParameter(Constants.A1toA3_m3.Name, Constants.A1toA3_m3.NickName, Constants.A1toA3_m3.Discription, GH_ParamAccess.item);
            
            //6
            pManager.AddNumberParameter("Element_ODP", "Element_ODP", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Element_POCP", "Element_POCP", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Element_EP", "Element_EP", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Element_AP", "Element_AP", "", GH_ParamAccess.item);
            
            //10
            pManager.AddNumberParameter("Element_A4_Cost", "Element_A4_Cost", "", GH_ParamAccess.item);
            
            //11
            pManager.AddNumberParameter("Element_Weight", "Element_Weight", "", GH_ParamAccess.item);
            
            //12
            pManager.AddIntegerParameter("Element_B4_Nreplacements", "Element_B4_Nreplacements", "", GH_ParamAccess.item);
            
            //13
            pManager.AddNumberParameter("Element_B4_Cost", "Element_B4_Cost", "", GH_ParamAccess.item);
            
            //14
            pManager.AddNumberParameter("Element_C_Cost", "Element_C_Cost", "", GH_ParamAccess.item);


            //15
            pManager.AddNumberParameter("Element_D_Cost", "Element_D_Cost", "", GH_ParamAccess.item);








        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            LCA_Element element = null;
            if (!DA.GetData<LCA_Element>(0, ref element)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "invalid element"); return; }

            DA.SetData(0, element.Material);
            DA.SetData(1, element.Element_Name);
            DA.SetData(2, element.Element_Group);
           // DA.SetData(3, element.ElementGUID);
            DA.SetData(4, element.Element_Lifetime);
            DA.SetData(5, element.Element_A1toA3);
            DA.SetData(6, element.Element_ODP);
            DA.SetData(7, element.Element_POCP);
            DA.SetData(8, element.Element_EP);
            DA.SetData(9, element.Element_AP);
            DA.SetData(10, element.Element_A4);
            DA.SetData(11, element.Element_Weight);
            DA.SetData(12, element.Element_B4_Nreplacements);
            DA.SetData(13, element.Element_B4_Sum);
            DA.SetData(14, element.Element_C1toC4_perTime);
            DA.SetData(15, element.Element_D_ReusePercent);




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
            get { return new Guid("421724E7-64D0-4127-8ECE-825B338FFD36"); }
        }
    }
}