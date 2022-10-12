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
        public TextPanel() : base(0, 0)
        {
        }
        public TextPanel(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            dynamic data = this.ChildElementManager.GetData<dynamic>(0, null);
            if (data is null)
            {
                textBlock.DisplayedText = "<null>";
                return;
            }
            switch (data.GetType())
            {
                case Type t when t == typeof(string):
                    textBlock.DisplayedText = (string)data;
                    break;
                case Type t when t == typeof(int):
                    textBlock.DisplayedText = ((int)data).ToString();
                    break;
                case Type t when t == typeof(double):
                    textBlock.DisplayedText = ((double)data).ToString();
                    break;
                case Type t when t == typeof(float):
                    textBlock.DisplayedText = ((float)data).ToString();
                    break;
                case Type t when t == typeof(bool):
                    textBlock.DisplayedText = ((bool)data).ToString();
                    break;
                case Type t when t == typeof(DateTime):
                    textBlock.DisplayedText = ((DateTime)data).ToString();
                    break;
                default:
                    {
                        try
                        {
                            textBlock.DisplayedText = data.ToString();
                        }
                        catch (Exception ex)
                        {
                            textBlock.DisplayedText = ex.Message;
                        }
                        break;
                    }
            }
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
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
        internal GenericDataNode nodeBlock;
        public override void Initialize()
        {
            nodeBlock = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Data");
            
            textBlock = new TextElement();
            textBlock.DisplayedText = "<null>";
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
