using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace EventsLibrary
{
    public class Flipflop : BaseComp
    {
        private bool _state = false;
        
        public Flipflop() : base()
        {
        }
        public Flipflop(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Flipflop", "Filters", "Events");
        
        private GenericEventNode eventIn;
        private GenericEventNode eventTrue;
        private GenericEventNode eventFalse;
        private BooleanDataNode state;
        public override void Initialize()
        {
            eventIn = new GenericEventNode(this, NodeType.Input);
            eventIn.NodeEvent += eventIn_NodeEvent;
            this.ChildElementManager.AddEventInputNode(eventIn);

            eventTrue = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(eventTrue, "True");

            eventFalse = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(eventFalse, "False");

            state = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(state, "State", true);
        }

        private void eventIn_NodeEvent(IEventNode container, EventArgData e)
        {
            _state = !_state;
            this.previewTextBlock.DisplayedText = _state.ToString();
            if (_state)
            {
                eventTrue.EventOccured(e);
            }
            else
            {
                eventFalse.EventOccured(e);
            }
            this.ChildElementManager.SetData<bool>(_state, state);
            ComputationCore.Compute(this, false);
        }
    }
}
