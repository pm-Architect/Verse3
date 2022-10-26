using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class LessThan : BaseComp
    {

        

        #region Constructors

        public LessThan() : base()
        {
        }

        public LessThan(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override CompInfo GetCompInfo() => new CompInfo(this, "Less Than", "Comparison", "Math");

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData<double>(nodeBlock, 0);
            double b = this.ChildElementManager.GetData<double>(nodeBlock1, 0);
            this.ChildElementManager.SetData<bool>((a < b), nodeBlock2);
            this.ChildElementManager.SetData<bool>((a <= b), nodeBlock3);
        }
        
        private NumberDataNode nodeBlock;
        private NumberDataNode nodeBlock1;
        private BooleanDataNode nodeBlock2;
        private BooleanDataNode nodeBlock3;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "A");

            nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "B");

            nodeBlock2 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Less Than", true);

            nodeBlock3 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Less Than/ Equal To");
        }
    }
}
