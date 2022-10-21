using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class Exponent : BaseComp
    {

        #region Constructors

        public Exponent() : base(0, 0)
        {
         
        }

        public Exponent(int x, int y) : base(x, y)
        {
          
        }

        #endregion

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData<double>(0, 1);
            double b = this.ChildElementManager.GetData<double>(1, 1);
            this.ChildElementManager.SetData<double>((Math.Pow(a, b)), 0);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Exponent", "Intermediate Operations", "Math");
        
        private NumberDataNode nodeBlock;
        private NumberDataNode nodeBlock1;
        private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Number");

            nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Power");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result", true);
        }
    }
}
