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
            double x = this.ChildElementManager.GetData<double>(0, 10);
            double y = this.ChildElementManager.GetData<double>(1, 10);
            double z = this.ChildElementManager.GetData<double>(2, 10);
            Point3d point1 = new Point3d(x, y, z);
            //Box box = new Box(Plane.WorldXY, new Interval(-(x / 2), (x / 2)), new Interval(-(y / 2), (y / 2)), new Interval(-(z / 2), (z / 2)));
            GeometryBase geo = new Rhino.Geometry.Point(point1);
            this.ChildElementManager.SetData<GeometryBase>(geo, 0);

            Random random = new Random();
            Point3d point2 = new Point3d(random.Next(-100, 100), random.Next(-100, 100), random.Next(-100, 100));
            GeometryBase geo2 = new Rhino.Geometry.Point(point2);
            this.ChildElementManager.SetData<GeometryBase>(geo2, 1);
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
