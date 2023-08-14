using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GH_LCA
{
    public class LAC_AssembleModel_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the LAC_CalculateModel_Component class.
        /// </summary>
        public LAC_AssembleModel_Component()
          : base("LCA Assemble Model", "LCA Assemble Model",
              "Description",
              Constants.ShortName, Constants.SubModel)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements", "Elements", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Calculated building lifetime [years]", "Calculated building lifetime [years]", "", GH_ParamAccess.item, 60);//1

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
            List<string> debugLog = new List<string>();
            List<LCA_Element> elements = new List<LCA_Element>();

            if (!DA.GetDataList(0, elements)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Elements found !"); return; }
            if (elements[0] == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Elements found"); return; }


            int modelifetime = -1;
            DA.GetData<int>(1, ref modelifetime);

            LCA_Model model = new LCA_Model(elements, modelifetime);
            debugLog.Add(model.elementsDataTable.ToString());



            //SET DATA
            
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
            get { return new Guid("FC20670C-8155-4F77-B9C6-D44F504ABA0A"); }
        }
    }
}