using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace EventsLibrary
{
    public class EventIndicator : BaseComp
    {
        internal int _counter = 0;
        internal string _argstring = "";

        #region Constructors

        public EventIndicator() : base()
        {
        }
        public EventIndicator(int x, int y) : base(x, y)
        {
            this.previewTextBlock.DisplayedText = $"Counter: {_counter}\nArgs: {_argstring}";
        }

        #endregion

        public override void Compute()
        {
            this.previewTextBlock.DisplayedText = $"Counter: {_counter}\nArgs: {_argstring}";
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Event Indicator", "Event Utilities", "Events");
        
        internal GenericEventNode nodeBlock;
        internal GenericEventNode nodeBlock1;
        internal GenericDataNode nodeBlock2;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            nodeBlock = new GenericEventNode(this, NodeType.Input);
            nodeBlock.NodeEvent += NodeBlock_NodeEvent;
            this.ChildElementManager.AddEventInputNode(nodeBlock);

            nodeBlock1 = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(nodeBlock1);

            nodeBlock2 = new GenericDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Args", true);
        }

        private void NodeBlock_NodeEvent(IEventNode container, EventArgData e)
        {
            _counter++;
            nodeBlock1.EventOccured(e);
            if (e.Count > 0)
            {
                _argstring = e[0].Data.ToString();
                object dataOut = e[0].Data;
                this.ChildElementManager.SetData(dataOut, 0);
            }
            ComputationCore.Compute(this, false);
        }
    }
}
