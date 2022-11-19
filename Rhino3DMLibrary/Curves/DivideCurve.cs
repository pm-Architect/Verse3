using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;


namespace Rhino3DMLibrary
{
    public class DivideCurve : BaseComp
    {
        public DivideCurve() : base()
        {
        }
        public DivideCurve(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Rhino.Geometry.Curve curve = (Rhino.Geometry.Curve)this.ChildElementManager.GetData<GeometryBase>(nodeBlockX, default);

            int count = (int)this.ChildElementManager.GetData(nodeBlockY, 5);
            bool includeEnds = this.ChildElementManager.GetData(nodeBlockZ, true);
            if (curve != null)
            {

                //TODO: DivideByCount in r3dm
                GeometryBase geo = curve;
                this.ChildElementManager.SetData<GeometryBase>(geo, nodeBlockResult);
            }

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Divide Curve", "Operations", "Curve");

        private RhinoGeometryDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;
        private BooleanDataNode nodeBlockZ;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Curve");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Segment count");

            nodeBlockZ = new BooleanDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockZ, "Include Ends");


            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Divided curve", true);
        }
    }
}
