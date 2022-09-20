using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace RhinoCommonLibrary
{
    public class MakeCube : BaseComp
    {
        public MakeCube() : base(0, 0)
        {
        }
        public MakeCube(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            //Box box = new Box(0, 0, 0, 10, 10, 10);
            //CoreRhinoInteropHelper c = new CoreRhinoInteropHelper();
            //textBlock.DisplayedText = c.Info;
            //this.GetType().Assembly.
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Test",
                Group = "Rhino",
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
        private NumberDataNode nodeBlock;
        //private NumberDataNode nodeBlock1;
        private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            //nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "A");

            //nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            ////nodeBlock1.Width = 50;
            //this.ChildElementManager.AddDataInputNode(nodeBlock1, "B");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            //nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result");

            textBlock = new TextElement();
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.DisplayedText = "NA";
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
