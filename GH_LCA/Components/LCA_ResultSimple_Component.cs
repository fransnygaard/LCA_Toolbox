using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_GeneralClassLibrary.UI;

namespace LCA_Toolbox.Components
{
    public class LCA_ResultSimple_Component : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the LCA_ResultSimple class.
        /// </summary>
        public LCA_ResultSimple_Component()
          : base("LCA: Result Simple", "LCA: Result Simple",
              "Description",
              Constants.PluginName, Constants.SubResults)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //Model input
            pManager.AddGenericParameter(Constants.Model.Name, Constants.Model.NickName, Constants.Model.Discription, GH_ParamAccess.item);
            
            //Allow Carbon sequestration
            pManager.AddBooleanParameter(Constants.AllowSequestration.Name, Constants.AllowSequestration.NickName, Constants.AllowSequestration.Discription, GH_ParamAccess.item,false);

            registrerInputParams(pManager);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter(Constants.Embodied_carbon.Name, Constants.Embodied_carbon.NickName, Constants.Embodied_carbon.Discription, GH_ParamAccess.item);
            pManager.AddNumberParameter(Constants.Operational_carbon.Name, Constants.Operational_carbon.NickName, Constants.Operational_carbon.Discription, GH_ParamAccess.item);
            pManager.AddNumberParameter(Constants.GWP_TOTAL.Name, Constants.GWP_TOTAL.NickName, Constants.GWP_TOTAL.Discription, GH_ParamAccess.item);

            registrerOutputParams(pManager);   

           
            
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            LCA_Model model = null;

            if (!DA.GetData<LCA_Model>(inputParams[Constants.Model], ref model)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Model not valid"); return; }


            DA.GetData<bool>(inputParams[Constants.AllowSequestration], ref model.AllowSequestration);

            DA.SetData(outputParams[Constants.Embodied_carbon], model.GetEmbodied_carbon());
            DA.SetData(outputParams[Constants.Operational_carbon], model.GetOperational_carbon());
            DA.SetData(outputParams[Constants.GWP_TOTAL], model.GetGWP_total());




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
            get { return new Guid("1B322474-9102-411E-BB67-EB3615C36B39"); }
        }
    }
}