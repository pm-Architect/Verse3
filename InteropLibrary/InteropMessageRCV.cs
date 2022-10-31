using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace InteropLibrary
{
    public class InteropMessageRCV : BaseComp
    {
        #region Constructors

        public InteropMessageRCV() : base()
        {
        }

        public InteropMessageRCV(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override void Compute()
        {
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Interop Message Receiver", "Events", "Interop");
        
        internal InteropMessageEventNode nodeBlock;
        internal GenericDataNode nodeBlock2;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;
            
            CoreInterop.InteropServer._LocalInteropServer.ClientMessage += _LocalInteropServer_ClientMessage;

            nodeBlock = new InteropMessageEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(nodeBlock);

            nodeBlock2 = new GenericDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Last Message", true);
        }

        private void _LocalInteropServer_ClientMessage(object? sender, DataStructure e)
        {
            this.ChildElementManager.EventOccured(nodeBlock, new EventArgData(e));
            this.ChildElementManager.SetData(e, nodeBlock2);
            ComputationCore.Compute(this, false);
        }
    }
}
