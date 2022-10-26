using Core;
using System;
using System.Windows;
using System.Windows.Media;
using Verse3;
using Verse3.VanillaElements;

namespace EventsLibrary
{
    public class Loop : BaseComp
    {
        internal bool _loopRunning = false;
        
        public Loop() : base()
        {
        }
        public Loop(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            if (CallbackComp != null && !_loopRunning)
            {
                _iterations = (int)this.ChildElementManager.GetData(Iterations, 10);
                if (_iterations < 0) _iterations = 0;
                _loopData = this.ChildElementManager.GetData(DataInNode);
            }
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "For Loop", "Recursion", "Events",
            (Color)ColorConverter.ConvertFromString("#FFFF6700"));

        private GenericEventNode LoopStartNode;
        private GenericDataNode DataInNode;
        private NumberDataNode Iterations;

        private EventCallbackNode EventCallbackNode;
        private GenericEventNode LoopBeginNode;
        //private GenericDataNode DataToLoopNode;
        private NumberDataNode Count;
        private ButtonElement ButtonBlock;
        
        internal Callback CallbackComp;

        public override void Initialize()
        {
            EventCallbackNode = new EventCallbackNode(this);
            EventCallbackNode.NodeEvent += EventCallbackNode_NodeEvent;
            this.ChildElementManager.AddEventOutputNode(EventCallbackNode);
            
            LoopStartNode = new GenericEventNode(this, NodeType.Input);
            LoopStartNode.NodeEvent += StartNode_NodeEvent;
            this.ChildElementManager.AddEventInputNode(LoopStartNode, "Start");

            LoopBeginNode = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(LoopBeginNode, "Begin");

            DataInNode = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(DataInNode, "Data");

            Iterations = new NumberDataNode(this, NodeType.Input, 0);
            this.ChildElementManager.AddDataInputNode(Iterations, "Iterations");

            //DataToLoopNode = new GenericDataNode(this, NodeType.Output);
            //this.ChildElementManager.AddDataOutputNode(DataToLoopNode, "Data");

            Count = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Count, "Count", true);

            ButtonBlock = new ButtonElement();
            ButtonBlock.DisplayedText = "Force Stop Loop";
            ButtonBlock.OnButtonClicked += ButtonBlock_OnButtonClicked;
            this.ChildElementManager.AddElement(ButtonBlock);
        }

        private void ButtonBlock_OnButtonClicked(object? sender, RoutedEventArgs e)
        {
            _loopRunning = false;
            _count = 0;
            this.ChildElementManager.SetData(_count, Count);
            ComputationCore.Compute(this, false);
        }

        private void EventCallbackNode_NodeEvent(IEventNode container, EventArgData e)
        {
            if (_iterations > 0)
            {
                if (_count < _iterations && _count > 0 && _loopRunning)
                {
                    //this.ChildElementManager.SetData(_loopData, DataToLoopNode);
                    _count++;
                    this.ChildElementManager.SetData(_count, Count);
                    LoopBeginNode.EventOccured(new EventArgData(_loopData));
                }
                else if (_count == _iterations)
                {
                    _count = 0;
                    this.ChildElementManager.SetData(_count, Count);
                }
                ComputationCore.Compute(this, false);
            }
        }

        internal int _iterations = 10;
        internal int _count = 0;
        internal DataStructure _loopData;

        private void StartNode_NodeEvent(IEventNode container, EventArgData e)
        {
            if (_iterations > 0)
            {
                if (!_loopRunning && _count == 0)
                {
                    //this.ChildElementManager.SetData(_loopData, DataToLoopNode);
                    _loopRunning = true;
                    LoopBeginNode.EventOccured(new EventArgData(_loopData));
                    _count++;
                    this.ChildElementManager.SetData(_count, Count);
                }
                ComputationCore.Compute(this, false);
            }
        }
    }

    public class EventCallbackNode : EventNodeElement
    {
        public EventCallbackNode(IRenderable parent) : base(parent, NodeType.Output)
        {
            Color c = (Color)ColorConverter.ConvertFromString("#FFFF6700");
            this.NodeColor = new SolidColorBrush(c);
            if (parent is Loop loop)
            {
                //Callback callback = new Callback(DataViewModel.WPFControl.GetMouseRelPosition().X, DataViewModel.WPFControl.GetMouseRelPosition().Y, parent as BaseComp);
                object[] args = new object[3];
                args[0] = DataViewModel.WPFControl.GetMouseRelPosition().X;
                args[1] = DataViewModel.WPFControl.GetMouseRelPosition().Y;
                args[2] = parent as BaseComp;
                using (Callback callback = new Callback(0, 0, parent as BaseComp))
                    EditorForm.compsPendingInst.Add(callback.GetCompInfo(), args);
                //Main_Verse3.ActiveMain.ActiveEditor.AddToCanvas_OnCall(this, new EventArgs());
            }
        }

        public override void ToggleActive()
        {
            //ComputationCore.Compute(this.Parent as BaseComp, false);
            if (this.Parent is Loop loop && (loop.CallbackComp != null))
            {
                ComputationCore.Compute(loop.CallbackComp, false);
            }
        }
    }
}
