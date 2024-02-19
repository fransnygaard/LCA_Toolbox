using System;
using System.Collections.Generic;
using System.IO;
using GH_GeneralClassLibrary.UI;
using Grasshopper.Kernel;
using LCA_Toolbox.Database;
using Rhino.Geometry;

namespace LCA_Toolbox
{
    public class LCA_CustomDatabase_Component : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the LCA_CustomDatabase_Component class.
        /// </summary>
        public LCA_CustomDatabase_Component()
          : base("LAC: Custom material database Add/Modify", " DB ",
            "Description",
            Constants.PluginName, Constants.SubMaterials)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Material.Name, Constants.Material.NickName, Constants.Material.Discription, GH_ParamAccess.list);
            pManager.AddTextParameter("DB filePath","Path","",GH_ParamAccess.item);
            pManager.AddBooleanParameter("Allow Overwrite?", "Overwite?", "", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("WriteToDB", "Write", "", GH_ParamAccess.item, false);

            registrerInputParams(pManager);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "Status", "", GH_ParamAccess.list);

            registrerOutputParams(pManager);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {


            //Get input material
            List<LCA_Material> materials = new List<LCA_Material>();
            if (!DA.GetDataList<LCA_Material>(inputParams[Constants.Material.Name],  materials)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid material input"); return; }



            //Get path and set connection string.
            List<string> status = new List<string>();
            string path = string.Empty;
            DA.GetData<string>(inputParams["DB filePath"], ref path);
            
            //ADD CHECK IF PATH IS VALID!!
            SqliteDataAcces dataAcces = new SqliteDataAcces(path);


            //get overwrite and run bools
            bool overwrite = false;
            bool write = false;
            DA.GetData<bool>(inputParams["Allow Overwrite?"], ref overwrite);
            DA.GetData<bool>(inputParams["WriteToDB"], ref write);


            if(!write)
            { status.Add(@"Set 'Write to DB' to TRUE"); }


            if (write)
            {
                foreach (LCA_Material mat in materials)
                {
                    status.Add($"Material: {mat.Name} -> {dataAcces.AddToDB(mat, overwrite)}");
                }

            }

            status.Add(dataAcces.Get_DB_path());

            DA.SetDataList(outputParams["Status"], status);

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
            get { return new Guid("0B59CB64-0DB6-4CC6-812E-FB9D604A06C7"); }
        }
    }
}