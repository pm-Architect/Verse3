using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class TextLength : BaseComp
    {
        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = "";
                if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                    dataIN = ((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data.ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                
                return $"Output Value: {dataIN}";
            }
        }

        #region Properties


        #endregion

        #region Constructors

        public TextLength() : base(0, 0)
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public TextLength(int x, int y, int width = 250, int height = 350) : base(x, y)
        {
            //base.boundingBox = new BoundingBox(x, y, width, height);

            //Random rnd = new Random();
            //byte rc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            //byte gc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            //byte bc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            //this.BackgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
            //this.Background = new SolidColorBrush(Colors.Gray);
        }

        #endregion

        public override void Compute()
        {
            string a = this.ChildElementManager.GetData<string>(0, "");
            double len = (double)a.Length;
            this.ChildElementManager.SetData<double>(len, 0);
            textBlock.DisplayedText = this.ElementText;
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Text Length",
                Group = "Operations",
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

        private TextElement textBlock = new TextElement();
        private TextDataNode nodeBlock;
 
        private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new TextDataNode(this, NodeType.Input);
            nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Text");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Length");

            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
