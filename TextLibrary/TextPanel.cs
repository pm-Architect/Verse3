using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace TextLibrary
{
    public class TextPanel : BaseComp
    {
        public TextPanel(int x, int y, int width = 250, int height = 300) : base(x, y)
        {
        }

        public override void Compute()
        {
            //throw new NotImplementedException();
            this.ComputationPipelineInfo.IOManager.GetData<string>(out string data, 0);
            textBlock.DisplayedText = data;
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Text Panel",
                Group = "Display",
                Tab = "Text",
                Description = "",
                Author = "",
                License = "",
                Repository = "",
                Version = "",
                Website = ""
            };
            return ci;
        }

        internal TextElement textBlock = new TextElement();
        internal TextDataNode nodeBlock;
        public override void Initialize()
        {
            nodeBlock = new TextDataNode(this, NodeType.Input);
            nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Text");
            
            textBlock = new TextElement();
            textBlock.DisplayedText = "";
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
