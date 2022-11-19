using Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Verse3;
using Verse3.VanillaElements;

namespace EventsLibrary
{
    public class Callback : BaseComp
    {
        private BaseComp _callbackToComp;
        
        public Callback() : base()
        {
        }
        public Callback(int x, int y, BaseComp callbackToComp) : base(x, y)
        {
            if (callbackToComp != null)
            {
                _callbackToComp = callbackToComp;
                if (_callbackToComp is Loop loop)
                {
                    loop.CallbackComp = this;
                }
            }
            else
            {
                Exception ex = new Exception("Callback must be initialized with a valid BaseComp");
                CoreConsole.Log(ex);
            }
        }
        
        public override void Compute()
        {
            //_loopData = this.ChildElementManager.GetData(LoopDataNode);
            try
            {
                if (_eventCallbackNodeFromcbComp != null && _callbackToComp != null)
                {
                    if (_eventCallbackNodeFromcbComp.Connections.Count != 1)
                    {
                        _eventCallbackNodeFromcbComp.Connections.Clear();
                        _eventCallbackNodeFromcbComp.RenderPipelineInfo.Children.Clear();
                        if (_callbackToComp.ChildElementManager.OutputSide.OfType<EventCallbackNode>().Any())
                        {
                            _eventCallbackNodeFromcbComp = _callbackToComp.ChildElementManager.OutputSide.OfType<EventCallbackNode>().First();
                            BezierElement b = new BezierElement(_eventCallbackNodeFromcbComp, CallbackEventNode);
                            _eventCallbackNodeFromcbComp.RenderPipelineInfo.AddChild(b);
                            _eventCallbackNodeFromcbComp.Connections.Add(b);
                            CallbackEventNode.RenderPipelineInfo.AddChild(b);
                            CallbackEventNode.Connections.Add(b);
                            _eventCallbackNodeFromcbComp.ComputationPipelineInfo.AddEventUpStream(_callbackToComp);
                            EditorForm.connectionsPending.Add(b);
                            //DataTemplateManager.RegisterDataTemplate(b);
                            //DataViewModel.Instance.Elements.Add(b);
                            //b.RedrawBezier(b.Origin, b.Destination);
                            //RenderingCore.Render(_callbackToComp);
                            Main_Verse3.ActiveMain.ActiveEditor.AddToCanvas_OnCall(this, new EventArgs());
                            RenderingCore.Render(_callbackToComp, false);
                        }
                    }
                    else
                    {
                        //Main_Verse3.ActiveMain.ActiveEditor.AddToCanvas_OnCall(this, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {

                CoreConsole.Log(ex);
            }
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Callback", "`", "`",
            (Color)ColorConverter.ConvertFromString("#FFFF6700"),
            new Type[] { typeof(int), typeof(int), typeof(BaseComp) });

        private CallbackNode CallbackEventNode;
        private EventCallbackNode _eventCallbackNodeFromcbComp;

        private GenericEventNode LoopFinishNode;
        private GenericEventNode LoopEndNode;
        private GenericDataNode LoopDataNode;
        private GenericDataNode DataOut;
        private NumberDataNode LoopIterationCount;

        public override void Initialize()
        {
            CallbackEventNode = new CallbackNode(this);
            this.ChildElementManager.AddEventInputNode(CallbackEventNode);

            if (_callbackToComp.ChildElementManager.OutputSide.OfType<EventCallbackNode>().Any())
            {
                _eventCallbackNodeFromcbComp = _callbackToComp.ChildElementManager.OutputSide.OfType<EventCallbackNode>().First();
            }

            LoopFinishNode = new GenericEventNode(this, NodeType.Input);
            LoopFinishNode.NodeEvent += LoopFinishNode_NodeEvent;
            this.ChildElementManager.AddEventInputNode(LoopFinishNode, "Finish");

            LoopEndNode = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(LoopEndNode, "End");

            //LoopDataNode = new GenericDataNode(this, NodeType.Input);
            //this.ChildElementManager.AddDataInputNode(LoopDataNode, "Data");

            DataOut = new GenericDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(DataOut, "Data");

            LoopIterationCount = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(LoopIterationCount, "Iteration Count", true);
        }

        private void LoopFinishNode_NodeEvent(IEventNode container, EventArgData e)
        {
            if (_callbackToComp is Loop loop)
            {
                //loop._loopData = this.ChildElementManager.GetData(LoopDataNode);
                if (e.Count > 0)
                {
                    if (e[0] is DataStructure eData)
                    {
                        loop._loopData = eData;
                    }
                }
                if (loop._count < loop._iterations)
                {
                    //loop._loopData = _loopData;
                    //loop._count++;
                    CallbackEventNode.EventOccured(new EventArgData(loop._loopData), true);
                }
                else if (loop._count == loop._iterations)
                {
                    loop._loopRunning = false;
                    CallbackEventNode.EventOccured(new EventArgData(loop._loopData), true);
                    LoopEndNode.EventOccured(new EventArgData(loop._loopData));
                }
                this.ChildElementManager.SetData(loop._loopData, DataOut);
                this.ChildElementManager.SetData(loop._count, LoopIterationCount);
                ComputationCore.Compute(this, false);
            }
        }
    }

    internal class CallbackNode : EventNodeElement
    {
        public CallbackNode(Callback parent) : base(parent, NodeType.Input)
        {
            Color c = (Color)ColorConverter.ConvertFromString("#FFFF6700");
            this.NodeColor = new SolidColorBrush(c);
        }

        public override void ToggleActive()
        {
            ComputationCore.Compute(this.Parent as BaseComp, false);
        }
    }
}
