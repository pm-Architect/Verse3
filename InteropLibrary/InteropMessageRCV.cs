using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace InteropLibrary
{
    public class InteropMessageRCV : BaseComp
    {
        internal string _lastMessage = "";
        //private double _inputValue = 0.0;

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = _lastMessage;
                //if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                //dataIN = ((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0])?.DataGoo.Data.ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                //TODO: Z Index control for IRenderable
                return $"Name: {name}" +
                    $"\nView: {viewname}" +
                    $"\nID: {this.ID}" +
                    $"\nX: {this.X}" +
                    $"\nY: {this.Y}" +
                    $"\nLast Message: {dataIN}";
            }
        }

        #region Properties


        #endregion

        #region Constructors

        public InteropMessageRCV() : base(0, 0)
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public InteropMessageRCV(int x, int y, int width = 250, int height = 100) : base(x, y)
        {
            //base.boundingBox = new BoundingBox(x, y, width, height);

            //Random rnd = new Random();
            //byte rc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //byte gc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //byte bc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //this.BackgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
            //this.Background = new SolidColorBrush(Colors.Gray);
        }

        #endregion

        public override void Compute()
        {
            //this.ComputationPipelineInfo.IOManager.SetData<double>(_sliderValue, 0);
            //if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count == 1)
            //{
            //    if (this.ComputationPipelineInfo.IOManager.DataOutputNodes[0] is NodeElement)
            //        ((NodeElement)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data = _sliderValue;
            //}
        }
        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Interop Message Receiver",
                Group = "Events",
                Tab = "Interop",
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
        //internal ButtonElement buttonBlock = new ButtonElement();
        internal InteropMessageEventNode nodeBlock;
        internal NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            //buttonBlock = new ButtonElement();
            //buttonBlock.DisplayedText = "Trigger";
            //buttonBlock.OnButtonClicked += ButtonBlock_OnButtonClicked;
            //buttonBlock.Width = 200;
            //this.ChildElementManager.AddElement(buttonBlock);
            CoreInterop.InteropServer._LocalInteropServer.ClientMessage += _LocalInteropServer_ClientMessage;

            nodeBlock = new InteropMessageEventNode(this, NodeType.Output);
            nodeBlock.Width = 50;
            this.ChildElementManager.AddEventOutputNode(nodeBlock as IEventNode);

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataOutputNode<double>(nodeBlock2 as IDataNode<double>);

            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }

        private void _LocalInteropServer_ClientMessage(object? sender, DataStructure e)
        {
            this.ComputationPipelineInfo.IOManager.EventOccured(0, new EventArgData(e));
            _lastMessage = e.ToString();
            textBlock.DisplayedText = this.ElementText;
            if (double.TryParse(e.Data.ToString(), out double num))
                this.ComputationPipelineInfo.IOManager.SetData<double>(num, 0);
            //ComputationPipeline.ComputeComputable(this);
        }

        //private IRenderable _parent;
        //public IRenderable Parent => _parent;
        //private ElementsLinkedList<IRenderable> _children = new ElementsLinkedList<IRenderable>();
        //public ElementsLinkedList<IRenderable> Children => _children;
    }
}
