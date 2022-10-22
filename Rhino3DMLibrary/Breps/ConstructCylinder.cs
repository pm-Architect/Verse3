using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructCylinder : BaseComp
    {
        public ConstructCylinder() : base()
        {
        }
        public ConstructCylinder(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            ((Rhino.Geometry.ArcCurve)this.ChildElementManager.GetData<GeometryBase>(0)).TryGetCircle(out Circle circle); 
            double height = this.ChildElementManager.GetData<double>(1, 50);

            if (circle.IsValid)
            {
                Cylinder cylinder = new Cylinder(circle, height);
                GeometryBase geo = cylinder.ToNurbsSurface();
                this.ChildElementManager.SetData<GeometryBase>(geo, 0);
            }

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Cylinder", "Basic", "Breps");

        private RhinoGeometryDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;

        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Circle");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Height");

            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Cylinder", true);
        }
    }
}
