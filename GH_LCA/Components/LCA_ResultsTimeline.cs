using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_GeneralClassLibrary.UI;
using Rhino.Render;
using Eto.Forms;
using GH_GeneralClassLibrary.Utils;
using Grasshopper;
using Grasshopper.Kernel.Data;

namespace LCA_Toolbox.Components
{
    public class LCA_ResultsTimeline : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the LCA_ResultsTimeline class.
        /// </summary>
        public LCA_ResultsTimeline()
          : base("LCA: Result Timeline", "LCA: Result Timeline",
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
            //pManager.AddBooleanParameter(Constants.AllowSequestration.Name, Constants.AllowSequestration.NickName, Constants.AllowSequestration.Discription, GH_ParamAccess.item, false);

            registrerInputParams(pManager);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter(Constants.DataGridHeaders.Name, Constants.DataGridHeaders.NickName, Constants.DataGridHeaders.Discription, GH_ParamAccess.list);
            pManager.AddNumberParameter(Constants.ValueTree.Name, Constants.ValueTree.NickName, Constants.ValueTree.Discription, GH_ParamAccess.tree);




            registrerOutputParams(pManager);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //GET DATA
            LCA_Model model = null;

            if (!DA.GetData<LCA_Model>(inputParams[Constants.Model], ref model)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Model not valid"); return; }
            //DA.GetData<bool>(inputParams[Constants.AllowSequestration], ref model.AllowSequestration);


            List<string> DatagridHeaderList = new List<string> {"Year", "Embodied", "Operational", "Total Carbon" , "Cumulative Carbon" } ;

            List<double> Years = model.GetDataColumnFromTimeline("Year");
            List<double> Sum_Embodied = model.GetDataColumnFromTimeline("Sum_Embodied");
            List<double> Sum_Operational = model.GetDataColumnFromTimeline("Sum_Operational");
            List<double> Sum_Carbon = model.GetDataColumnFromTimeline("Sum_Carbon");
            List<double> Cumulative_Carbon = model.GetDataColumnFromTimeline("Cumulative_Carbon");

            //List<List<double>> ValueTreeNestedList = new List<List<double>>();
            DataTree<double> ValueTree = new DataTree<double>();
            for (int i = 0; i < Sum_Embodied.Count;i++)
            {
                ///Rhino.Collections.RhinoList<double> branch = new Rhino.Collections.RhinoList<double>();
                List<double> branch = new List<double>();
                branch.Add(Years[i]);
                branch.Add(Sum_Embodied[i]);
                branch.Add(Sum_Operational[i]);
                branch.Add(Sum_Carbon[i]);
                branch.Add(Cumulative_Carbon[i]);
                ValueTree.AddRange(branch,new GH_Path(i));

            }

            //SET DATA

            DA.SetDataList(outputParams[Constants.DataGridHeaders],DatagridHeaderList);
            DA.SetDataTree(outputParams[Constants.ValueTree], ValueTree);


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
            get { return new Guid("32CD091C-3977-48DF-BFC6-F300DC7DF84D"); }
        }
    }
}