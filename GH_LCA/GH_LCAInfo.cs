using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace LCA_Toolbox
{
    public class GH_LCAInfo : GH_AssemblyInfo
    {
        public override string Name => Constants.PluginName;

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;


        //Return a short string describing the purpose of this GHA library.
        public override string Description => Constants.PluginDescription;

        public override Guid Id => new Guid("162E4178-8907-40DE-A37B-A3073A5B6325");

        //Return a string identifying you or your company.
        public override string AuthorName => "Frans Nygaard";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "www.fransnygaard.com";

        public override string Version => "0.1.5-Alpha";
    }
}