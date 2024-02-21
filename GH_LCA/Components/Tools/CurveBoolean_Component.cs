using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;

using GH_GeneralClassLibrary.Utils;
using GH_GeneralClassLibrary.UI;

namespace LCA_Toolbox
{
    public class CurveBoolean_Component : GH_MyExtendableComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public CurveBoolean_Component()
          : base("CurveBoolean", "CurveBoolean",
            "CurveBoolean",
            Constants.PluginName, Constants.Tools)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //Input curves
            pManager.AddCurveParameter(Constants.Curves.Name, Constants.Curves.NickName, Constants.Curves.Discription, GH_ParamAccess.list);
            
            //Input points (optional)
            pManager.AddPointParameter(Constants.Points.Name, Constants.Points.NickName, "Input points for boolean regions. If input is left empty, outside region is calculated", GH_ParamAccess.list);
            pManager[pManager.ParamCount - 1].Optional = true;

            //Input plane (default to World XY)
            pManager.AddPlaneParameter(Constants.Plane.Name, Constants.Plane.NickName, "Input plane.", GH_ParamAccess.item,Plane.WorldXY);

            pManager.AddBooleanParameter("combineRegions", "combineRegions", "", GH_ParamAccess.item,true);

            pManager.AddNumberParameter("tolerance", "tolerance", "", GH_ParamAccess.item, DocumentTolerance());





            registrerInputParams(pManager);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddCurveParameter(Constants.Curves.Name, Constants.Curves.NickName, Constants.Curves.Discription, GH_ParamAccess.list);

            registrerOutputParams(pManager);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            bool solveOutside = false;

            //GET input curves
            List<Curve> inputCurves = new List<Curve>();
            DA.GetDataList<Curve>(inputParams[Constants.Curves.Name], inputCurves);

            //GET input points, if no points set solveOutside = true
            List<Point3d> inputPoints = new List<Point3d>();
            if (!DA.GetDataList<Point3d>(inputParams[Constants.Points.Name], inputPoints)) solveOutside = true;

            Plane inputPlane = new Plane();
            //GET input plane.
            DA.GetData(inputParams[Constants.Plane.Name], ref inputPlane);

            //INPUT bool combine regions
            bool combineRegions = true;
            DA.GetData<bool>(inputParams["combineRegions"], ref combineRegions);


            //INPUT tolerance 
            double tolerance = DocumentTolerance();
            DA.GetData<double>(inputParams["tolerance"], ref tolerance);



            //If solveOutside = true
            //      Make bounding box around curves, offset box and add point to inputPoints that is located between original and offset bounding. 

            CurveBooleanRegions regions = default;

            //SOLVE for booleanRegions with no points inputs
            if (!solveOutside) //first try to solve for point inside,  If no points in input solveOutside  = true 
            {
                regions = Curve.CreateBooleanRegions(inputCurves, inputPlane,inputPoints, combineRegions,tolerance);
            }

            if(regions.RegionCount == 0) // if no regions found solve outside. 
            {
                regions = Curve.CreateBooleanRegions(inputCurves, inputPlane, true, tolerance);
            }


            //Extract regionCurves
            List<Curve> outputCurves = new List<Curve>();
            for (int i = 0; i < regions.RegionCount; i++)
            {
                outputCurves.Add(regions.RegionCurves(i)[0]);
            }

            //OUTPUT result
            DA.SetDataList(outputParams[Constants.Curves.Name], outputCurves);
            

            //If solveOutside = true , remove the bounding box curve from list before setting outputs


            



        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("91886650-d1ff-4d77-ac0f-1429ca1a198b");
    }
}