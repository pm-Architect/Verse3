using Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace EventsLibrary
{
    public class Wait : BaseComp
    {
        private int _duration = 1000;
        private int _counter = 0;
        private bool _gateCollateMode = true;

        private Task _gateTask;

        #region Constructors

        public Wait() : base()
        {
        }
        public Wait(int x, int y) : base(x, y)
        {
            this.previewTextBlock.DisplayedText = $"Duration: {_duration} ms\nCounter: {_counter}";
        }

        #endregion
        
        public override void Compute()
        {
            this.previewTextBlock.DisplayedText = $"Duration: {_duration} ms\nCounter: {_counter}";
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Wait", "Timing", "Events");

        private GenericEventNode eventIn;
        private NumberDataNode duration;
        private GenericEventNode eventOut;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            eventIn = new GenericEventNode(this, NodeType.Input);
            eventIn.NodeEvent += NodeBlock_NodeEvent;
            this.ChildElementManager.AddEventInputNode(eventIn);

            duration = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(duration, "Duration");

            eventOut = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(eventOut);
        }

        private void NodeBlock_NodeEvent(IEventNode container, EventArgData e)
        {
            _gateTask = Task.Delay(_duration);
            _gateTask.ContinueWith((_gateTask) => Wait_Completed(_gateTask, e));
        }

        private void Wait_Completed(Task t, EventArgData e)
        {
            switch (t.Status)
            {
                case TaskStatus.RanToCompletion:
                    {
                        _counter++;
                        if (_gateCollateMode)
                        {
                            if (_gateTask != t)
                            {
                                break;
                            }
                        }
                        eventOut.EventOccured(e);
                        ComputationCore.Compute(this, false);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
