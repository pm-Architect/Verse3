using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Core.Geometry2D;

namespace Core
{
    #region Nodes and Connections

    public interface INode : IElement
    {
        public IElement Parent { get; set; }
        public ElementsLinkedList<IConnection> Connections { get; }
        public NodeType NodeType { get; }
        public CanvasPoint Hotspot { get; }
        public double HotspotThresholdRadius { get; }
        public new ElementType ElementType { get => ElementType.Node; }

    }

    public interface IDataNode : INode, IDataGooContainer
    {
        //public event EventHandler<DataChangedEventArgs> DataChanged;
        public void ToggleActive();
    }

    public interface IDataNode<D> : IDataNode
    {
        //public new event EventHandler<DataChangedEventArgs<D>> DataChanged;
        public delegate void NodeDataChangedEventHandler(IDataNode<D> container, DataChangedEventArgs<D> e);
        public event NodeDataChangedEventHandler NodeDataChanged;
        new DataStructure<D> DataGoo { get; set; }
    }

    public interface IEventNode : INode, IComputable
    {
        public delegate void NodeEventHandler(IEventNode container, EventArgData e);
        public event NodeEventHandler NodeEvent;
        public EventArgData EventArgData { get; set; }
        public void TriggerEvent();
        public bool EventOccured(EventArgData e);
        void ToggleActive();
    }

    public class EventArgData : DataStructure<EventArgs>
    {
        //TODO: Implement relevant fields
    }

    public interface IConnection : IElement
    {
        public INode Origin { get; }
        public INode Destination { get; }
        public ConnectionType ConnectionType { get; }
        public new ElementType ElementType { get => ElementType.Connection; }
    }

    #endregion
}
