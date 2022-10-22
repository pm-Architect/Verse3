using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class SquareRoot : BaseComp
    {

        #region Constructors

        public SquareRoot() : base()
        {
           
        }

        public SquareRoot(int x, int y) : base(x, y)
        {
           
        }

        #endregion

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData<double>(0, 1);
            this.ChildElementManager.SetData<double>((Math.Sqrt(a)), 0);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Square Root", "Advanced Operations", "Math");
        
        private NumberDataNode nodeBlock;
        private NumberDataNode nodeBlock1;
        private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "A");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result", true);
        }
    }
}
