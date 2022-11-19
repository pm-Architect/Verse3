using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructCircle : BaseComp
    {
        public ConstructCircle() : base()
        {
        }
        public ConstructCircle(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Rhino.Geometry.Point point1 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(nodeBlockX, default);
            double radius = this.ChildElementManager.GetData<double>(nodeBlockY, 10);
            if (point1 != null)
            {
                Circle circle = new Circle(point1.Location, radius);

                GeometryBase geo = new Rhino.Geometry.ArcCurve(circle);
                this.ChildElementManager.SetData<GeometryBase>(geo, nodeBlockResult);
            }

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Circle", "Line", "Curve");

        private RhinoGeometryDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Point");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Radius");

            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Circle", true);
        }
    }
}
