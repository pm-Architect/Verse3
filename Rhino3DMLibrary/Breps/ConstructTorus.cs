using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructTorus : BaseComp
    {
        public ConstructTorus() : base()
        {
        }
        public ConstructTorus(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            GeometryBase geoPlane = this.ChildElementManager.GetData<GeometryBase>(0, ((new PlaneSurface(Plane.WorldXY, new Interval(-10, 10), new Interval(-10, 10)) as GeometryBase)));
            if (geoPlane is null) return;
            Plane plane = Plane.WorldXY;
            if (geoPlane is PlaneSurface planeSrf)
            {
                if (!planeSrf.TryGetPlane(out plane))
                {
                    //return;
                }
            }
            //else return;
            double majorradius = this.ChildElementManager.GetData<double>(1, 50);
            double minorradius = this.ChildElementManager.GetData<double>(2, 10);
            if (plane.IsValid)
            {
                Torus torus = new Torus(plane, majorradius, minorradius);
                GeometryBase geo = torus.ToNurbsSurface();
                this.ChildElementManager.SetData<GeometryBase>(geo, 0);
            }
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Torus", "Basic", "Breps");

        private RhinoGeometryDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;
        private NumberDataNode nodeBlockZ;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Plane");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Major Radius");

            nodeBlockZ = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockZ, "Minor Radius");


            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Torus", true);
        }
    }
}
