using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace InteropLibrary
{
    public class InteropMessageSND : BaseComp
    {
        internal string _lastMessage = "";

        #region Constructors

        public InteropMessageSND() : base()
        {
        }

        public InteropMessageSND(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override void Compute()
        {
            object dataIN = this.ChildElementManager.GetData<object>(nodeBlock);
            if (dataIN != null)
            {
                _lastMessage = dataIN.ToString();
                DataStructure goo = new DataStructure(dataIN);
                CoreInterop.InteropServer._LocalInteropServer.Send(goo);
                this.previewTextBlock.DisplayedText = $"Last Message = {_lastMessage}";
            }
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Interop Message Send", "Events", "Interop");
        
        internal GenericDataNode nodeBlock;
        internal GenericEventNode eventNode;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            eventNode = new GenericEventNode(this, NodeType.Input);
            this.ChildElementManager.AddEventInputNode(eventNode, "Force Send");

            nodeBlock = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Data");
        }
    }
}
