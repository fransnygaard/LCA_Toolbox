using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using GH_GeneralClassLibrary.DataStructures;
using GH_GeneralClassLibrary.Utils;
using GH_LCA;

namespace ART_MACHINE.Components
{
    public class QuadTreeComponent_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the QuadTreeComponent class.
        /// </summary>
        public QuadTreeComponent_Component()
          : base("QuadTree", "QT",
              "Description",
               Constants.PluginName, Constants.Tools)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddRectangleParameter("Rect", "R", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Point3d> points = new List<Point3d>();
            if (!DA.GetDataList<Point3d>(0, points)) { return; }

            var boundingRectangle = new Rhino.Geometry.BoundingBox(points).ToRectangle3D();

            QuadTree qt = new QuadTree(20, boundingRectangle);
           // if (!qt.InsertPoints(points)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more points not added too quadtree"); };

            IEnumerable<QT_Cell> AllCells = qt.rootCell.TraverseNested(node => node.subCells);
            var bounds = QT_ClassExtentions.GetAllBoundaries(AllCells);




            DA.SetDataList(0, bounds);

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
            get { return new Guid("03E87FEE-31C2-4822-9E9F-182304F3CFD2"); }
        }
    }
}