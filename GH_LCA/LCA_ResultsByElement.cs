using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GH_LCA
{
    public class LCA_ResultsByElement : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FilterElements class.
        /// </summary>
        public LCA_ResultsByElement()
          : base("LCA Results By Element", "LCA Results By Element",
              "Description",
              "LCA", "04 Results")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "Model", "", GH_ParamAccess.list);
            pManager.AddTextParameter("ElementName filter", "ElementName filter", "", GH_ParamAccess.item);
            pManager.AddTextParameter("ElementGroup filter", "ElementGroup filter", "", GH_ParamAccess.item);



        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            LCA_Model model = null;

            if (!DA.GetData<LCA_Model>(0, ref model)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Model not valid"); return; }

            //foreach LCA_Element element in model.get



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
            get { return new Guid("80C83C6C-A3B3-4DFA-B2EC-938EB20BDEA3"); }
        }
    }
}