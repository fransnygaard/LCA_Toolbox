using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using LAC_ClassLibrary;
using KarambaUIWidgets.UIWidgets;
using GH_LCA.Extentions;
using System.Xml.Linq;
using GH_IO.Serialization;

namespace GH_LCA
{
    public class LCA_ConstrucMaterial_Pyramiden_Component : GH_MyExtendableComponent
    {


        //GH_ExtendableComponent variables
        private MenuDropDown _dropdownmenu_category;
        private MenuDropDown _dropdownmenu_material;
        private MenuCheckBox _checkbox1;
        private MenuCheckBox _checkbox2;
        private string _valuecheckbox1 = "Initial Value";
        private string _valuecheckbox2 = "Initial Value";
        private MenuRadioButtonGroup _colorGrp;
        private string radiovariable = "radiovar";
        private double _doublefunction;
        private MenuSlider _sliderexample;
        private static string dropvariable_notSelected = "Not selected";
        private string dropvariable_category = "--ALL--";
        private string dropvariable_material = dropvariable_notSelected;
        private string dropvariable_category_SAVED = "--ALL--";
        private string dropvariable_material_SAVED = dropvariable_notSelected;

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
            Constants.PluginName,Constants.SubMaterials)
        {
        }


        
        //persistent values 
        public override bool Write(GH_IWriter writer)

        {

            writer.SetString("dropvariable_category", dropvariable_category);
            writer.SetString("dropvariable_material", dropvariable_material);

            return base.Write(writer);

        }

        public override bool Read(GH_IReader reader)

        {

            reader.TryGetString("dropvariable_category", ref dropvariable_category_SAVED);
            reader.TryGetString("dropvariable_material", ref dropvariable_material_SAVED);

            update_dropdown_category(true);

            return base.Read(reader);

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            inputIndexCounter_Reset();
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            outputIndexCounter_Reset();

            pManager.AddGenericParameter(Constants.Material.Name, Constants.Material.NickName, Constants.Material.Discription, GH_ParamAccess.item); // 0
            outputParams.Add(pManager[pManager.ParamCount - 1].Name, IndexCounter);

        }

        protected override void Setup(GH_ExtendableComponentAttributes attr)
        {
            //Extendable Menu (Drop Down Menu)

            GH_ExtendableMenu gH_ExtendableMenu1 = new GH_ExtendableMenu(1, "MATERIAL");
            gH_ExtendableMenu1.Name = "SELECT MATERIAL";
            gH_ExtendableMenu1.Expand();
            attr.AddMenu(gH_ExtendableMenu1);

            MenuPanel dropdownmenupanel = new MenuPanel(1, "dropdowntest");
            dropdownmenupanel.Header = "random words";




            ////CATEGORY


            MenuStaticText menuStaticText0 = new MenuStaticText();
            menuStaticText0.Text = "Select material category.";
            menuStaticText0.Header = "HEADER";
            dropdownmenupanel.AddControl(menuStaticText0);



            _dropdownmenu_category = new MenuDropDown(0, "drop_down_menu_cat", "cat");
            _dropdownmenu_category.VisibleItemCount = 10;
            // _dropdownmenu_category.Header = "A random header hehe";


            _dropdownmenu_category.AddItem("--ALL--", "------- ALL categories -------");
 
            foreach (string group in SqliteDataAcces.GetMaterialGroups())
            {
                _dropdownmenu_category.AddItem(group, group);
            }



            _dropdownmenu_category.ValueChanged += _dropdown_Category__valueChanged;

            dropdownmenupanel.AddControl(_dropdownmenu_category);




            //MATERIALS


            MenuStaticText menuStaticText1 = new MenuStaticText();
            menuStaticText1.Text = "Select material.";
            //menuStaticText0.Header = "HEADER";
            dropdownmenupanel.AddControl(menuStaticText1);

            _dropdownmenu_material = new MenuDropDown(1, "drop_down_menu_mat", "mat");
            _dropdownmenu_material.VisibleItemCount = 10;
            //_dropdownmenu_material.Header = "A random header hehe";

            if (dropvariable_category == "--ALL--")
            {
                foreach (LCA_Material material in SqliteDataAcces.GetMaterials())
                {
                    _dropdownmenu_material.AddItem(material.Name, material.Name);
                }
            }
            else
            {
                foreach (LCA_Material material in SqliteDataAcces.GetMaterialsByGroup(get_dropvariable_category))
                {
                    _dropdownmenu_material.AddItem(material.Name, material.Name);
                }
            }

            // INIT WITH ALL MATERIALS

            dropdownmenupanel.AddControl(_dropdownmenu_material);

            _dropdownmenu_material.ValueChanged += _dropdown_material__valueChanged;
           // dropvariable_material = _dropdownmenu_material.Items[_dropdownmenu_material.Value].name;


            //Add Panel
            gH_ExtendableMenu1.AddControl(dropdownmenupanel);

            //update_dropdown_category(true); 

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
            


            LCA_Material material;

            if (SqliteDataAcces.GetMaterialByName(get_dropvariable_material, out material))
            {
                if(material == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid material selected"); return; }



                double _tempNR = 0;

                //if (DA.GetData(1, ref _tempNR)) { material.A4_A5 = _tempNR; }
                //if (DA.GetData(2, ref _tempNR)) { material.C = _tempNR; }
                //if (DA.GetData(3, ref _tempNR)) { material.D = _tempNR; }

                material.DataSource = "MaterialPytamiden.dk";

            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid material selected"); return;
            }


            //SET OUTPUTS


            DA.SetData(0, material);

            //DA.SetData(1, material.Name);
            //DA.SetData(2, material.Category);
            //DA.SetData(3, material.Description);
            //DA.SetData(4, material.Density);
            //DA.SetData(5, material.Insulation);
            //DA.SetData(6, material.GWP);
            //DA.SetData(7, material.ODP);
            //DA.SetData(8, material.POCP);
            //DA.SetData(9, material.EP);
            //DA.SetData(10, material.AP);
            //DA.SetData(11, material.A4_A5);
            //DA.SetData(12, material.C);
            //DA.SetData(13, material.D);
            //DA.SetData(14, material.DataSource);

            //DA.SetData(15, material.MaterialGUID);

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



        //list functionality


        //tickbox functionality
        private void _dispCheck__valueChanged(object sender, EventArgs e)
        {
            if (((MenuCheckBox)sender).Active)
            {
                _valuecheckbox1 = "Value has been changed to true";
            }
            else
            {
                _valuecheckbox1 = "Value has been changed to false";
            }
            setModelProps();
        }

        private void _dispCheck2__valueChanged(object sender, EventArgs e)
        {
            if (((MenuCheckBox)sender).Active)
            {
                _valuecheckbox2 = "Value of checkbox 2 is true";
            }
            else
            {
                _valuecheckbox2 = "Value of checkbox 2 is false";
            }
            setModelProps();
        }

        //radiobutton functionality

        private void _dropdown_Category__valueChanged(object sender, EventArgs e)
        {
            update_dropdown_category();

        }


        private void _dropdown_material__valueChanged(object sender, EventArgs e)
        {
            update_dropdown_material();

      

        }



        private void update_dropdown_material(bool firstTimeSetup = false)
        {
            if (firstTimeSetup)
                dropvariable_material = dropvariable_material_SAVED;
            else
                dropvariable_material = _dropdownmenu_material.Items[_dropdownmenu_material.Value].name;

            setModelProps();
        }
         /// <summary>
         /// FIX SÅ MAN KAN KOPIERE UTEN Å MISTE. 
         /// </summary>
        private void update_dropdown_category(bool firstTimeSetup = false)
        {
            
            if (firstTimeSetup)
                dropvariable_category = dropvariable_category_SAVED;
            else
                dropvariable_category = _dropdownmenu_category.Items[_dropdownmenu_category.Value].name;



            _dropdownmenu_material.Clear();

            if (dropvariable_category == "--ALL--")
            {
                foreach (LCA_Material material in SqliteDataAcces.GetMaterials())
                {
                    _dropdownmenu_material.AddItem(material.Name, material.Name);
                }
            }
            else
            {
                foreach (LCA_Material material in SqliteDataAcces.GetMaterialsByGroup(dropvariable_category))
                {
                    _dropdownmenu_material.AddItem(material.Name, material.Name);
                }
            }

            update_dropdown_material(firstTimeSetup);

            setModelProps();
        }

        private string get_dropvariable_material
        { 
            get
            {
               return _dropdownmenu_material.Items[_dropdownmenu_material.Value].name;
            }
        }


        private string get_dropvariable_category
        {
            get
            {
                return _dropdownmenu_category.Items[_dropdownmenu_category.Value].name;

            }
        }




        //private void _drop__valueChanged(object sender, EventArgs e)
        //{
        //    _ = (MenuDropDown)sender;
        //    setModelProps();
        //}

        private void _colorGrp__valueChanged(object sender, EventArgs e)
        {
            int radioindex = _colorGrp.GetActiveInt()[0];
            radiovariable = radioindex.ToString() + ". Radiobutton Active";
            setModelProps();
        }

        private void _slider__valueChanged(object sender, EventArgs e)
        {
            _doublefunction = ((MenuSlider)sender).Value;
            setModelProps();
        }

        //To recompute component/solve instance method.
        protected void setModelProps()
        {
            ((GH_DocumentObject)this).ExpireSolution(true);
        }

    }
}