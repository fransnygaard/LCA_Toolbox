using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using GH_LCA;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace LCA_Toolbox.Utilities
{
    public class Debug_OBSOLETE : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Debug class.
        /// </summary>
        public Debug_OBSOLETE()
          : base("LCA: Debug", "Nickname",
              "Description",
              Constants.PluginName, "Debug")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("D", "D", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> rtnList = new List<string>();

            rtnList.Add(Database.SqliteDataAcces.LoadConnectionStringPublic);



            DA.SetDataList(0, rtnList);


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
            get { return new Guid("840A27C4-B302-4AFC-94DF-C9276A78469B"); }
        }

        public override GH_Exposure Exposure 
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }
    }
}