using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Geometry2D;

namespace Core
{
    #region Nodes and Connections

    public interface INode : IElement
    {
        public IElement Parent { get; }
        public ElementsLinkedList<IConnection> Connections { get; }
        public NodeType NodeType { get; }
        public CanvasPoint Hotspot { get; }
        public double HotspotThresholdRadius { get; }
        public new ElementType ElementType { get => ElementType.Node; }

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
