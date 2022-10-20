using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class ConvertToRadians : BaseComp
    {



        #region Constructors

        public ConvertToRadians() : base(0, 0)
        {
          
        }

        public ConvertToRadians(int x, int y) : base(x, y)
        {
   
        }

        #endregion

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData(nodeBlock, 0);
            this.ChildElementManager.SetData((Math.PI/180)*a, nodeBlock2);
            
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Convert to Radians", "Conversions", "Math 2");

        private NumberDataNode nodeBlock;
        private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Degrees");



            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Radians", true);


        }
    }
}
