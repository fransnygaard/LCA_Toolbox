using System;
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
            inputIndexCounter_Reset();

            pManager.AddGenericParameter(Constants.Model.Name, Constants.Model.NickName, "Model to filter", GH_ParamAccess.item); //0
            inputParams.Add(Constants.Model.Name, IndexCounter);

            pManager.AddTextParameter(Constants.Material.Name, Constants.Material.NickName, "List of material names to keep", GH_ParamAccess.item);
            inputParams.Add(Constants.Material.Name, IndexCounter);
            pManager[pManager.ParamCount - 1].Optional = true;


            pManager.AddTextParameter(Constants.Element_Name.Name, Constants.Element_Name.NickName, "List of element names to keep", GH_ParamAccess.item); 
            inputParams.Add(Constants.Element_Name.Name, IndexCounter);
            pManager[pManager.ParamCount - 1].Optional = true;


            pManager.AddTextParameter(Constants.Element_Group.Name, Constants.Element_Group.NickName, "List of element groups to keep", GH_ParamAccess.item);
            inputParams.Add(Constants.Element_Group.Name, IndexCounter);
            pManager[pManager.ParamCount - 1].Optional = true;


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "Model", "", GH_ParamAccess.item); //0
            pManager.AddTextParameter("List names by material", "List names by material", "", GH_ParamAccess.list); //1
            pManager.AddNumberParameter("SUM GWP[kg CO2eq]", "SUM GWP[kg CO2eq]", "", GH_ParamAccess.item); //2
            pManager.AddNumberParameter("List GWP [kg CO2eq] by material", "List GWP [kg CO2eq] by material", "", GH_ParamAccess.list); //3
            pManager.AddNumberParameter("List GWP [%] by material", "List GWP [%] by material", "", GH_ParamAccess.list); //4
            pManager.AddNumberParameter("SUM Weight [kg]", "SUM Weight [kg]", "", GH_ParamAccess.item); //5
            pManager.AddNumberParameter("List Weight [kg] by material", "List Weight [kg] by material", "", GH_ParamAccess.list); //6
            pManager.AddNumberParameter("List Weight [%] by material", "List Weight [%] by material", "", GH_ParamAccess.list); //7
            pManager.AddNumberParameter("SUM Volume [m3]", "SUM Volume [m3]", "", GH_ParamAccess.item); //8
            pManager.AddNumberParameter("List Volume [m3] by material", "List Volume [m3] by material", "", GH_ParamAccess.list); //9
            pManager.AddNumberParameter("List Volume [%] by material", "List Volume [%] by material", "", GH_ParamAccess.list); //10
            pManager.AddNumberParameter("Sum A1-A3 (Product stage) [kg CO2eq]", "Sum A1-A3 (Product stage) [kg CO2eq]", "", GH_ParamAccess.item); //11
            pManager.AddNumberParameter("Sum A4 (Transportation) [kg CO2eq]", "Sum A4 (Transportation) [kg CO2eq]", "", GH_ParamAccess.item); //12
            pManager.AddNumberParameter("Sum B4 (Replacement) [kg CO2eq]", "Sum A4 (Replacement) [kg CO2eq]", "", GH_ParamAccess.item); //13
            pManager.AddNumberParameter("Sum C [kg CO2eq]", "Sum C (End of life) [kg CO2eq]", "", GH_ParamAccess.item); //14
            pManager.AddNumberParameter("Sum D [kg CO2eq]", "Sum D (Beyond system bounds) [kg CO2eq]", "Benefits and loads beyond the system boundariies", GH_ParamAccess.item); //15



            pManager.AddTextParameter("DEBUG", "DEBUG", "", GH_ParamAccess.list); //16

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            LCA_Model model = new LCA_Model();
            string MaterialFilter = string.Empty; 

            if (!DA.GetData<LCA_Model>(inputParams[Constants.Model.Name], ref model)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid model found !"); return; }

            if (DA.GetData(inputParams[Constants.Material.Name], ref MaterialFilter)) 
            {
                model.FiterDataTableByMaterialName(MaterialFilter);
            }


            //SET DATA
            string debugLog = "Bla bla bla";
            //DEBUG
            DA.SetDataList(16, debugLog);


            DA.SetData(0, model);
            DA.SetDataList(1, model.ListUniqueMaterialNames());
            DA.SetData(2, model.GetCollumnSum("Element_GWP"));
            DA.SetDataList(3, model.GetCollumnSum_ListByMaterial("Element_GWP"));
            DA.SetDataList(4, model.GetCollumnPercentage_ListByMaterial("Element_GWP", model.GetCollumnSum("Element_GWP")));
            DA.SetData(5, model.GetCollumnSum("Element_Weight"));
            DA.SetDataList(6, model.GetCollumnSum_ListByMaterial("Element_Weight"));
            DA.SetDataList(7, model.GetCollumnPercentage_ListByMaterial("Element_Weight", model.GetCollumnSum("Element_Weight")));
            DA.SetData(8, model.GetCollumnSum("Element_Volume"));
            DA.SetDataList(9, model.GetCollumnSum_ListByMaterial("Element_Volume"));
            DA.SetDataList(10, model.GetCollumnPercentage_ListByMaterial("Element_Volume", model.GetCollumnSum("Element_Volume")));


            //A1-A3
            DA.SetData(11, model.GetCollumnSum("Element_GWP"));

            //A4
            DA.SetData(12, model.GetCollumnSum("Element_A4"));

            //B4
            DA.SetData(13, model.GetCollumnSum("Element_B4"));


            //C
            DA.SetData(14, model.GetCollumnSum("Element_C"));

            //D
            DA.SetData(15, model.GetCollumnSum("Element_D"));







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