using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Dynamic;
using System.Web;
using GH_GeneralClassLibrary.UI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace LCA_Toolbox
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
            pManager.AddGenericParameter(Constants.Material.Name, Constants.Material.NickName, Constants.Material.Discription, GH_ParamAccess.item); // 0

            pManager.AddTextParameter(Constants.Mat_Name.Name, Constants.Mat_Name.NickName, Constants.Mat_Name.Discription, GH_ParamAccess.item); //1

            pManager.AddTextParameter(Constants.Mat_Category.Name, Constants.Mat_Category.NickName, Constants.Mat_Category.Discription, GH_ParamAccess.item); //2

            pManager.AddTextParameter(Constants.Mat_Description.Name,Constants.Mat_Description.NickName, Constants.Mat_Description.Discription, GH_ParamAccess.item);//3

            pManager.AddNumberParameter(Constants.Density.Name, Constants.Density.NickName, Constants.Density.Discription, GH_ParamAccess.item); //4

            pManager.AddNumberParameter(Constants.Insulation.Name, Constants.Insulation.NickName, Constants.Insulation.Discription, GH_ParamAccess.item); //5

            pManager.AddNumberParameter(Constants.A1toA3_m3.Name, Constants.A1toA3_m3.NickName, Constants.A1toA3_m3.Discription, GH_ParamAccess.item);  //6

            pManager.AddNumberParameter(Constants.ODP.Name, Constants.ODP.NickName, Constants.ODP.Discription, GH_ParamAccess.item);  //7

            pManager.AddNumberParameter(Constants.POCP.Name, Constants.POCP.NickName, Constants.POCP.Discription, GH_ParamAccess.item);  

            pManager.AddNumberParameter(Constants.EP.Name, Constants.EP.NickName, Constants.EP.Discription, GH_ParamAccess.item);  

            pManager.AddNumberParameter(Constants.AP.Name, Constants.AP.NickName, Constants.AP.Discription, GH_ParamAccess.item);  

            pManager.AddNumberParameter(Constants.C1_C4.Name, Constants.C1_C4.NickName, Constants.C1_C4.Discription, GH_ParamAccess.item);

            pManager.AddTextParameter(Constants.DataSource.Name, Constants.DataSource.NickName, Constants.DataSource.Discription, GH_ParamAccess.item);

            pManager.AddTextParameter(Constants.Notes.Name, Constants.Notes.NickName, Constants.Notes.Discription, GH_ParamAccess.item);


            //Set all to optional.
            foreach (var p in Params.Input)
                p.Optional = true;

            registrerInputParams(pManager);


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)

        {


            pManager.AddGenericParameter(Constants.Material.Name, Constants.Material.NickName, Constants.Material.Discription, GH_ParamAccess.item); // 0

            pManager.AddTextParameter(Constants.Mat_Name.Name, Constants.Mat_Name.NickName, Constants.Mat_Name.Discription, GH_ParamAccess.item); //1

            pManager.AddTextParameter(Constants.Mat_Category.Name, Constants.Mat_Category.NickName, Constants.Mat_Category.Discription, GH_ParamAccess.item); //2

            pManager.AddTextParameter(Constants.Mat_Description.Name, Constants.Mat_Description.NickName, Constants.Mat_Description.Discription, GH_ParamAccess.item);//3

            pManager.AddNumberParameter(Constants.Density.Name, Constants.Density.NickName, Constants.Density.Discription, GH_ParamAccess.item); //4

            pManager.AddNumberParameter(Constants.Insulation.Name, Constants.Insulation.NickName, Constants.Insulation.Discription, GH_ParamAccess.item); //5

            pManager.AddNumberParameter(Constants.A1toA3_m3.Name, Constants.A1toA3_m3.NickName, Constants.A1toA3_m3.Discription, GH_ParamAccess.item);  //6

            pManager.AddNumberParameter(Constants.ODP.Name, Constants.ODP.NickName, Constants.ODP.Discription, GH_ParamAccess.item);  //7
            
            pManager.AddNumberParameter(Constants.POCP.Name, Constants.POCP.NickName, Constants.POCP.Discription, GH_ParamAccess.item);

            pManager.AddNumberParameter(Constants.EP.Name, Constants.EP.NickName, Constants.EP.Discription, GH_ParamAccess.item);

            pManager.AddNumberParameter(Constants.AP.Name, Constants.AP.NickName, Constants.AP.Discription, GH_ParamAccess.item);

            pManager.AddNumberParameter(Constants.C1_C4.Name, Constants.C1_C4.NickName, Constants.C1_C4.Discription, GH_ParamAccess.item);

            pManager.AddTextParameter(Constants.DataSource.Name, Constants.DataSource.NickName, Constants.DataSource.Discription, GH_ParamAccess.item);

            pManager.AddTextParameter(Constants.Notes.Name, Constants.Notes.NickName, Constants.Notes.Discription, GH_ParamAccess.item);



            registrerOutputParams(pManager);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

          

            IGH_Goo gooB = null;
            DA.GetData(inputParams[Constants.Material], ref gooB);


            
            LCA_Material material;
            if (!gooB.TryGetMaterial(out material)) return;
            

            string _tempSTR = "";
            double _tempNR = 0;

            if(DA.GetData(inputParams[Constants.Mat_Name], ref _tempSTR)) { material.Name = _tempSTR; }

            if (DA.GetData(inputParams[Constants.Mat_Category], ref _tempSTR)) { material.Category = _tempSTR; }

            if (DA.GetData(inputParams[Constants.Mat_Description], ref _tempSTR)) { material.Description = _tempSTR; }

            if (DA.GetData(inputParams[Constants.Density], ref _tempNR)) { material.Density = _tempNR; }

            if (DA.GetData(inputParams[Constants.Insulation], ref _tempNR)) { material.Insulation = _tempNR; }

            if (DA.GetData(inputParams[Constants.A1toA3_m3], ref _tempNR)) { material.A1toA3 = _tempNR; }

            if (DA.GetData(inputParams[Constants.ODP], ref _tempNR)) { material.ODP = _tempNR; }

            if (DA.GetData(inputParams[Constants.POCP], ref _tempNR)) { material.POCP = _tempNR; }

            if (DA.GetData(inputParams[Constants.EP], ref _tempNR)) { material.EP = _tempNR; }

            if (DA.GetData(inputParams[Constants.AP], ref _tempNR)) { material.AP = _tempNR; }

            if (DA.GetData(inputParams[Constants.C1_C4], ref _tempNR)) { material.C1toC4 = _tempNR; }

            if (DA.GetData(inputParams[Constants.DataSource], ref _tempSTR)) { material.DataSource = _tempSTR; }

            if (DA.GetData(inputParams[Constants.Notes], ref _tempSTR)) { material.Notes = _tempSTR; }


            if (material.Name == "NULL" || material.Name == null || material.Name == "") { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Name can not be NULL"); return; }
            if (material.Density <= 0 || material.Name == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"{Constants.Density} can not be negative or NULL"); return; }
            if (material.A1toA3 == double.NaN) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"{Constants.A1toA3_m3} must be a valid number."); return; }


            //SET OUTPUTS
            DA.SetData(outputParams[Constants.Material.Name], material);
            DA.SetData(outputParams[Constants.Mat_Name.Name], material.Name);
            DA.SetData(outputParams[Constants.Mat_Category.Name], material.Category);
            DA.SetData(outputParams[Constants.Mat_Description.Name], material.Description);
            DA.SetData(outputParams[Constants.Density.Name], material.Density);
            DA.SetData(outputParams[Constants.Insulation.Name], material.Insulation);
            DA.SetData(outputParams[Constants.A1toA3_m3.Name], material.A1toA3);
            DA.SetData(outputParams[Constants.ODP.Name], material.ODP);
            DA.SetData(outputParams[Constants.POCP], material.POCP);
            DA.SetData(outputParams[Constants.EP], material.EP);
            DA.SetData(outputParams[Constants.AP], material.AP);
            DA.SetData(outputParams[Constants.C1_C4], material.C1toC4);
            DA.SetData(outputParams[Constants.DataSource], material.DataSource);
            DA.SetData(outputParams[Constants.Notes], material.Notes);


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