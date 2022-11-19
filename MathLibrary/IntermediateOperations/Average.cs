using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class Average : BaseComp
    {

        #region Constructors

        public Average() : base()
        {
            
        }

        public Average(int x, int y) : base(x, y)
        {
            
        }

        #endregion

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData<double>(nodeBlock, 1);
            double b = this.ChildElementManager.GetData<double>(nodeBlock1, 1);
            this.ChildElementManager.SetData<double>((a+b)/2, nodeBlock2);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Average", "Intermediate Operations", "Math");
        
        private NumberDataNode nodeBlock;
        private NumberDataNode nodeBlock1;
        private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "A");

            nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "B");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result");
        }
    }
}
