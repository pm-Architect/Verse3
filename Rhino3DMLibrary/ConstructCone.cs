using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructCone : BaseComp
    {
        public ConstructCone() : base(0, 0)
        {
        }
        public ConstructCone(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            ((Rhino.Geometry.PlaneSurface)this.ChildElementManager.GetData<GeometryBase>(0)).TryGetPlane(out Rhino.Geometry.Plane plane);
            double height = this.ChildElementManager.GetData<double>(1, 0);
            double radius = this.ChildElementManager.GetData<double>(2, 0);
            if (plane.IsValid)
            {
                Cone cone= new Cone(plane, height, radius);
                GeometryBase geo = cone.ToNurbsSurface();
                this.ChildElementManager.SetData<GeometryBase>(geo, 0);
                textBlock.DisplayedText = cone.ToString();
            }

        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Construct Cone",
                Group = "Basic",
                Tab = "Breps",
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
        private RhinoGeometryDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;
        private NumberDataNode nodeBlockZ;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Plane");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Height");

            nodeBlockZ = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockZ, "Radius");


            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Cone");

            textBlock = new TextElement();
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.DisplayedText = "";
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
