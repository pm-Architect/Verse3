using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class DeconstructPoint : BaseComp
    {
        public DeconstructPoint() : base()
        {
        }
        public DeconstructPoint(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            GeometryBase point = this.ChildElementManager.GetData<GeometryBase>(0);
            if (point != null)
            {
                if (point is Rhino.Geometry.Point)
                {
                    Point3d p = ((Rhino.Geometry.Point)point).Location;
                    this.ChildElementManager.SetData<double>(p.X, 0);
                    this.ChildElementManager.SetData<double>(p.Y, 1);
                    this.ChildElementManager.SetData<double>(p.Z, 2);
                }
                else return;
                this.previewTextBlock.DisplayedText = point.ToString();
            }
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Deconstruct Point", "Point", "Vector");

        private NumberDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;
        private NumberDataNode nodeBlockZ;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockResult, "Point");
            
            nodeBlockX = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockX, "X");

            nodeBlockY = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockY, "Y");

            nodeBlockZ = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockZ, "Z");
        }
    }
}
