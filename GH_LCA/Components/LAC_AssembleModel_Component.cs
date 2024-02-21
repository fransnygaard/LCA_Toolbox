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

            pManager.AddNumberParameter(Constants.B6_Year.Name, Constants.B6_Year.NickName, Constants.B6_Year.Discription, GH_ParamAccess.item,0);

            pManager.AddIntegerParameter(Constants.Lifetime.Name,Constants.Lifetime.NickName, Constants.Lifetime.Discription, GH_ParamAccess.item, -1);
            
            
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
            List<string> debugLog = new List<string>();



            List<LCA_Element> elements = new List<LCA_Element>();
            if (!DA.GetDataList(inputParams[Constants.Elements], elements)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Elements found !"); return; }
            if (elements[0] == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Elements found"); return; }


            double B6_perYear = 0;
            DA.GetData(inputParams[Constants.B6_Year], ref B6_perYear);


            int modelifetime = -1;
            DA.GetData<int>(inputParams[Constants.Lifetime], ref modelifetime);

            LCA_Model model = new LCA_Model(elements, modelifetime, B6_perYear);




            debugLog.Add(model.elementsDataTable.ToString());




            //SET DATA
            DA.SetData(outputParams[Constants.Model], model);

            DA.SetData(outputParams[Constants.GWP_TOTAL], model.GetGWPAllStages());



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