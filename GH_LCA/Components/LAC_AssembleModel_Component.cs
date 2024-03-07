using System;
using System.Collections.Generic;
using GH_GeneralClassLibrary.UI;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace LCA_Toolbox
{
    public class LAC_AssembleModel_Component : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the LAC_CalculateModel_Component class.
        /// </summary>
        public LAC_AssembleModel_Component()
          : base("LCA Assemble Model", "LCA Assemble Model",
              "Description",
              Constants.PluginName, Constants.SubModel)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Elements.Name, Constants.Elements.NickName, Constants.Elements.Discription, GH_ParamAccess.list);

            pManager.AddNumberParameter(Constants.B6_Year.Name, Constants.B6_Year.NickName, Constants.B6_Year.Discription, GH_ParamAccess.list,0);

            pManager.AddIntegerParameter(Constants.Lifetime.Name,Constants.Lifetime.NickName, Constants.Lifetime.Discription, GH_ParamAccess.item, 60);

            pManager.AddBooleanParameter(Constants.AllowSequestration.Name, Constants.AllowSequestration.NickName, Constants.AllowSequestration.Discription, GH_ParamAccess.item, false);

            registrerInputParams(pManager);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddGenericParameter(Constants.Model.Name, Constants.Model.NickName,Constants.Model.Discription, GH_ParamAccess.item);
            pManager.AddNumberParameter(Constants.GWP_TOTAL.Name, Constants.GWP_TOTAL.NickName, Constants.GWP_TOTAL.Discription, GH_ParamAccess.item);

            registrerOutputParams(pManager);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<LCA_Element> input_elements = new List<LCA_Element>();
            if (!DA.GetDataList(inputParams[Constants.Elements], input_elements)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Elements found !"); return; }
            if (input_elements[0] == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Elements found"); return; }


            List<double> input_B6_perYear = new List<double>();
            DA.GetDataList(inputParams[Constants.B6_Year], input_B6_perYear);


            int input_modelifetime = -1;
            DA.GetData<int>(inputParams[Constants.Lifetime], ref input_modelifetime);
            if(input_modelifetime <= 0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"{Constants.Lifetime.Name} has to be larger than 0"); return; }


            bool _AllowSequestration = false;
            DA.GetData<bool>(inputParams[Constants.AllowSequestration], ref _AllowSequestration);


            LCA_Model model = new LCA_Model(input_elements, input_modelifetime, input_B6_perYear, _AllowSequestration);




            //SET DATA
            DA.SetData(outputParams[Constants.Model], model);
            DA.SetData(outputParams[Constants.GWP_TOTAL], model.GetTotalGWP_SumModel());



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
            get { return new Guid("FC20670C-8155-4F77-B9C6-D44F504ABA0A"); }
        }
    }
}