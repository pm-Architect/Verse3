using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class RotatePlane : BaseComp
    {
        public RotatePlane() : base()
        {
        }
        public RotatePlane(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            ((Rhino.Geometry.PlaneSurface)this.ChildElementManager.GetData<GeometryBase>(nodeBlock1, default)).TryGetPlane(out Rhino.Geometry.Plane plane);
            double angledegreesZ = this.ChildElementManager.GetData<double>(nodeBlock2, 0);
            double angledegreesY = this.ChildElementManager.GetData<double>(nodeBlock3, 0);
            double angledegreesX = this.ChildElementManager.GetData<double>(nodeBlock4, 0);

            angledegreesZ *= Math.PI/180;
            angledegreesY *= Math.PI/180;
            angledegreesX *= Math.PI/180;

            if (plane.IsValid)
            {
                plane.Rotate(angledegreesZ, plane.ZAxis);
                plane.Rotate(angledegreesY, plane.YAxis);
                plane.Rotate(angledegreesX, plane.XAxis);
                GeometryBase geo3 = new Rhino.Geometry.PlaneSurface(plane, new Interval(-10.0, 10.0), new Interval(-10.0, 10.0));
                this.ChildElementManager.SetData<GeometryBase>(geo3, nodeBlock5);
            }


        }


        public override CompInfo GetCompInfo() => new CompInfo(this, "Rotate Plane", "Operations", "Surface");

        private RhinoGeometryDataNode nodeBlock1;
        private NumberDataNode nodeBlock2;
        private NumberDataNode nodeBlock3;
        private NumberDataNode nodeBlock4;
        private RhinoGeometryDataNode nodeBlock5;


        public override void Initialize()
        {
            nodeBlock1 = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Plane");

            nodeBlock2 = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock2, "Rotation Z");

            nodeBlock3 = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock3, "Rotation Y");

            nodeBlock4 = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock4, "Rotation X");

            nodeBlock5 = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock5, "Rotated Plane", true);
        }
    }
}
