using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Dynamic;
using System.Web;
using GH_GeneralClassLibrary.UI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace GH_LCA
{
    public class LCA_Material_Component : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the LCA_CustomMaterial_Component class.
        /// </summary>
        public LCA_Material_Component()
          : base("LCA Material", "LCA Material",
              "Construct / Deconstruct / modify material",
              Constants.PluginName,Constants.SubMaterials)
        {
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            inputIndexCounter_Reset();

            pManager.AddGenericParameter(Constants.Material.Name, Constants.Material.NickName, Constants.Material.Discription, GH_ParamAccess.item); // 0
            inputParams.Add(Constants.Material.Name, IndexCounter);


            pManager.AddTextParameter(Constants.Mat_Name.Name, Constants.Mat_Name.NickName, Constants.Mat_Name.Discription, GH_ParamAccess.item); //1
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddTextParameter(Constants.Mat_Category.Name, Constants.Mat_Category.NickName, Constants.Mat_Category.Discription, GH_ParamAccess.item); //2
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddTextParameter(Constants.Mat_Description.Name,Constants.Mat_Description.NickName, Constants.Mat_Description.Discription, GH_ParamAccess.item);//3
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddNumberParameter(Constants.Density.Name, Constants.Density.NickName, Constants.Density.Discription, GH_ParamAccess.item); //4
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddNumberParameter(Constants.Insulation.Name, Constants.Insulation.NickName, Constants.Insulation.Discription, GH_ParamAccess.item); //5
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddNumberParameter(Constants.GWP_A1_A3.Name, Constants.GWP_A1_A3.NickName, Constants.GWP_A1_A3.Discription, GH_ParamAccess.item);  //6
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddNumberParameter(Constants.ODP.Name, Constants.ODP.NickName, Constants.ODP.Discription, GH_ParamAccess.item);  //7
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddNumberParameter("POCP", "POCP", "", GH_ParamAccess.item);  //8
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddNumberParameter("EP", "EP", "", GH_ParamAccess.item);  //9
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddNumberParameter("AP", "AP", "", GH_ParamAccess.item);  //10
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddNumberParameter("A4 [CO2eq/kg]", "A4 [CO2eq/kg]", "", GH_ParamAccess.item); //11
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddNumberParameter("C [CO2eq/kg]", "C [CO2eq/kg]", "", GH_ParamAccess.item); //12
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddNumberParameter("D [CO2eq/kg]", "D [CO2eq/kg]", "", GH_ParamAccess.item); //13
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddTextParameter("DataSource", "DataSource", "", GH_ParamAccess.item); //14
            inputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            //Set all to optional.
            foreach (var p in Params.Input)
                p.Optional = true;


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)

        {
            outputIndexCounter_Reset();


            pManager.AddGenericParameter(Constants.Material.Name, Constants.Material.NickName, Constants.Material.Discription, GH_ParamAccess.item); // 0
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddTextParameter(Constants.Mat_Name.Name, Constants.Mat_Name.NickName, Constants.Mat_Name.Discription, GH_ParamAccess.item); //1
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddTextParameter(Constants.Mat_Category.Name, Constants.Mat_Category.NickName, Constants.Mat_Category.Discription, GH_ParamAccess.item); //2
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddTextParameter(Constants.Mat_Description.Name, Constants.Mat_Description.NickName, Constants.Mat_Description.Discription, GH_ParamAccess.item);//3
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddNumberParameter(Constants.Density.Name, Constants.Density.NickName, Constants.Density.Discription, GH_ParamAccess.item); //4
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddNumberParameter(Constants.Insulation.Name, Constants.Insulation.NickName, Constants.Insulation.Discription, GH_ParamAccess.item); //5
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddNumberParameter(Constants.GWP_A1_A3.Name, Constants.GWP_A1_A3.NickName, Constants.GWP_A1_A3.Discription, GH_ParamAccess.item);  //6
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddNumberParameter(Constants.ODP.Name, Constants.ODP.NickName, Constants.ODP.Discription, GH_ParamAccess.item);  //7
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddNumberParameter("POCP", "POCP", "", GH_ParamAccess.item);  //8
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddNumberParameter("EP", "EP", "", GH_ParamAccess.item);  //9
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddNumberParameter("AP", "AP", "", GH_ParamAccess.item);  //10
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddNumberParameter("A4 [CO2eq/kg]", "A4 [CO2eq/kg]", "", GH_ParamAccess.item); //11
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddNumberParameter("C [CO2eq/kg]", "C [CO2eq/kg]", "", GH_ParamAccess.item); //12
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddNumberParameter("D [CO2eq/kg]", "D [CO2eq/kg]", "", GH_ParamAccess.item); //13
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);


            pManager.AddTextParameter("DataSource", "DataSource", "", GH_ParamAccess.item); //14
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

            pManager.AddTextParameter("MaterialGUID", "MaterialGUID", "used for debugging", GH_ParamAccess.item); //15
            outputParams.Add(pManager[pManager.ParamCount-1].Name, IndexCounter);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

          

            IGH_Goo gooB = null;
            DA.GetData(inputParams[Constants.Material.Name], ref gooB);


            
            LCA_Material material;
            if (!gooB.TryGetMaterial(out material)) return;
            

            string _tempSTR = "";
            double _tempNR = 0;

            if(DA.GetData(inputParams[Constants.Mat_Name.Name], ref _tempSTR)) { material.Name = _tempSTR; }

            if (DA.GetData(inputParams[Constants.Mat_Category], ref _tempSTR)) { material.Category = _tempSTR; }

            if (DA.GetData(inputParams[Constants.Mat_Description.Name], ref _tempSTR)) { material.Description = _tempSTR; }

            if (DA.GetData(inputParams[Constants.Density.Name], ref _tempNR)) { material.Density = _tempNR; }

            if (DA.GetData(inputParams[Constants.Insulation.Name], ref _tempNR)) { material.Insulation = _tempNR; }

            if (DA.GetData(inputParams[Constants.GWP_A1_A3.Name], ref _tempNR)) { material.A1toA3 = _tempNR; }

            if (DA.GetData(inputParams[Constants.ODP.Name], ref _tempNR)) { material.ODP = _tempNR; }

            if (DA.GetData(8, ref _tempNR)) { material.POCP = _tempNR; }

            if (DA.GetData(9, ref _tempNR)) { material.EP = _tempNR; }

            if (DA.GetData(10, ref _tempNR)) { material.AP = _tempNR; }

            if (DA.GetData(11, ref _tempNR)) { material.A4 = _tempNR; }

            if (DA.GetData(12, ref _tempNR)) { material.C1toC4 = _tempNR; }

            if (DA.GetData(13, ref _tempNR)) { material.D = _tempNR; }

            if (DA.GetData(14, ref _tempSTR)) { material.DataSource = _tempSTR; }


            if (material.Name == "NULL" || material.Name == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Name can not be NULL"); return; }
            if (material.Density <= 0 || material.Name == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "density can not be negative or null"); return; }
            if (material.A1toA3 == double.NaN) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "GWP must be a valid number."); return; }


            //SET OUTPUTS
            DA.SetData(outputParams[Constants.Material.Name], material);
            DA.SetData(outputParams[Constants.Mat_Name.Name], material.Name);
            DA.SetData(outputParams[Constants.Mat_Category.Name], material.Category);
            DA.SetData(outputParams[Constants.Mat_Description.Name], material.Description);
            DA.SetData(outputParams[Constants.Density.Name], material.Density);
            DA.SetData(outputParams[Constants.Insulation.Name], material.Insulation);
            DA.SetData(outputParams[Constants.GWP_A1_A3.Name], material.A1toA3);
            DA.SetData(outputParams[Constants.ODP.Name], material.ODP);





            //DA.SetData(0, material);

            //DA.SetData(1, material.Name);
            //DA.SetData(2, material.Category);
            //DA.SetData(3, material.Description);
            //DA.SetData(4, material.Density);
            //DA.SetData(5, material.Insulation);

            //DA.SetData(6, material.GWP);
            //DA.SetData(7, material.ODP);
            DA.SetData(8, material.POCP);
            DA.SetData(9, material.EP);
            DA.SetData(10, material.AP);
            DA.SetData(11, material.A4);
            DA.SetData(12, material.C1toC4);
            DA.SetData(13, material.D);
            DA.SetData(14, material.DataSource);

            DA.SetData(15, material.MaterialGUID);

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
            get { return new Guid("201310B1-29C8-47E5-9773-BDC02EDFBE3B"); }
        }
    }
}