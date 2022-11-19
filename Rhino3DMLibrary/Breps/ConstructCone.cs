using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructCone : BaseComp
    {
        public ConstructCone() : base()
        {
        }
        public ConstructCone(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            ((Rhino.Geometry.PlaneSurface)this.ChildElementManager.GetData<GeometryBase>(nodeBlockX, default)).TryGetPlane(out Rhino.Geometry.Plane plane);
            double height = this.ChildElementManager.GetData<double>(nodeBlockY, 0);
            double radius = this.ChildElementManager.GetData<double>(nodeBlockZ, 0);
            if (plane.IsValid)
            {
                Cone cone= new Cone(plane, height, radius);
                GeometryBase geo = cone.ToNurbsSurface();
                this.ChildElementManager.SetData<GeometryBase>(geo, nodeBlockResult);
            }

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Cone", "Basic", "Breps");

        private RhinoGeometryDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;
        private NumberDataNode nodeBlockZ;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Plane");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Height");

            nodeBlockZ = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockZ, "Radius");


            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Cone", true);
        }
    }
}
