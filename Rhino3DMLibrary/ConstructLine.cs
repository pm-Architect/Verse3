using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructLine : BaseComp
    {
        public ConstructLine() : base(0, 0)
        {
        }
        public ConstructLine(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Rhino.Geometry.Point point1 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(0);
            Rhino.Geometry.Point point2 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(1);
            Line line = new Line(point1.Location, point2.Location);
            GeometryBase geo = new Rhino.Geometry.LineCurve(line);
            this.ChildElementManager.SetData<GeometryBase>(geo, 0);
            textBlock.DisplayedText = line.ToString();
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Construct Line",
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
        private RhinoGeometryDataNode nodeBlockX;
        private RhinoGeometryDataNode nodeBlockY;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Start");

            nodeBlockY = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "End");

            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Line");

            textBlock = new TextElement();
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.DisplayedText = "";
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
