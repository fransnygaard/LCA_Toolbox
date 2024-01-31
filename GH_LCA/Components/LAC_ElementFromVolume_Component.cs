using System;
using System.Collections.Generic;
using LAC_ClassLibrary;
using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using GH_GeneralClassLibrary.UI;


namespace GH_LCA
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
            pManager.AddGenericParameter(Constants.Material.Name, Constants.Material.NickName, Constants.Material.Discription, GH_ParamAccess.item);

            pManager.AddNumberParameter(Constants.Volume.Name, Constants.Volume.NickName, Constants.Volume.Discription, GH_ParamAccess.item);

            pManager.AddIntegerParameter(Constants.Lifetime.Name, Constants.Lifetime.NickName, Constants.Lifetime.Discription, GH_ParamAccess.item, -1);

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

            pManager.AddNumberParameter(Constants.GWP_A1_A3.Name, Constants.GWP_A1_A3.NickName, Constants.GWP_A1_A3.Discription, GH_ParamAccess.item);



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
            if (DA.GetData(inputParams[Constants.Element_Name.Name], ref _tempStr)) { element.Element_Name = _tempStr; }
            if (DA.GetData(inputParams[Constants.Element_Group.Name], ref _tempStr)) { element.Element_Group = _tempStr; }

            DA.SetData(outputParams[Constants.Element.Name], element);
            DA.SetData(outputParams[Constants.Weight.Name], element.Element_Weight);
            DA.SetData(outputParams[Constants.GWP_A1_A3.Name], element.Element_GWP_A13);


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