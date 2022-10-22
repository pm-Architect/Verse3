using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class NotGate : BaseComp
    {

        

        #region Constructors

        public NotGate() : base()
        {
        }

        public NotGate(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override CompInfo GetCompInfo() => new CompInfo(this, "NOT Gate", "Gates", "Boolean");

        public override void Compute()
        {
            bool a = this.ChildElementManager.GetData<bool>(0, false);
            this.ChildElementManager.SetData<bool>((!(a)), 0);
        }
        
        private BooleanDataNode nodeBlock;
        private BooleanDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new BooleanDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "A");

            nodeBlock2 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result", true);
        }
    }
}
