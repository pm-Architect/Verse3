using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class Distance : BaseComp
    {
        public Distance() : base(0, 0)
        {
        }
        public Distance(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Rhino.Geometry.Point p1 = this.ChildElementManager.GetData<Rhino.Geometry.Point>(0);
            Rhino.Geometry.Point p2 = this.ChildElementManager.GetData<Rhino.Geometry.Point>(1);
            double dist = (p2.Location - p1.Location).Length;
            this.ChildElementManager.SetData<double>(dist, 0);
            textBlock.DisplayedText = dist.ToString();
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Distance",
                Group = "Point",
                Tab = "Vector",
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
        private NumberDataNode nodeBlockDistance;
        private RhinoGeometryDataNode nodeBlock1;
        private RhinoGeometryDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock1 = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Point A");
            
            nodeBlock2 = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock2, "Point B");

            nodeBlockDistance = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockDistance, "Distance");

            textBlock = new TextElement();
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.DisplayedText = "";
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
