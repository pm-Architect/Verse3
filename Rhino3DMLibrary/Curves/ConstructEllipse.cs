using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructEllipse : BaseComp
    {
        public ConstructEllipse() : base()
        {
        }
        public ConstructEllipse(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Rhino.Geometry.Point point1 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(nodeBlockX, default);
            Rhino.Geometry.Point point2 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(nodeBlockY, default);
            Rhino.Geometry.Point point3 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(nodeBlockZ, default);

            if (point1 != null && point2 != null && point3 != null)
            {
                Ellipse ellipse = new Ellipse(point1.Location, point2.Location, point3.Location);
                GeometryBase geo = ellipse.ToNurbsCurve();
                this.ChildElementManager.SetData<GeometryBase>(geo, nodeBlockResult);
            }

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Ellipse", "Line", "Curve");

        private RhinoGeometryDataNode nodeBlockX;
        private RhinoGeometryDataNode nodeBlockY;
        private RhinoGeometryDataNode nodeBlockZ;

        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Point 1");

            nodeBlockY = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Point 2");

            nodeBlockZ = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockZ, "Point 3");

            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Ellipse", true);
        }
    }
}
