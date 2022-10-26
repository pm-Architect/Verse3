using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructPlane : BaseComp
    {
        public ConstructPlane() : base()
        {
        }
        public ConstructPlane(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Rhino.Geometry.Point point1 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(nodeBlock1, default);
            Rhino.Geometry.Point point2 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(nodeBlock2, default);
            Rhino.Geometry.Point point3 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(nodeBlock3, default);
            if (point1 != null && point2 != null && point3 != null)
            {
                Plane plane = new Plane(point1.Location, point2.Location, point3.Location);
           
                GeometryBase geo = new Rhino.Geometry.PlaneSurface(plane, new Interval(-10.0, 10.0), new Interval(-10.0, 10.0));
                this.ChildElementManager.SetData<GeometryBase>(geo, nodeBlockResult);
            }

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Plane", "Basic", "Surface");

        private RhinoGeometryDataNode nodeBlock1;
        private RhinoGeometryDataNode nodeBlock2;
        private RhinoGeometryDataNode nodeBlock3;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlock1 = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Point 1");

            nodeBlock2 = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock2, "Point 2");

            nodeBlock3 = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock3, "Point 3");

            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Plane", true);
        }
    }
}
