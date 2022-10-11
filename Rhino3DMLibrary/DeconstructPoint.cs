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
    public class DeconstructPoint : BaseComp
    {
        public DeconstructPoint() : base(0, 0)
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
                textBlock.DisplayedText = point.ToString();
            }
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Deconstruct Point",
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
            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockResult, "Point");
            
            nodeBlockX = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockX, "X");

            nodeBlockY = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockY, "Y");

            nodeBlockZ = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockZ, "Z");

            textBlock = new TextElement();
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.DisplayedText = "";
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
