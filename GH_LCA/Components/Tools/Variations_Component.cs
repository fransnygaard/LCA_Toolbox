using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using GH_GeneralClassLibrary.Utils;
using GH_GeneralClassLibrary.UI;
using Grasshopper.Kernel.Types;

namespace LCA_Toolbox
{
    public class Variations_Component : GH_MyExtendableComponent
    {
        
       

        /// <summary>
        /// Initializes a new instance of the DeconstrucPermutationSet class.
        /// </summary>
        public Variations_Component()
          : base("Variations", "Variations",
              "Description", Constants.PluginName, Constants.Tools)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Input[]", "in", "", GH_ParamAccess.list);

            pManager.AddIntegerParameter("Length", "L", "", GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddBooleanParameter("Alow repetitions", "R", "", GH_ParamAccess.item,false);

            registrerInputParams(pManager);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Variations[[]]", "V", "", GH_ParamAccess.tree);
            registrerOutputParams(pManager);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<IGH_Goo> inputList = new List<IGH_Goo>();
            DA.GetDataList(inputParams["Input[]"], inputList);

            int length = -1;
            if (!DA.GetData<int>(inputParams["Length"], ref length)) length = inputList.Count;
            length = Math.Max(1, length);

            bool allowRepetitions = true;
            DA.GetData<bool>(inputParams["Alow repetitions"], ref allowRepetitions);



            var rtnList = allowRepetitions ?
                Permutations.GetVariationsWithRepetitions(inputList, length)
                :
                Permutations.GetVariations(inputList, length);

            var rtnDataTree = new DataTree<IGH_Goo>(rtnList.ToDataTree());

            DA.SetDataTree(outputParams["Variations[[]]"], rtnDataTree);



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
            get { return new Guid("06B9AED4-8C17-4F7A-B914-7115AA989CE0"); }
        }
    }
}