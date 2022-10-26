using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace EventsLibrary
{
    public class SetEventArgs : BaseComp
    {
        internal int _counter = 0;
        internal string Old_argstring = "";
        internal string New_argstring = "";

        #region Constructors

        public SetEventArgs() : base()
        {
        }
        public SetEventArgs(int x, int y) : base(x, y)
        {
            this.previewTextBlock.DisplayedText = $"Counter: {_counter}\nOld Args: {Old_argstring}\nNew Args = {New_argstring}";
        }

        #endregion

        public override void Compute()
        {
            this.previewTextBlock.DisplayedText = $"Counter = {_counter}\nOld Args = {Old_argstring}\nNew Args = {New_argstring}";
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Set Event Args", "Event Utilities", "Events");

        internal GenericEventNode nodeBlock;
        internal GenericEventNode nodeBlock1;
        internal GenericDataNode nodeBlock2;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            nodeBlock = new GenericEventNode(this, NodeType.Input);
            nodeBlock.NodeEvent += NodeBlock_NodeEvent;
            this.ChildElementManager.AddEventInputNode(nodeBlock);

            nodeBlock2 = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Args");

            nodeBlock1 = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(nodeBlock1);
        }

        private void NodeBlock_NodeEvent(IEventNode container, EventArgData e)
        {
            try
            {
                _counter++;
                DataStructure argData = this.ChildElementManager.GetData(nodeBlock2);
                EventArgData eNew = new EventArgData(argData);
                nodeBlock1.EventOccured(eNew);
                New_argstring = eNew.ToString();
                Old_argstring = e.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ComputationCore.Compute(this, false);
        }
    }
}
