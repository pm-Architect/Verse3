using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace InteropLibrary
{
    public class DataChangeEvent : BaseComp
    {
        private object _lastData;

        #region Constructors

        public DataChangeEvent() : base()
        {
        }
        public DataChangeEvent(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override void Compute()
        {
            DataStructure? dataIN = this.ChildElementManager.GetData(dataInput);
            if (dataIN != null && _lastData != dataIN)
            {
                this.ChildElementManager.SetData<object>(_lastData, lastData);
                dataChangedEvent.EventOccured(new EventArgData(dataIN));
                this.previewTextBlock.DisplayedText = $"Last Data: {_lastData}\nNew Data: {dataIN}";
                _lastData = dataIN;
            }
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Data Changed Event", "Data", "Events");
        
        private GenericDataNode dataInput;
        private GenericEventNode dataChangedEvent;
        private GenericDataNode lastData;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            dataInput = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(dataInput);

            dataChangedEvent = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(dataChangedEvent);

            lastData = new GenericDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(lastData, "Last Data", true);
        }
    }
}
