using System;
using System.Collections.Generic;
using LAC_ClassLibrary;
using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using GH_GeneralClassLibrary.UI;
using Grasshopper.Kernel.Types;
using GH_GeneralClassLibrary.Utils;

namespace LCA_Toolbox
{
    public class LAC_ElementFromShell_Component : GH_MyExtendableComponent
    {
        /// <summary>
        /// Initializes a new instance of the LAC_CreateElementFromSurface_Component class.
        /// </summary>
        public LAC_ElementFromShell_Component()
          : base("LCA: Element from Shell", "LCA: Element from Shell",
              "Create a LAC:Element from shell geomerty",
              Constants.PluginName, Constants.SubElements)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            //SPESIFIC INPUTS FOR ELEMENT FROM SHELL
            pManager.AddGeometryParameter(Constants.ShellGeo.Name, Constants.ShellGeo.NickName, Constants.ShellGeo.Discription, GH_ParamAccess.item);
            pManager.AddNumberParameter(Constants.Thickness.Name, Constants.Thickness.NickName, Constants.Thickness.Discription, GH_ParamAccess.item);

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

            double thickness = -1;
            if (!DA.GetData<double>(inputParams[Constants.Thickness.Name], ref thickness)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid thickness input"); return;}
            if(thickness <= 0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Thickness needs to be larger than 0"); return; }
            thickness = LCA_HelperCalss.convertValueToMeters(thickness);


            IGH_GeometricGoo inputGeo = default;
            if (!DA.GetData<IGH_GeometricGoo>(inputParams[Constants.ShellGeo.Name], ref inputGeo)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "no valid input geometry"); return; }

            double surfaceArea = -1;
            Surface srf = null;
            Mesh mesh = null;
            Brep brep = null;

            if (GH_Convert.ToSurface(inputGeo, ref srf, GH_Conversion.Both))
            {
                surfaceArea = Rhino.Geometry.AreaMassProperties.Compute(srf).Area;
            }
            else if (GH_Convert.ToBrep(inputGeo, ref brep, GH_Conversion.Both))
            {
                surfaceArea = Rhino.Geometry.AreaMassProperties.Compute(brep).Area;
                if(brep.IsSolid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "One or more Breps are solid, this component uses the surface area * thickness to calculate volume.\n" +
                        "           To get volume of solid use LCA:Element from Solid.");
                }
            }
            else if (GH_Convert.ToMesh(inputGeo, ref mesh, GH_Conversion.Both))
            {
                surfaceArea = Rhino.Geometry.AreaMassProperties.Compute(mesh).Area;
                if (mesh.IsSolid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "One or more Meshes are solid, this component uses the surface area * thickness to calculate volume.\n" +
                        "           To get volume of solid use LCA:Element from Solid.");
                }
            }
            else
            { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid geometry"); return; }



        
            surfaceArea  = LCA_HelperCalss.convertSquaredValueToMeters(surfaceArea);
            if (surfaceArea == double.NaN || thickness == double.NaN) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Rhino units needs to be mm, cm or m"); return; }

            double volume = surfaceArea *  thickness;


            int expectedLifetime = -1;
            DA.GetData<int>(Constants.Lifetime.Name, ref expectedLifetime);
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
            get { return new Guid("B3EE58AB-C735-485A-BC25-7536E4815EF5"); }
        }
    }
}