using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructLine : BaseComp
    {
        public ConstructLine() : base()
        {
        }
        public ConstructLine(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Rhino.Geometry.Point point1 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(0);
            Rhino.Geometry.Point point2 = (Rhino.Geometry.Point)this.ChildElementManager.GetData<GeometryBase>(1);
            if (point1 != null && point2 != null)
            {
                Line line = new Line(point1.Location, point2.Location);
                GeometryBase geo = new Rhino.Geometry.LineCurve(line);
                this.ChildElementManager.SetData<GeometryBase>(geo, 0);
            }

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Line", "Line", "Curve");

        private RhinoGeometryDataNode nodeBlockX;
        private RhinoGeometryDataNode nodeBlockY;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Start");

            nodeBlockY = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "End");

            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Line", true);
        }
    }
}
