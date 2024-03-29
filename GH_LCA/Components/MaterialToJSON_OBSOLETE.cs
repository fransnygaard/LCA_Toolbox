﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_GeneralClassLibrary.UI;
using Grasshopper.Kernel.Types;
using Newtonsoft.Json;

namespace LCA_Toolbox.Components
{
    public class MaterialToJSON_OBSOLETE : GH_MyExtendableComponent
    {
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }
        /// <summary>
        /// Initializes a new instance of the MateriaToJSON class.
        /// </summary>
        public MaterialToJSON_OBSOLETE()
          : base("Material To JSON", "Mat->JSON",
              "Turn your material into JSON format to start building your own custom material database.",
              Constants.PluginName, Constants.SubMaterials)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            inputIndexCounter_Reset();

            pManager.AddGenericParameter(Constants.Material.Name, Constants.Material.NickName, Constants.Material.Discription, GH_ParamAccess.list); // 0
            inputParams.Add(Constants.Material.Name, IndexCounter);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            outputIndexCounter_Reset();

            pManager.AddTextParameter(Constants.JSON.Name, Constants.JSON.NickName,Constants.JSON.Discription, GH_ParamAccess.item); // 0
            outputParams.Add(Constants.JSON.Name, IndexCounter);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            LCA_Material_List material_list = new LCA_Material_List();

            if (!DA.GetDataList<LCA_Material>(inputParams[Constants.Material.Name], material_list.list)) return;
            if (material_list.list[0] == null) return;

        
            string serialized = JsonConvert.SerializeObject(material_list.list);
            DA.SetData(outputParams[Constants.JSON.Name], serialized);

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
            get { return new Guid("924352dd-8677-480d-9071-307e2c9d4e6a"); }
        }
    }
}