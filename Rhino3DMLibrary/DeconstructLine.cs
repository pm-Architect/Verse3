using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    [Serializable]
    public class DeconstructLine : BaseComp
    {
        public DeconstructLine() : base(0, 0)
        {
        }
        public DeconstructLine(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Rhino.Geometry.LineCurve line = (Rhino.Geometry.LineCurve)this.ChildElementManager.GetData<GeometryBase>(0);
            if (line != null)
            {
                Rhino.Geometry.Point p1 = new Rhino.Geometry.Point(line.PointAtStart);
                Rhino.Geometry.Point p2 = new Rhino.Geometry.Point(line.PointAtEnd);
                this.ChildElementManager.SetData<GeometryBase>(p1, 0);
                this.ChildElementManager.SetData<GeometryBase>(p2, 1);
                textBlock.DisplayedText = p1.ToString() + " to " + p2.ToString();
            }
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Deconstruct Line",
                Group = "Line",
                Tab = "Curve",
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
        private RhinoGeometryDataNode nodeBlockLine;
        private RhinoGeometryDataNode nodeBlockp1;
        private RhinoGeometryDataNode nodeBlockp2;
        public override void Initialize()
        {
            nodeBlockLine = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockLine, "Line");

            nodeBlockp1 = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockp1, "Start");

            nodeBlockp2 = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockp2, "End");

            textBlock = new TextElement();
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.DisplayedText = "";
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
