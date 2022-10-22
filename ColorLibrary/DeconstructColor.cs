using Core;
using System;
using System.Windows;
using System.Windows.Media;
using Verse3;
using Verse3.VanillaElements;

namespace ColorLibrary
{
    public class DeconstructColor : BaseComp
    {
        public DeconstructColor() : base()
        {
        }
        public DeconstructColor(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Color color = this.ChildElementManager.GetData(nodeBlock, Color.FromArgb(255,255,255,255));

            this.ChildElementManager.SetData(color.A, nodeBlockA);
            this.ChildElementManager.SetData(color.R, nodeBlockR);
            this.ChildElementManager.SetData(color.G, nodeBlockG);
            this.ChildElementManager.SetData(color.B, nodeBlockB);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Deconstruct color", "Primitive", "Color");

        private ColorDataNode nodeBlock;
        private NumberDataNode nodeBlockA;
        private NumberDataNode nodeBlockR;
        private NumberDataNode nodeBlockG;
        private NumberDataNode nodeBlockB;

        public override void Initialize()
        {

            nodeBlock = new ColorDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Color");

            nodeBlockA = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockA, "A");
            
            nodeBlockR = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockR, "R");

            nodeBlockG = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockG, "G");

            nodeBlockB = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockB, "B");


        }
    }

}
