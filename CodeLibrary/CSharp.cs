using Core;
//using EventsLibrary;
using System;
using System.Windows;
using VanillaElements;
using Verse3;
using Verse3.VanillaElements;

namespace CodeLibrary
{
    public class CSharp : BaseComp
    {

        #region Properties

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = ideElement.Script;
                //if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                //    dataIN = (Math.Round((((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data), 2)).ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                //TODO: Z Index control for IRenderable
                return $"Name: {name}" +
                    $"\nView: {viewname}" +
                    $"\nID: {this.ID}" +
                    $"\nX: {this.X}" +
                    $"\nY: {this.Y}" +
                    $"\nOutput Value: {dataIN}";
            }
        }

        #endregion

        #region Constructors

        public CSharp() : base(0, 0)
        {
        }

        public CSharp(int x, int y, int width = 250, int height = 350) : base(x, y)
        {
        }

        #endregion

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "CSharp",
                Group = "CSharp",
                Tab = "Code",
                Description = "",
                Author = "",
                License = "",
                Repository = "",
                Version = "",
                Website = ""
            };
            return ci;
        }

        public override void Compute()
        {
            //double a = this.ChildElementManager.GetData<double>(0, 0);
            //double b = this.ChildElementManager.GetData<double>(1, 0);
            //this.ChildElementManager.SetData<double>((a + b), 0);
            textBlock.DisplayedText = this.ElementText;
        }
        
        private TextElement textBlock = new TextElement();
        private IDEElement ideElement = new IDEElement();
        internal ButtonElement buttonBlock = new ButtonElement();
        //private ButtonClickedEventNode nodeBlock;
        //private NumberDataNode nodeBlock1;
        //private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            //nodeBlock = new NumberDataNode(this, NodeType.Input);
            ////nodeBlock.Width = 50;
            //this.ChildElementManager.AddDataInputNode(nodeBlock, "A");

            //nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            ////nodeBlock1.Width = 50;
            //this.ChildElementManager.AddDataInputNode(nodeBlock1, "B");

            //nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            ////nodeBlock2.Width = 50;
            //this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result");
            
            //nodeBlock = new ButtonClickedEventNode(this, NodeType.Input);
            //nodeBlock.Width = 50;
            //nodeBlock.NodeEvent += NodeBlock_NodeEvent;
            //this.ChildElementManager.AddEventInputNode(nodeBlock as IEventNode, "Compile");

            ideElement = new IDEElement();
            ideElement.Width = 600;
            ideElement.Height = 350;
            this.ChildElementManager.AddElement(ideElement);

            buttonBlock = new ButtonElement();
            buttonBlock.DisplayedText = "Trigger";
            buttonBlock.OnButtonClicked += ButtonBlock_OnButtonClicked;
            buttonBlock.Width = 200;
            this.ChildElementManager.AddElement(buttonBlock);

            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }

        //private void NodeBlock_NodeEvent(IEventNode container, EventArgData e)
        //{
        //    this.Compute();
        //}

        private void ButtonBlock_OnButtonClicked(object? sender, RoutedEventArgs e)
        {
            this.Compute();
        }
    }
}
