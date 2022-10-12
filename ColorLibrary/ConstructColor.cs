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
        public ConstructColor() : base(0, 0)
        {
        }
        public ConstructColor(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            int a = (int)this.ChildElementManager.GetData<double>(0, 10);
            int r = (int)this.ChildElementManager.GetData<double>(1, 10);
            int g = (int)this.ChildElementManager.GetData<double>(2, 10);
            int b = (int)this.ChildElementManager.GetData<double>(3, 10);
            Color color = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
            this.ChildElementManager.SetData<Color>(color, 0);
            textBlock.DisplayedText = color.ToString();
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Construct Color",
                Group = "Primitive",
                Tab = "Color",
                Description = "",
                Author = "",
                License = "",
                Repository = "",
                Version = "",
                Website = ""
            };
            return ci;
        }

        private TextElement textBlock = new TextElement();
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
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Color");

            textBlock = new TextElement();
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.DisplayedText = "";
            this.ChildElementManager.AddElement(textBlock);
        }
    }

    public class ColorDataNode : DataNodeElement<Color>
    {
        public ColorDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
}
