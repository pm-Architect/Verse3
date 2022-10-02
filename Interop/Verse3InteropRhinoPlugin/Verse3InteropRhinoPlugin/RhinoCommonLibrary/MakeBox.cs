using Core;
using CoreInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;
using Verse3RhinoInterop;

namespace RhinoCommonLibrary
{
    public class MakeBox : BaseComp
    {
        public MakeBox() : base(0, 0)
        {
        }
        public MakeBox(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            double x = this.ChildElementManager.GetData<double>(0, 10);
            double y = this.ChildElementManager.GetData<double>(1, 10);
            double z = this.ChildElementManager.GetData<double>(2, 10);
            Box box = new Box(Plane.WorldXY, new Interval(-(x / 2), (x / 2)), new Interval(-(y / 2), (y / 2)), new Interval(-(z / 2), (z / 2)));
            GeometryBase geo = (GeometryBase)box.ToBrep();
            this.ChildElementManager.SetData<RhinoGeometryWrapper>(new RhinoGeometryWrapper(geo), 0);
            textBlock.DisplayedText = box.ToString();
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Make Box",
                Group = "Geometry",
                Tab = "Rhino",
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
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Box");

            textBlock = new TextElement();
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.DisplayedText = "NA";
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
