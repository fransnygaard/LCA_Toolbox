using GH_GeneralClassLibrary.UI;
using GH_GeneralClassLibrary.Utils;
using Grasshopper.Kernel;
using LAC_ClassLibrary;
using Rhino.Geometry;
using System;
using System.Xml;

namespace GH_LCA
{
    public class LAC_ElementFromCurve_Component : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the LAC_CreateElementFromCurve class.
        /// </summary>
        public LAC_ElementFromCurve_Component()
          : base("LCA: Element from Curve", "LCA: Element from Curve",
              "Create a LAC:Element from curve geomerty",
              Constants.PluginName, Constants.SubElements)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter(Constants.CurveGeo.Name, Constants.CurveGeo.NickName, Constants.CurveGeo.Discription, GH_ParamAccess.item); //0
            pManager.AddGenericParameter(Constants.Material.Name,Constants.Material.NickName,Constants.Material.Discription, GH_ParamAccess.item); //1
            pManager.AddNumberParameter(Constants.CrossSection.Name, Constants.CrossSection.NickName,Constants.CrossSection.Discription, GH_ParamAccess.item);//2
            pManager.AddIntegerParameter(Constants.Lifetime.Name, Constants.Lifetime.NickName,Constants.Lifetime.Discription, GH_ParamAccess.item,-1);//3

            pManager.AddTextParameter(Constants.Element_Name.Name,Constants.Element_Name.NickName,Constants.Element_Name.Discription, GH_ParamAccess.item); //4
            Params.Input[4].Optional = true;

            pManager.AddTextParameter(Constants.Element_Group.Name,Constants.Element_Group.NickName, Constants.Element_Group.Discription, GH_ParamAccess.item); //5
            Params.Input[5].Optional = true;

            registrerInputParams(pManager);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Element.Name,Constants.Element.NickName,Constants.Element.Discription, GH_ParamAccess.item);
            pManager.AddNumberParameter(Constants.Volume.Name,Constants.Volume.NickName,Constants.Volume.Discription, GH_ParamAccess.item);
            pManager.AddNumberParameter(Constants.Weight.Name,Constants.Weight.NickName,Constants.Weight.Discription, GH_ParamAccess.item);
            pManager.AddNumberParameter(Constants.GWP_A1_A3.Name,Constants.GWP_A1_A3.NickName, Constants.GWP_A1_A3.Discription, GH_ParamAccess.item);

            registrerOutputParams(pManager);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            LCA_Material material = new LCA_Material();

            if (!DA.GetData<Curve>(inputParams[Constants.CurveGeo], ref curve)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "no input curve"); return; }
            if (curve == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Curve can not be NULL"); return; }



            if (!DA.GetData<LCA_Material>(inputParams[Constants.Material], ref material))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid material input");
                return;
            }

            double crossSectionArea = double.NaN;

            DA.GetData(inputParams[Constants.CrossSection], ref crossSectionArea);

            if(crossSectionArea <= 0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cross section area needs to be larger than 0"); return; }

  
            double curveLength = curve.GetLength();
           

            double volume = curveLength * crossSectionArea;

            //Convert volume to meters
            volume = LCA_HelperCalss.convertCubedValueToMeters(volume);
            if (volume == double.NaN) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Rhino units needs to be mm, cm or m"); return; }


            int expectedLifetime = -1;
            DA.GetData<int>(inputParams[Constants.Lifetime], ref expectedLifetime);
            LCA_Element element = new LCA_Element(material, volume, expectedLifetime);

            //ELEMENT NAME AND GROUP
            string _tempStr = string.Empty;

            if (DA.GetData(inputParams[Constants.Element_Name], ref _tempStr)) { element.Element_Name = _tempStr; }
            if (DA.GetData(inputParams[Constants.Element_Group], ref _tempStr)) { element.Element_Group = _tempStr; }

            DA.SetData(outputParams[Constants.Element], element);
            DA.SetData(outputParams[Constants.Volume], element.Element_Volume);
            DA.SetData(outputParams[Constants.Weight], element.Element_Weight);
            DA.SetData(outputParams[Constants.GWP_A1_A3], element.Element_GWP_A13);


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
            get { return new Guid("778A4467-CC0E-4753-9713-F347619BD052"); }
        }
    }
}