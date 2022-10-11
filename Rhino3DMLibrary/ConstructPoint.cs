using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    [Serializable]
    public class ConstructPoint : BaseComp
    {
        public ConstructPoint() : base(0, 0)
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
            Point3d point = new Point3d(x, y, z);
            //Box box = new Box(Plane.WorldXY, new Interval(-(x / 2), (x / 2)), new Interval(-(y / 2), (y / 2)), new Interval(-(z / 2), (z / 2)));
            GeometryBase geo = new Rhino.Geometry.Point(point);
            this.ChildElementManager.SetData<GeometryBase>(geo, 0);
            textBlock.DisplayedText = point.ToString();
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Construct Point",
                Group = "Point",
                Tab = "Vector",
                Description = "",
                Author = "",
                License = "",
                Repository = "",
                Version = "",
                Website = ""
            };
            return ci;
        }

        private TextElement textBlock = new TextElement();
        private NumberDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;
        private NumberDataNode nodeBlockZ;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "X");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Y");

            nodeBlockZ = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockZ, "Z");

            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Point");

            textBlock = new TextElement();
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.DisplayedText = "";
            this.ChildElementManager.AddElement(textBlock);
        }
    }

    [Serializable]
    public class RhinoGeometryDataNode : DataNodeElement<GeometryBase>
    {
        public RhinoGeometryDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
}
