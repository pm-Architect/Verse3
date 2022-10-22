using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructSphere : BaseComp
    {
        public ConstructSphere() : base()
        {
        }
        public ConstructSphere(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Rhino.Geometry.Point point1 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(0);
            double radius = this.ChildElementManager.GetData<double>(1, 0);
            if (point1 != null)
            {
                Sphere sphere = new Sphere(point1.Location, radius);
                GeometryBase geo = sphere.ToBrep();
                this.ChildElementManager.SetData<GeometryBase>(geo, 0);
            }

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Sphere", "Basic", "Breps");

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
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Sphere", true);
        }
    }
}
