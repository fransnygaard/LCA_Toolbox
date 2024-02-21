using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_GeneralClassLibrary.UI;

namespace LCA_Toolbox
{
    public class LCA_DetailedResult_Component : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the LCA_DetailedResult_Component class.
        /// </summary>
        public LCA_DetailedResult_Component()
          : base("LCA: Detailed Result", "LCA: Detailed Result",
              "Description",
              Constants.PluginName, Constants.SubResults)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Model.Name, Constants.Model.NickName,Constants.Model.Discription, GH_ParamAccess.item);

            //Allow Carbon sequestration
            pManager.AddBooleanParameter(Constants.AllowSequestration.Name, Constants.AllowSequestration.NickName, Constants.AllowSequestration.Discription, GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

            registrerInputParams(pManager);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

            

            pManager.AddGenericParameter(Constants.Model.Name, Constants.Model.NickName, Constants.Model.Discription, GH_ParamAccess.item);

            pManager.AddTextParameter("List names by material", "List names by material", "", GH_ParamAccess.list); //1
            pManager.AddNumberParameter(Constants.A1toA3_m3.Name, "SUM GWP[kg CO2eq]", "", GH_ParamAccess.item); //2
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


            registrerOutputParams(pManager);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            LCA_Model model = null;

            if (!DA.GetData<LCA_Model>(inputParams[Constants.Model], ref model)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Model not valid"); return; }

            //foreach LCA_Element element in model.get
            bool allowSeq = false;
            DA.GetData<bool>(inputParams[Constants.AllowSequestration], ref allowSeq);

            model.AllowSequestration = allowSeq;

       



      //SET DATA

            //DEBUG
            //DA.SetDataList(16, debugLog);


            DA.SetData(outputParams[Constants.Model.Name], model);

            DA.SetDataList(1, model.ListUniqueMaterialNames());
            DA.SetData(2, model.GetColumnSum("Element_A1toA3"));
            DA.SetDataList(3, model.GetColumnSum_ListByMaterial("Element_A1toA3"));
            DA.SetDataList(4, model.GetColumnPercentage_ListByMaterial("Element_A1toA3"));
            DA.SetData(5, model.GetColumnSum("Element_Weight"));
            DA.SetDataList(6, model.GetColumnSum_ListByMaterial("Element_Weight"));
            DA.SetDataList(7, model.GetColumnPercentage_ListByMaterial("Element_Weight"));
            DA.SetData(8, model.GetColumnSum("Element_Volume"));
            DA.SetDataList(9, model.GetColumnSum_ListByMaterial("Element_Volume"));
            DA.SetDataList(10, model.GetColumnPercentage_ListByMaterial("Element_Volume"));


            //A1-A3
            DA.SetData(11, model.GetColumnSum("Element_A1toA3"));

            //A4
            DA.SetData(12, model.GetColumnSum("Element_A4"));

            //B4
            DA.SetData(13, model.GetColumnSum("Element_B4_Sum"));


            //C
            //DA.SetData(14, model.GetColumnSum("Element_C"));

            //D
           // DA.SetData(15, model.GetColumnSum("Element_D"));





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
            get { return new Guid("80C83C6C-A3B3-4DFA-B2EC-938EB20BDEA3"); }
        }
    }
}