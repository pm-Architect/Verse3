using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class CreateExtrusion : BaseComp
    {
        public CreateExtrusion() : base()
        {
        }
        public CreateExtrusion(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            Rhino.Geometry.Curve curve = (Rhino.Geometry.Curve)this.ChildElementManager.GetData<GeometryBase>(0);

            double height = this.ChildElementManager.GetData<double>(1, 50);
            bool cap = this.ChildElementManager.GetData<bool>(2, true);
            if (curve != null)
            {

                GeometryBase geo = Extrusion.Create(curve, height, cap);
                this.ChildElementManager.SetData<GeometryBase>(geo, 0);
            }

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Create Extrusion", "Operations", "Curve");

        private RhinoGeometryDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;
        private BooleanDataNode nodeBlockZ;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Curve");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Height");

            nodeBlockZ = new BooleanDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockZ, "Is Capped");


            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Extruded surface", true);
        }
    }
}
