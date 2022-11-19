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
        public TextPanel() : base()
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
                this.previewTextBlock.DisplayedText = "<null>";
                this.ChildElementManager.AdjustBounds(true);
                return;
            }
            else
            {
                switch (data.DataType)
                {
                    case Type t when t == typeof(string):
                        this.previewTextBlock.DisplayedText = (string)data.Data;
                        break;
                    case Type t when t == typeof(int):
                        this.previewTextBlock.DisplayedText = ((int)data.Data).ToString();
                        break;
                    case Type t when t == typeof(double):
                        this.previewTextBlock.DisplayedText = ((double)data.Data).ToString();
                        break;
                    case Type t when t == typeof(float):
                        this.previewTextBlock.DisplayedText = ((float)data.Data).ToString();
                        break;
                    case Type t when t == typeof(bool):
                        this.previewTextBlock.DisplayedText = ((bool)data.Data).ToString();
                        break;
                    case Type t when t == typeof(DateTime):
                        this.previewTextBlock.DisplayedText = ((DateTime)data.Data).ToString();
                        break;
                    default:
                        {
                            try
                            {
                                if (data.Count > 0)
                                {
                                    this.previewTextBlock.DisplayedText = "[";
                                    foreach (IDataGoo goo in data)
                                    {
                                        this.previewTextBlock.DisplayedText += (goo.Data.ToString() + ", ");
                                    }
                                    this.previewTextBlock.DisplayedText = this.previewTextBlock.DisplayedText.TrimEnd(',', ' ');
                                    this.previewTextBlock.DisplayedText += "]";
                                }
                                else
                                {
                                    this.previewTextBlock.DisplayedText = data.ToString();
                                }
                            }
                            catch (Exception ex)
                            {
                                CoreConsole.Log(ex);
                                this.previewTextBlock.DisplayedText = ex.Message;
                            }
                            break;
                        }
                }
                this.ChildElementManager.AdjustBounds(false);
            }
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Text Panel", "Display", "Text");

        internal GenericDataNode nodeBlock;
        public override void Initialize()
        {
            nodeBlock = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Data");
        }
    }
}
