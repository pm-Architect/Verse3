using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructRectangle : BaseComp
    {
        public ConstructRectangle() : base(0, 0)
        {
        }
        public ConstructRectangle(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            ((Rhino.Geometry.PlaneSurface)this.ChildElementManager.GetData<GeometryBase>(0)).TryGetPlane(out Rhino.Geometry.Plane plane);
            double width = this.ChildElementManager.GetData<double>(1, 20);
            double height = this.ChildElementManager.GetData<double>(2, 10);
            if (plane.IsValid)
            {
                Rectangle3d rectangle = new Rectangle3d(plane, width, height);
                GeometryBase geo = rectangle.ToNurbsCurve();
                this.ChildElementManager.SetData<GeometryBase>(geo, 0);
                textBlock.DisplayedText = rectangle.ToString();
            }

        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Construct Rectangle",
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
        private NumberDataNode nodeBlockY;
        private NumberDataNode nodeBlockZ;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Plane");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Width");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Height");

            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Rectangle");

            textBlock = new TextElement();
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.DisplayedText = "";
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
