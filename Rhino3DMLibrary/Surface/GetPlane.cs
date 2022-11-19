using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class GetPlane : BaseComp
    {
        public GetPlane() : base()
        {
        }
        public GetPlane(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Rhino.Geometry.Point point = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(nodeBlock1, default);

            Vector3d vectorz = new Vector3d(0.0, 0.0, 1.0);
            Vector3d vectory = new Vector3d(0.0, 1.0, 0.0);
            Vector3d vectorx = new Vector3d(1.0, 0.0, 0.0);
            Point3d originbase = new Point3d(0, 0, 0);
            Rhino.Geometry.Point origin = new Rhino.Geometry.Point(originbase);

            if (point == null)
            {
                point = origin;
            }

                Plane planeXY = new Plane(point.Location, vectorz);
                GeometryBase geoXY = new Rhino.Geometry.PlaneSurface(planeXY, new Interval(-10.0, 10.0), new Interval(-10.0, 10.0));
                this.ChildElementManager.SetData<GeometryBase>(geoXY, nodeBlock2);

                Plane planeYZ = new Plane(point.Location, vectorx);
                GeometryBase geoYZ = new Rhino.Geometry.PlaneSurface(planeYZ, new Interval(-10.0, 10.0), new Interval(-10.0, 10.0));
                this.ChildElementManager.SetData<GeometryBase>(geoYZ, nodeBlock3);

                Plane planeZX = new Plane(point.Location, vectory);
                GeometryBase geoZX = new Rhino.Geometry.PlaneSurface(planeZX, new Interval(-10.0, 10.0), new Interval(-10.0, 10.0));
                this.ChildElementManager.SetData<GeometryBase>(geoZX, nodeBlock4);

        }


        public override CompInfo GetCompInfo() => new CompInfo(this, "Get Plane", "Basic", "Surface");

        private RhinoGeometryDataNode nodeBlock1;
        private RhinoGeometryDataNode nodeBlock2;
        private RhinoGeometryDataNode nodeBlock3;
        private RhinoGeometryDataNode nodeBlock4;

        public override void Initialize()
        {
            nodeBlock1 = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Point Origin");

            nodeBlock2 = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Plane XY", true);

            nodeBlock3 = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Plane YZ");

            nodeBlock4 = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock4, "Plane ZX");
        }
    }
}
