using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace EventsLibrary
{
    public class IfThenEventFilter : BaseComp
    {
        private bool _condition = false;
        
        public IfThenEventFilter() : base()
        {
        }
        public IfThenEventFilter(int x, int y) : base(x, y)
        {
            this.previewTextBlock.DisplayedText = _condition.ToString();
        }

        public override void Compute()
        {
            _condition = this.ChildElementManager.GetData(condition, false);
            this.previewTextBlock.DisplayedText = _condition.ToString();
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "If Then", "Filters", "Events");
        
        private GenericEventNode eventIn;
        private BooleanDataNode condition;
        private GenericEventNode eventTrue;
        private GenericEventNode eventFalse;

        public override void Initialize()
        {
            eventIn = new GenericEventNode(this, NodeType.Input);
            eventIn.NodeEvent += eventIn_NodeEvent;
            this.ChildElementManager.AddEventInputNode(eventIn);

            condition = new BooleanDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(condition, "Condition");

            eventTrue = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(eventTrue, "True");

            eventFalse = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(eventFalse, "False");
        }
        private EventArgData lastEvArgs;
        private void eventIn_NodeEvent(IEventNode container, EventArgData e)
        {
            lastEvArgs = e;
            ComputationCore.Compute(this, false);
            if (_condition)
            {
                eventTrue.EventOccured(e);
            }
            else
            {
                eventFalse.EventOccured(e);
            }
        }
    }
}
