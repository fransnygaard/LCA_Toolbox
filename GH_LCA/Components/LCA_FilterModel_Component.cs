﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;
using GH_LCA.Extentions;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GH_LCA.Components
{
    public class LCA_FilterModel_Component : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the FilterModel class.
        /// </summary>
        public LCA_FilterModel_Component()
 : base("LCA Filter Model", "LCA Filter Model",
              "Filter model to only contain a set of materials or elements.",
              Constants.PluginName, Constants.SubResults)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Model.Name, Constants.Model.NickName, "Model to filter", GH_ParamAccess.item); //0
            //inputParams.Add(Constants.Model.Name, IndexCounter);

            pManager.AddTextParameter(Constants.Mat_Name.Name, Constants.Mat_Name.NickName, "List of material names to keep", GH_ParamAccess.item);
            //inputParams.Add(Constants.Material.Name, IndexCounter);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddTextParameter(Constants.Mat_Category.Name, Constants.Mat_Category.NickName, "List of material categories to keep", GH_ParamAccess.item);
            //inputParams.Add(Constants.Material.Name, IndexCounter);
            pManager[pManager.ParamCount - 1].Optional = true;


            pManager.AddTextParameter(Constants.Element_Name.Name, Constants.Element_Name.NickName, "List of element names to keep", GH_ParamAccess.item); 
            //inputParams.Add(Constants.Element_Name.Name, IndexCounter);
            pManager[pManager.ParamCount - 1].Optional = true;


            pManager.AddTextParameter(Constants.Element_Group.Name, Constants.Element_Group.NickName, "List of element groups to keep", GH_ParamAccess.item);
            //inputParams.Add(Constants.Element_Group.Name, IndexCounter);
            pManager[pManager.ParamCount - 1].Optional = true;


            registrerInputParams(pManager);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddGenericParameter(Constants.Model.Name, Constants.Model.NickName,"Filterd model" , GH_ParamAccess.item);
            //outputParams.Add(Constants.Model.Name, IndexCounter);


            registrerOutputParams(pManager);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            LCA_Model model = null;
            string MaterialFilter = string.Empty;
            string Element_nameFilter = string.Empty;
            string Element_groupFilter = string.Empty;


            if (!DA.GetData<LCA_Model>(inputParams[Constants.Model.Name], ref model)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid model found !"); return; }

            if (DA.GetData(inputParams[Constants.Mat_Name.Name], ref MaterialFilter)) 
            {
                model.FiterDataTableByMaterialName(MaterialFilter); // FILTER OUT BY NAME
            }

            if (DA.GetData(inputParams[Constants.Element_Name.Name], ref Element_nameFilter))
            {
                model.FiterDataTableByElementName(Element_nameFilter); // FILTER OUT BY ELEMENT NAME
            }

            if (DA.GetData(inputParams[Constants.Element_Group.Name], ref Element_groupFilter))
            {
                model.FiterDataTableByElementGroup(Element_groupFilter); // FILTER OUT BY ELEMENT GROUP
            }



            //SET DATA
            DA.SetData(outputParams[Constants.Model.Name], model);

            //DA.SetDataList(1, model.ListUniqueMaterialNames());
            //DA.SetData(2, model.GetCollumnSum("Element_GWP"));
            //DA.SetDataList(3, model.GetCollumnSum_ListByMaterial("Element_GWP"));
            //DA.SetDataList(4, model.GetCollumnPercentage_ListByMaterial("Element_GWP", model.GetCollumnSum("Element_GWP")));
            //DA.SetData(5, model.GetCollumnSum("Element_Weight"));
            //DA.SetDataList(6, model.GetCollumnSum_ListByMaterial("Element_Weight"));
            //DA.SetDataList(7, model.GetCollumnPercentage_ListByMaterial("Element_Weight", model.GetCollumnSum("Element_Weight")));
            //DA.SetData(8, model.GetCollumnSum("Element_Volume"));
            //DA.SetDataList(9, model.GetCollumnSum_ListByMaterial("Element_Volume"));
            //DA.SetDataList(10, model.GetCollumnPercentage_ListByMaterial("Element_Volume", model.GetCollumnSum("Element_Volume")));


            ////A1-A3
            //DA.SetData(11, model.GetCollumnSum("Element_GWP"));

            ////A4
            //DA.SetData(12, model.GetCollumnSum("Element_A4"));

            ////B4
            //DA.SetData(13, model.GetCollumnSum("Element_B4"));


            ////C
            //DA.SetData(14, model.GetCollumnSum("Element_C"));

            ////D
            //DA.SetData(15, model.GetCollumnSum("Element_D"));







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
            get { return new Guid("ABFAEF77-5882-49EE-9595-D3EE9C5519FA"); }
        }
    }
}