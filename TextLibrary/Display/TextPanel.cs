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
            DataStructure data = this.ChildElementManager.GetData(nodeBlock);
            if (data is null)
            {
                textBlock.DisplayedText = "<null>";
                this.ChildElementManager.AdjustBounds(true);
                return;
            }
            else
            {
                switch (data.DataType)
                {
                    case Type t when t == typeof(string):
                        textBlock.DisplayedText = (string)data.Data;
                        break;
                    case Type t when t == typeof(int):
                        textBlock.DisplayedText = ((int)data.Data).ToString();
                        break;
                    case Type t when t == typeof(double):
                        textBlock.DisplayedText = ((double)data.Data).ToString();
                        break;
                    case Type t when t == typeof(float):
                        textBlock.DisplayedText = ((float)data.Data).ToString();
                        break;
                    case Type t when t == typeof(bool):
                        textBlock.DisplayedText = ((bool)data.Data).ToString();
                        break;
                    case Type t when t == typeof(DateTime):
                        textBlock.DisplayedText = ((DateTime)data.Data).ToString();
                        break;
                    default:
                        {
                            try
                            {
                                if (data.Count > 0)
                                {
                                    textBlock.DisplayedText = "[";
                                    foreach (IDataGoo goo in data)
                                    {
                                        textBlock.DisplayedText += (goo.Data.ToString() + ", ");
                                    }
                                    textBlock.DisplayedText += "]";
                                }
                                else
                                {
                                    textBlock.DisplayedText = data.ToString();
                                }
                            }
                            catch (Exception ex)
                            {
                                textBlock.DisplayedText = ex.Message;
                            }
                            break;
                        }
                }
                //this.ChildElementManager.AdjustBounds(true);
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
