using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using TextElement = Verse3.VanillaElements.TextElement;

namespace MathLibrary
{
    public class RandomNumber : BaseComp
    {
        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = "";
                if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                    dataIN = (Math.Round((((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data), 2)).ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                
                return $"Output Value: {dataIN}";
            }
        }

        #region Properties


        #endregion

        #region Constructors

        public RandomNumber() : base(0, 0)
        {
        }

        public RandomNumber(int x, int y, int width = 250, int height = 350) : base(x, y)
        {
        }

        #endregion

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData<double>(0, 100);
            double b = this.ChildElementManager.GetData<double>(1, 0);
            Random rd = new Random();
            this.ChildElementManager.SetData<double>((double)rd.Next(), 0);
            this.ChildElementManager.SetData<double>(rd.Next((int)b, (int)a), 1);
            textBlock.DisplayedText = this.ElementText;
        }


        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Random",
                Group = "Miscellaneous",
                Tab = "Math",
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
        private NumberDataNode nodeBlock1;
        private NumberDataNode nodeBlock2;
        private NumberDataNode nodeBlock3;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Maximum");
            
            nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            nodeBlock1.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Minimum");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "True Random");

            nodeBlock3 = new NumberDataNode(this, NodeType.Output);
            nodeBlock3.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Random between the limits");

            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
