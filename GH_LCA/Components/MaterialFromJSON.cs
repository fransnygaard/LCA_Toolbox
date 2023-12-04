using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_LCA.Extentions;
using Grasshopper.Kernel.Types;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using System.Diagnostics.Eventing.Reader;

namespace GH_LCA.Components
{
    public class MaterialFromJSON : GH_MyExtendableComponent
    {


        /// <summary>
        /// Initializes a new instance of the MaterialFromJSON class.
        /// </summary>
        public MaterialFromJSON()
          : base("Material From JSON", "JSON->Mat",
              "Turn JSON file containing materials into material for use in " + Constants.PluginName,
              Constants.PluginName, Constants.SubMaterials)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            inputIndexCounter_Reset();

            pManager.AddTextParameter(Constants.JSON.Name, Constants.JSON.NickName, Constants.JSON.Discription, GH_ParamAccess.item); // 0
            inputParams.Add(Constants.JSON.Name, IndexCounter);
            Params.Input[inputParams[Constants.JSON.Name]].Optional = true;


            pManager.AddTextParameter(Constants.FilePath.Name, Constants.FilePath.NickName, Constants.FilePath.Discription, GH_ParamAccess.item); // 0
            inputParams.Add(Constants.FilePath.Name, IndexCounter);
            Params.Input[inputParams[Constants.FilePath.Name]].Optional = true;


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            
            outputIndexCounter_Reset();

            pManager.AddGenericParameter(Constants.Material.Name, Constants.Material.NickName, Constants.Material.Discription, GH_ParamAccess.list); // 0
            outputParams.Add(Constants.Material.Name, IndexCounter);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            string filePath = string.Empty;
            string jsonString = string.Empty;


            if (!DA.GetData<string>(inputParams[Constants.FilePath.Name], ref filePath)) 
            {
                if (!DA.GetData<string>(inputParams[Constants.JSON.Name], ref jsonString)) return;
            }
            else
            {
                jsonString = File.ReadAllText(filePath);
            }
           
            

            List<LCA_Material> materialList = JsonConvert.DeserializeObject<List<LCA_Material>>(jsonString);


            DA.SetDataList(outputParams[Constants.Material.Name], materialList);


           // DA.SetData(outputParams[Constants.JOSN.Name], serialized);


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
            get { return new Guid("1AEBE2AC-18C3-4B5D-A4EE-0189460FC97F"); }
        }
    }
}