using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructPoint : BaseComp
    {
        public ConstructPoint() : base()
        {
        }
        public ConstructPoint(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            double x = this.ChildElementManager.GetData<double>(nodeBlockX, 10);
            double y = this.ChildElementManager.GetData<double>(nodeBlockY, 10);
            double z = this.ChildElementManager.GetData<double>(nodeBlockZ, 10);
            Point3d point1 = new Point3d(x, y, z);
            
            GeometryBase geo = new Rhino.Geometry.Point(point1);
            this.ChildElementManager.SetData<GeometryBase>(geo, nodeBlockResult1);

            Random random = new Random();
            Point3d point2 = new Point3d(random.Next(-100, 100), random.Next(-100, 100), random.Next(-100, 100));
            GeometryBase geo2 = new Rhino.Geometry.Point(point2);
            this.ChildElementManager.SetData<GeometryBase>(geo2, nodeBlockResult2);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Point", "Point", "Vector");

        private NumberDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;
        private NumberDataNode nodeBlockZ;
        private RhinoGeometryDataNode nodeBlockResult1;
        private RhinoGeometryDataNode nodeBlockResult2;
        public override void Initialize()
        {
            nodeBlockX = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "X");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Y");

            nodeBlockZ = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockZ, "Z");

            nodeBlockResult1 = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult1, "Generated Point", true);

            nodeBlockResult2 = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult2, "Random Point");
        }
    }

    public class RhinoGeometryDataNode : DataNodeElement<GeometryBase>
    {
        public RhinoGeometryDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
}
