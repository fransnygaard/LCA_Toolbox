using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_GeneralClassLibrary.UI;
using Rhino.Commands;
using System.Windows.Forms;
using System.Configuration;
using Grasshopper.Kernel.Types;
using System.Drawing;
using Rhino.Display;
using System.Data;
using LAC_ClassLibrary;
using System.Runtime.CompilerServices;
using GH_GeneralClassLibrary.Utils;
using LCA_Toolbox.Database;
using GH_IO.Serialization;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace LCA_Toolbox.Components
{
    public class LCA_Result_Visualizer_Componenet : GH_MyExtendableComponent
    {

        enum GroupingModes
        {
            Element,
            Element_Name,
            Element_Group,
            Material_Name

        }

        //GH_ExtendableComponent variables
        private MenuDropDown _dropdownmenu_category;
        private string dropvariable_grouping = "Element";
        private string dropvariable_grouping_SAVED = "Element";


        ///Global variables  

        List<IGH_GeometricGoo> drawGeo = new List<IGH_GeometricGoo>();
        List<Color> colors = new List<Color>();




        //persistent values 
        public override bool Write(GH_IWriter writer)

        {

            writer.SetString("dropvariable_grouping", dropvariable_grouping);

            return base.Write(writer);

        }

        public override bool Read(GH_IReader reader)

        {

            reader.TryGetString("dropvariable_grouping", ref dropvariable_grouping_SAVED);

            update_dropdown_grouping(true);

            return base.Read(reader);

        }

        protected override void Setup(GH_ExtendableComponentAttributes attr)
        {
            //Extendable Menu (Drop Down Menu)

            GH_ExtendableMenu gH_ExtendableMenu1 = new GH_ExtendableMenu(1, "Grouping mode");
            gH_ExtendableMenu1.Name = "SELECT GROUPING";
            gH_ExtendableMenu1.Expand();
            attr.AddMenu(gH_ExtendableMenu1);

            MenuPanel dropdownmenupanel = new MenuPanel(1, "dropdowntest");
            dropdownmenupanel.Header = "random words";




            ////GROUPS DROPDOWN


            MenuStaticText menuStaticText0 = new MenuStaticText();
            menuStaticText0.Text = "Select grouping mode.";
            menuStaticText0.Header = "HEADER";
            dropdownmenupanel.AddControl(menuStaticText0);



            _dropdownmenu_category = new MenuDropDown(0, "drop_down_grouping", "group");
            _dropdownmenu_category.VisibleItemCount = 10;


            foreach (GroupingModes enumValue in Enum.GetValues(typeof(GroupingModes)))
            {
                _dropdownmenu_category.AddItem(enumValue.ToString(), enumValue.ToString());
            }




            _dropdownmenu_category.ValueChanged += _dropdown_Grouping__valueChanged;

            dropdownmenupanel.AddControl(_dropdownmenu_category);



            //Add Panel
            gH_ExtendableMenu1.AddControl(dropdownmenupanel);


        }


        private void _dropdown_Grouping__valueChanged(object sender, EventArgs e)
        {
            update_dropdown_grouping();

        }


        /// <summary>
        /// FIX SÅ MAN KAN KOPIERE UTEN Å MISTE. 
        /// </summary>
        private void update_dropdown_grouping(bool firstTimeSetup = false)
        {

            if (firstTimeSetup)
                dropvariable_grouping = dropvariable_grouping_SAVED;
            else
                dropvariable_grouping = _dropdownmenu_category.Items[_dropdownmenu_category.Value].name;





            setModelProps();
        }


        private string get_dropvariable_category
        {
            get
            {
                return _dropdownmenu_category.Items[_dropdownmenu_category.Value].name;

            }
        }

        /// 
        /// <summary>
        /// Initializes a new instance of the LCA_Result_Visualizer class.
        /// </summary>
        public LCA_Result_Visualizer_Componenet()
          : base("LCA: Result Viewer", "LCA: Result Viewer",
              "Description",
              Constants.PluginName, Constants.SubResults)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //Model input
            pManager.AddGenericParameter(Constants.Model.Name, Constants.Model.NickName, Constants.Model.Discription, GH_ParamAccess.item);
            pManager.AddBooleanParameter(Constants.NormalizeValues.Name, Constants.NormalizeValues.NickName, Constants.NormalizeValues.Discription, GH_ParamAccess.item,false );
            pManager.AddColourParameter("Color Min", "Color0", "", GH_ParamAccess.item,Color.White);
            pManager.AddColourParameter("Color Max", "Color1", "", GH_ParamAccess.item, Color.Red);

            registrerInputParams(pManager);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter(Constants.GeoGoo.Name, Constants.GeoGoo.NickName, Constants.GeoGoo.Discription, GH_ParamAccess.list);
            ((IGH_PreviewObject)pManager[0]).Hidden = true; //Hide the GEO 
            

            pManager.AddColourParameter(Constants.ResultColour.Name, Constants.ResultColour.NickName, Constants.ResultColour.Discription, GH_ParamAccess.list);

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



            drawGeo.Clear();
            colors.Clear();

            Color color0 = Color.Blue;
            Color color1 = Color.Red;
            DA.GetData<Color>(inputParams["Color Min"], ref color0);
            DA.GetData<Color>(inputParams["Color Max"], ref color1);

            //Find totoal emmions. 
            double total_embodied = model.GetEmbodied_SumModel();


            List<double> lambda = new List<double>();
            foreach(DataRow row in model.elementsDT.Rows )
            {
                object value = row["geoGoo"];
                if (value == DBNull.Value)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One Element does not contain a preview geometry");
                    continue;
                }
                else
                {

                    drawGeo.Add((row["geoGoo"] as IGH_GeometricGoo));
                    //find what factor of total emisons this element has. 
                    // ether per element or by group 

                    double elemet_embodied = double.NaN;


                    switch(dropvariable_grouping)
                    {
                        case "Element":
                            elemet_embodied = (double)row["Element_Embodied"];
                            break;

                        case "Element_Name":
                            elemet_embodied = model.GetEmbodied_SumByElementName((string)row["Element_Name"]);
                            break;
                            

                        case "Element_Group":
                            elemet_embodied = model.GetEmbodied_SumByElementGroup((string)row["Element_Group"]);
                            break;

                        case "Material_Name":
                            elemet_embodied = model.GetEmbodied_SumByMaterialName((string)row["Material_Name"]);
                            break;
                    }

                    lambda.Add(elemet_embodied / total_embodied);
                }
                
            }


            //REMAP LAMBDA ? 
            bool NormalizeValues = false;
            DA.GetData<bool>(inputParams[Constants.NormalizeValues], ref NormalizeValues);
            if (NormalizeValues) lambda = lambda.NormalizeValues();


            foreach (var l in lambda) colors.Add(ColorInterpolator.InterpolateBetween(color0, color1, l));


            //SET DATA
  
            DA.SetDataList(Constants.GeoGoo, drawGeo);
            DA.SetDataList(Constants.ResultColour,colors); 

        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {

            //base.DrawViewportMeshes(args);
            for (int i = 0; i < drawGeo.Count; i++)
            {
                if (i > colors.Count - 1) continue;
                var material = new Rhino.Display.DisplayMaterial(colors[i]);

                Mesh mesh = null;
                Brep brep = null;
                if (GH_Convert.ToBrep(drawGeo[i], ref brep, GH_Conversion.Both))
                {
                    args.Display.DrawBrepShaded(brep, material);
                }
                else if (GH_Convert.ToMesh(drawGeo[i], ref mesh, GH_Conversion.Both))
                {
                    args.Display.DrawMeshShaded(mesh, material);
                }

            }
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



        //To recompute component/solve instance method.
        protected void setModelProps()
        {
            ((GH_DocumentObject)this).ExpireSolution(true);
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4A9DFD9B-EDEC-49B9-9006-0834D0FC7BD0"); }
        }
    }
}