using Core;
using System;
using System.Windows;
using System.Windows.Media;
using Verse3;
using Verse3.VanillaElements;

namespace ColorLibrary
{
    public class ConstructColor : BaseComp
    {
        public ConstructColor() : base()
        {
        }
        public ConstructColor(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            int a = (int)this.ChildElementManager.GetData<double>(nodeBlockA, 10);
            int r = (int)this.ChildElementManager.GetData<double>(nodeBlockR, 10);
            int g = (int)this.ChildElementManager.GetData<double>(nodeBlockG, 10);
            int b = (int)this.ChildElementManager.GetData<double>(nodeBlockB, 10);
            Color color = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
            this.ChildElementManager.SetData<Color>(color, nodeBlockResult);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Color", "Primitive", "Color");

        private NumberDataNode nodeBlockA;
        private NumberDataNode nodeBlockR;
        private NumberDataNode nodeBlockG;
        private NumberDataNode nodeBlockB;
        private ColorDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockA = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockA, "A");
            
            nodeBlockR = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockR, "R");

            nodeBlockG = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockG, "G");

            nodeBlockB = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockB, "B");

            nodeBlockResult = new ColorDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Color", true);
        }
    }

    public class ColorDataNode : DataNodeElement<Color>
    {
        public ColorDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
}
