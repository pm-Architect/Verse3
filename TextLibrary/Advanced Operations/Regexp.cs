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

        public Regexp() : base()
        {

        }

        public Regexp(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override void Compute()
        {
            string a = this.ChildElementManager.GetData<string>(nodeBlock, "Some very random text");
            string b = this.ChildElementManager.GetData<string>(nodeBlock1, "No pattern");
            string pattern = @$"{b}";

            this.ChildElementManager.SetData<bool>(Regex.Match(a, pattern).Success, nodeBlock2);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Regex", "Advanced Operations", "Text");

        private TextDataNode nodeBlock;
        private TextDataNode nodeBlock1;
        private BooleanDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new TextDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Text");

            nodeBlock1 = new TextDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Regex pattern");

            nodeBlock2 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result", true);
        }
    }
}
