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
    public class PermutationSetFromTwoLists_Component : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the Permutations_List_List class.
        /// </summary>
        public PermutationSetFromTwoLists_Component()
          : base("Permutation-set from two lists", "Permutations_2Lists",
            "Creates a set of permutations based on two input list.\n" +
                "   -Output data-tree is based on List 0 \n" +
                "   -The order in the output are {L0;L1}",
            Constants.PluginName, Constants.Tools)
        {
        }
        //public override GH_Exposure Exposure
        //{
        //    get { return GH_Exposure.hidden; }
        //}

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("List0", "L0", "" , GH_ParamAccess.list);
            pManager.AddGenericParameter("List1", "L1", "" , GH_ParamAccess.list);

            registrerInputParams(pManager);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Permutation tree", "P", "", GH_ParamAccess.tree);

            registrerOutputParams(pManager);


        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {


            //INPUTS 
            List<IGH_Goo> list0 = new List<IGH_Goo>();
            DA.GetDataList(inputParams["List0"], list0);


            List<IGH_Goo> list1 = new List<IGH_Goo>();
            DA.GetDataList(inputParams["List1"], list1);




            //USE THE PermutationsFromTwoLists() func from library.
            //var rtnList = Permutations.PermutationsFromTwoLists(list0, list1);

            var rtnList =
            from vo in list0
            from v1 in list1
            select new[] { vo, v1 };
            //return rtnList;

            //OUTPUTS
            DataTree<IGH_Goo> rtnTree = rtnList.ToDataTree<IGH_Goo>();

            DA.SetDataTree(outputParams["Permutation tree"], rtnTree);


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
            get { return new Guid("7626F69B-6CB5-42F3-862A-AB25C7C8FCFD"); }
        }
    }
}