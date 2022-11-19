using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructPolygon : BaseComp
    {
        public ConstructPolygon() : base()
        {
        }
        public ConstructPolygon(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Plane plane = new Plane();

            ((Rhino.Geometry.PlaneSurface)this.ChildElementManager.GetData<GeometryBase>(nodeBlockX, default)).TryGetPlane(out plane);
            double radius = Math.Abs(this.ChildElementManager.GetData<double>(nodeBlockY, 50));
            double sideCount = (int)Math.Abs(this.ChildElementManager.GetData<double>(nodeBlockZ, 10));

            if (plane.IsValid && sideCount >= 3 && radius > 0)
            {
                Circle circle = new Circle(plane, radius);
                Polyline polyline = Polyline.CreateInscribedPolygon(circle, Math.Abs((int)sideCount));
                GeometryBase geo = polyline.ToPolylineCurve();
                this.ChildElementManager.SetData<GeometryBase>(geo, nodeBlockResult);
            }

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Polygon", "Line", "Curve");

        private RhinoGeometryDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;
        private NumberDataNode nodeBlockZ;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Plane");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Radius");

            nodeBlockZ = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockZ, "Side Count");

            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Polygon", true);
        }
    }
}
