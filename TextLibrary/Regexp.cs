using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using System.Text.RegularExpressions;

namespace MathLibrary
{
    public class Regexp : BaseComp
    {
        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = "";
                if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                    dataIN = ((BooleanDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data.ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                
                return $"Output Value: {dataIN}";
            }
        }

        #region Properties


        #endregion

        #region Constructors

        public Regexp() : base(0, 0)
        {

        }

        public Regexp(int x, int y, int width = 250, int height = 350) : base(x, y)
        {
            //base.boundingBox = new BoundingBox(x, y, width, height);


        }

        #endregion

        public override void Compute()
        {
            string a = this.ChildElementManager.GetData<string>(0, "Some very random text");
            string b = this.ChildElementManager.GetData<string>(1, "No pattern");
            string pattern = @$"{b}";

            this.ChildElementManager.SetData<bool>(Regex.Match(a, pattern).Success, 0);
            textBlock.DisplayedText = this.ElementText;
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Regex",
                Group = "Advanced Operations",
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
        private TextDataNode nodeBlock1;
        private BooleanDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new TextDataNode(this, NodeType.Input);
            nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Text");

            nodeBlock1 = new TextDataNode(this, NodeType.Input);
            nodeBlock1.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Regex pattern");

            nodeBlock2 = new BooleanDataNode(this, NodeType.Output);
            nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result");

            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
