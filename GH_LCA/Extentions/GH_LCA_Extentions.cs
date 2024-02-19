using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;

namespace LCA_Toolbox
{
    public static class GH_LCA_Extentions
    {
        // Cast to material , else create empty material.
        public static bool TryGetMaterial(this IGH_Goo input, out LCA_Material material)
        {
            if (input == null)
            {
                material = new LCA_Material();
                return true;
            }
            else if (input.CastTo<LCA_Material>(out LCA_Material _material))
                {
                material = new LCA_Material(_material);
                return true;
                }
            else if (input.CastTo<LCA_Element>(out LCA_Element _element))
            {
                material = new LCA_Material(_element.Material);
                return true;
            }
            else
            {
                material = new LCA_Material();
                return false;
            }

        }

        public static bool TryGetElement(this IGH_Goo input, out LCA_Element element)
        {
            if (input == null)
            {
                element = new LCA_Element();
                return true;
            }
            else if( input.CastTo<LCA_Element>(out LCA_Element _element))
            {
                element = _element;
                return true;
            }
            else
            {
                element = new LCA_Element();
                return false;
            }
        }
    }
}
