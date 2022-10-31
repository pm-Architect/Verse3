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
            GeometryBase geo = this.ChildElementManager.GetData<GeometryBase>(nodeBlockResult, default);
            if (geo != null)
            {
                if (geo is Rhino.Geometry.Point pt)
                {
                    Point3d p = pt.Location;
                    this.ChildElementManager.SetData<double>(p.X, nodeBlockX);
                    this.ChildElementManager.SetData<double>(p.Y, nodeBlockY);
                    this.ChildElementManager.SetData<double>(p.Z, nodeBlockZ);
                    this.previewTextBlock.DisplayedText = ($"({p.X}, {p.Y}, {p.Z})");
                }
                else return;
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
