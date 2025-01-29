using System;
using System.Collections.Generic;
using LAC_ClassLibrary;
using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using GH_GeneralClassLibrary.UI;


namespace LCA_Toolbox
{
    public class LAC_ElementFromVolume_Component : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the LAC_CreateElementFromSurface_Component class.
        /// </summary>
        public LAC_ElementFromVolume_Component()
          : base("LCA: Element from Volume", "LCA: Element from Volume",
              "Description",
              Constants.PluginName, Constants.SubElements)
        {

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //SPESIFIC INPUTS FOR ELEMENT FROM Volume
            pManager.AddNumberParameter(Constants.Volume.Name, Constants.Volume.NickName, Constants.Volume.Discription, GH_ParamAccess.item);

            //FOR ALL ELEMENT TO XXX components
            pManager.AddGenericParameter(Constants.Material.Name, Constants.Material.NickName, Constants.Material.Discription, GH_ParamAccess.item);

            pManager.AddIntegerParameter(Constants.Lifetime.Name, Constants.Lifetime.NickName, Constants.Lifetime.Discription, GH_ParamAccess.item, -1);

            pManager.AddNumberParameter(Constants.A4_kg.Name, Constants.A4_kg.NickName, Constants.A4_kg.Discription, GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddNumberParameter(Constants.D_Reuse.Name, Constants.D_Reuse.NickName, Constants.D_Reuse.Discription, GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddTextParameter(Constants.Element_Name.Name, Constants.Element_Name.NickName, Constants.Element_Name.Discription, GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddTextParameter(Constants.Element_Group.Name, Constants.Element_Group.NickName, Constants.Element_Group.Discription, GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

            registrerInputParams(pManager);


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Element.Name, Constants.Element.NickName, Constants.Element.Discription, GH_ParamAccess.item);

            pManager.AddNumberParameter(Constants.Weight.Name, Constants.Weight.NickName, Constants.Weight.Discription, GH_ParamAccess.item);

            pManager.AddNumberParameter(Constants.Volume.Name, Constants.Volume.NickName, Constants.Volume.Discription, GH_ParamAccess.item);

            pManager.AddNumberParameter(Constants.A1toA3_ELEMENT.Name, Constants.A1toA3_ELEMENT.NickName, Constants.A1toA3_ELEMENT.Discription, GH_ParamAccess.item);
            
            pManager.AddNumberParameter(Constants.A4_ELEMENT.Name, Constants.A4_ELEMENT.NickName, Constants.A4_ELEMENT.Discription, GH_ParamAccess.item);
            
            pManager.AddNumberParameter(Constants.A1toA4_ELEMENT.Name, Constants.A1toA4_ELEMENT.NickName, Constants.A1toA4_ELEMENT.Discription, GH_ParamAccess.item);

            registrerOutputParams(pManager);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            LCA_Material material = new LCA_Material();

            if (!DA.GetData<LCA_Material>(inputParams[Constants.Material.Name], ref material)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid material input"); return;}

            double volume = 0;

            DA.GetData<double>(inputParams[Constants.Volume.Name], ref volume);




            int expectedLifetime = -1;
            DA.GetData<int>(inputParams[Constants.Lifetime.Name], ref expectedLifetime);
            LCA_Element element = new LCA_Element(material, volume, expectedLifetime);

            string _tempStr = string.Empty;
            double _tempNr = double.NaN;
            if (DA.GetData(inputParams[Constants.Element_Name.Name], ref _tempStr)) { element.Element_Name = _tempStr; }
            if (DA.GetData(inputParams[Constants.Element_Group.Name], ref _tempStr)) { element.Element_Group = _tempStr; }



            //FOR ALL ELEMENT TO XX COMPONENTS
            //Set A4
            if (DA.GetData(inputParams[Constants.A4_kg], ref _tempNr)) { element.Element_A4_perKG = _tempNr; }

            //SET DATA

            DA.SetData(outputParams[Constants.Element], element);
            DA.SetData(outputParams[Constants.Volume], element.Element_Volume);
            DA.SetData(outputParams[Constants.Weight], element.Element_Weight);
            DA.SetData(outputParams[Constants.A1toA4_ELEMENT], element.Element_A1toA4());
            DA.SetData(outputParams[Constants.A4_ELEMENT], element.Element_A4);
            DA.SetData(outputParams[Constants.A1toA3_ELEMENT], element.Element_A1toA3);
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
            get { return new Guid("33f2c035-a227-431f-a857-d8c77619a755"); }
        }
    }
}