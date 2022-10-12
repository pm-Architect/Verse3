using System;
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
        public string Name { get; set; }
        public new ElementType ElementType { get/* => ElementType.Node*/; }
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
        public void TriggerEvent(EventArgData e);
        public bool EventOccured(EventArgData e);
        void ToggleActive();
    }

    public class EventArgData : DataStructure<EventArgs>
    {
        //TODO: Implement relevant fields
        public EventArgData() : base()
        {
        }
        public EventArgData(EventArgs eventargs) : base(eventargs)
        {
        }
        public EventArgData(DataStructure argdata) : base()
        {
            this.Add(argdata);
            //if (this.Count > 0)
            //{
            //    string data = this[0].Data.ToString();
            //}
        }
        public EventArgData(EventArgs eventargs, DataStructure argdata) : base(eventargs)
        {
            this.Add(argdata);
        }
    }

    public interface IConnection : IElement
    {
        public INode Origin { get; }
        public INode Destination { get; }
        public ConnectionType ConnectionType { get; }
        public new ElementType ElementType { get/* => ElementType.Connection*/; }
    }

    #endregion

    public static class NodeUtilities
    {
        public static bool CheckCompatibility(INode origin, INode destination)
        {
            if (origin.NodeType == NodeType.Input && destination.NodeType == NodeType.Output
                || origin.NodeType == NodeType.Output && destination.NodeType == NodeType.Input)
            {
                if (origin is IDataNode && destination is IDataNode)
                {
                    if ((origin as IDataNode).DataValueType == (destination as IDataNode).DataValueType)
                    {
                        return true;
                    }
                    else if ((destination as IDataNode).DataValueType.IsAssignableFrom((origin as IDataNode).DataValueType))
                    {
                        return true;
                    }
                    else if ((origin as IDataNode).DataValueType.IsAssignableFrom((destination as IDataNode).DataValueType))
                    {
                        //CAST!!
                        return true;
                    }
                }
                else if (origin is IEventNode && destination is IEventNode)
                {
                    if ((origin as IEventNode).EventArgData != null && (destination as IEventNode).EventArgData != null)
                    {
                        if ((origin as IEventNode).EventArgData.DataType == (destination as IEventNode).EventArgData.DataType)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        //TODO: Warn user that event data type does not match or are null!!!!
                        //DISALLOW if a flag is true
                        return true;
                    }
                }
            }
            return false;
        }
    }

    //public class HorizontalAlignmentConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if ((NodeType)value == NodeType.Input)
    //            return HorizontalAlignment.Left;
    //        else if ((NodeType)value == NodeType.Output)
    //            return HorizontalAlignment.Right;
    //        else return HorizontalAlignment.Left;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
