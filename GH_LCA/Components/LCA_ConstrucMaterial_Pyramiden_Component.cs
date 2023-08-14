using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;


namespace GH_LCA
{
    public class LCA_ConstrucMaterial_Pyramiden_Component : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public LCA_ConstrucMaterial_Pyramiden_Component()
          : base("LAC: Construct material from Materialpyramiden.dk", "LCA Construct material from Materialpyramiden.dk",
            "Description",
            Constants.ShortName,Constants.SubMaterials)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
           // pManager.AddTextParameter("Material group", "Material group", "", GH_ParamAccess.item, "Materialgroup");
            pManager.AddTextParameter("Material name", "Material name", "", GH_ParamAccess.item, "");//0
            pManager.AddNumberParameter("A4 [CO2eq/kg]", "A4 [CO2eq/kg]", "", GH_ParamAccess.item); //1
            pManager.AddNumberParameter("C [CO2eq/kg]", "C [CO2eq/kg]", "", GH_ParamAccess.item); //2
            pManager.AddNumberParameter("D [CO2eq/kg]", "D [CO2eq/kg]", "", GH_ParamAccess.item); //3



            Params.Input[1].Optional = true;
            Params.Input[2].Optional = true;
            Params.Input[3].Optional = true;
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


            pManager.AddNumberParameter(Constants.GWP_A1_A3.Name, Constants.GWP_A1_A3.NickName, Constants.GWP_A1_A3.Discription, GH_ParamAccess.item);  //6


            pManager.AddNumberParameter(Constants.ODP.Name, Constants.ODP.NickName, Constants.ODP.Discription, GH_ParamAccess.item);  //7


            pManager.AddNumberParameter("POCP", "POCP", "", GH_ParamAccess.item);  //8


            pManager.AddNumberParameter("EP", "EP", "", GH_ParamAccess.item);  //9


            pManager.AddNumberParameter("AP", "AP", "", GH_ParamAccess.item);  //10


            pManager.AddNumberParameter("A4 [CO2eq/kg]", "A4 [CO2eq/kg]", "", GH_ParamAccess.item); //11


            pManager.AddNumberParameter("C [CO2eq/kg]", "C [CO2eq/kg]", "", GH_ParamAccess.item); //12


            pManager.AddNumberParameter("D [CO2eq/kg]", "D [CO2eq/kg]", "", GH_ParamAccess.item); //13


            pManager.AddTextParameter("DataSource", "DataSource", "", GH_ParamAccess.item); //14

            //15
            pManager.AddTextParameter("MaterialGUID", "MaterialGUID", "used for debugging", GH_ParamAccess.item);

        }


        //Presitent variables
        private bool in0NeedsUpdate = false; // used to check for new value list connection.


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //List<string> debugString = new List<string>();

            string selectedMaterial = string.Empty;
            

            //Populate value list input 1
            if (Params.Input[0].SourceCount == 0)
            {
                in0NeedsUpdate = true;
            }
            else if (in0NeedsUpdate)
            {
                GH_ValueList vallist = Params.Input[0].Sources[0] as GH_ValueList;
                if (vallist != null)
                {

                    vallist.NickName = "Material Name:";
                    vallist.ListMode = GH_ValueListMode.DropDown;
                    vallist.ListItems.Clear();

                    foreach (LCA_Material mat in SqliteDataAcces.GetMaterials())
                    {
                        vallist.ListItems.Add(new GH_ValueListItem(mat.Name, $"\"{mat.Name}\""));
                    }
                }

                in0NeedsUpdate = false;
            }
            
            DA.GetData<string>(0, ref selectedMaterial);

            LCA_Material material;

            if (SqliteDataAcces.GetMaterialByName(selectedMaterial, out material))
            {
                if(material == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid material selected"); return; }



                double _tempNR = 0;

                if (DA.GetData(1, ref _tempNR)) { material.A4_A5 = _tempNR; }
                if (DA.GetData(2, ref _tempNR)) { material.C = _tempNR; }
                if (DA.GetData(3, ref _tempNR)) { material.D = _tempNR; }

                material.DataSource = "MaterialPytamiden.dk";

            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid material selected"); return;
            }


            //SET OUTPUTS


            DA.SetData(0, material);

            DA.SetData(1, material.Name);
            DA.SetData(2, material.Category);
            DA.SetData(3, material.Description);
            DA.SetData(4, material.Density);
            DA.SetData(5, material.Insulation);
            DA.SetData(6, material.GWP);
            DA.SetData(7, material.ODP);
            DA.SetData(8, material.POCP);
            DA.SetData(9, material.EP);
            DA.SetData(10, material.AP);
            DA.SetData(11, material.A4_A5);
            DA.SetData(12, material.C);
            DA.SetData(13, material.D);
            DA.SetData(14, material.DataSource);

            DA.SetData(15, material.MaterialGUID);

        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;





        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("63F5C8D5-7E8B-4DE3-A162-32341C5CDB36");
    }
}