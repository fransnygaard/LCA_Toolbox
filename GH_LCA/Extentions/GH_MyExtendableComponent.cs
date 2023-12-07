using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KarambaUIWidgets.UIWidgets;
using System.Security.Permissions;

namespace GH_LCA.Extentions
{



    public abstract class GH_MyExtendableComponent : GH_ExtendableComponent
    {
        protected internal GH_MyExtendableComponent(string sName, string sAbbreviation, string sDescription, string sCategory, string sSubCategory)
    : base(sName, sAbbreviation, sDescription, sCategory, sSubCategory)
        {

        }


        protected Dictionary<String, int> inputParams, outputParams;

        private int indexCounter = -1;
        protected int IndexCounter {
            get {
                indexCounter++;
                return indexCounter;
            }
        }
        protected void inputIndexCounter_Reset() { indexCounter = -1; inputParams = new Dictionary<string, int>(); }
        protected void  outputIndexCounter_Reset() { indexCounter = -1; outputParams = new Dictionary<string, int>(); }



        // USAGE 
        /*
            inputParams.Add(Constants.Material.Name, IndexCounter);
            pManager[pManager.ParamCount - 1].Optional = true;


        and to get or set  

        DA.GetData<LCA_Model>(inputParams[Constants.Model.Name], ref model)

         */

        protected void registrerInputParams(GH_Component.GH_InputParamManager pManager)
        {
            inputIndexCounter_Reset();
            for (int i = 0; i < pManager.ParamCount; i++)
            {
                inputParams.Add(pManager[i].Name, i);
            }
        }
        protected void registrerOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            outputIndexCounter_Reset();
            for (int i = 0; i < pManager.ParamCount; i++)
            {
                outputParams.Add(pManager[i].Name, i);
            }
        }
    }
}
