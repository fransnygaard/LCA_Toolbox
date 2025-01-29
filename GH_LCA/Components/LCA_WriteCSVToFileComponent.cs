using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.IO;
using GH_GeneralClassLibrary.UI;

namespace LCA_Toolbox.Components
{
    public class LCA_WriteCSVToFileComponent : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the WriteTextToFileComponent class.
        /// </summary>
        public LCA_WriteCSVToFileComponent()
          :base("LCA: Write CSV to file", "LCA: Write CSV",
              "Description",
              Constants.PluginName, Constants.SubResults)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter(Constants.CSV.Name,Constants.CSV.NickName,Constants.CSV.Discription, GH_ParamAccess.list);
            
            pManager.AddTextParameter(Constants.Folder.Name, Constants.Folder.NickName, "Folder to write file to", GH_ParamAccess.item);
            pManager.AddTextParameter(Constants.FileName.Name, Constants.FileName.NickName,Constants.FileName.Discription, GH_ParamAccess.item);
            //pManager.AddTextParameter("FileType", "T", "Filetype / file ending", GH_ParamAccess.item, "gcode");
            pManager.AddBooleanParameter(Constants.RUN.Name, Constants.RUN.NickName, Constants.RUN.Discription, GH_ParamAccess.item, false);
            
            registrerInputParams(pManager);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter(Constants.Status.Name, Constants.Status.NickName, Constants.Status.Discription, GH_ParamAccess.list);
            registrerOutputParams(pManager);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string fullPath = "";
            string _folder = "";
            string _fileName = "";
           // string _fileType = "gcode";
            bool _run = false;



            List<string> linesToWrite = new List<string>();

            List<string> rtnStatus = new List<string>();

            //if (!DA.GetDataList(inputParams[Constants.Elements], input_elements))

            DA.GetDataList(inputParams[Constants.CSV], linesToWrite);
            DA.GetData(inputParams[Constants.Folder], ref _folder);
            DA.GetData(inputParams[Constants.FileName], ref _fileName);
            // DA.GetData(inputParams[Constants.], ref _fileType);
            DA.GetData(inputParams[Constants.RUN], ref _run);

            if(!_run) { return; }

            fullPath = _folder + _fileName; // + "." + _fileType;

            File.WriteAllLines(fullPath, linesToWrite.ToArray());

            rtnStatus.Add($"Written {linesToWrite.Count} lines to: {fullPath}");

            DA.SetDataList(outputParams["Status"], rtnStatus);

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
            get { return new Guid("efa34aef-21c2-4e77-af41-7720edc8a108"); }
        }
    }
}