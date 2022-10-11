using Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Verse3.VanillaElements
{
    public class GenericDataNode : DataNodeElement<object>
    {
        public GenericDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }


    public class NumberDataNode : DataNodeElement<double>
    {
        public NumberDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }

    public class BooleanDataNode : DataNodeElement<bool>
    {
        public BooleanDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
    public class TextDataNode : DataNodeElement<string>
    {
        public TextDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
    public class DateTimeDataNode : DataNodeElement<DateTime>
    {
        public DateTimeDataNode(BaseComp parent, NodeType nodeType) : base(parent, nodeType)
        {
        }
    }
    public class GenericEventNode : EventNodeElement
    {
        public GenericEventNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
    public class ButtonClickedEventNode : EventNodeElement
    {
        public ButtonClickedEventNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }

        public static implicit operator GenericEventNode(ButtonClickedEventNode v)
        {
            GenericEventNode outValue = new GenericEventNode(v.Parent, v.NodeType);
            outValue = (GenericEventNode)(v as EventNodeElement);
            return outValue;
        }
    }
    public class InteropMessageEventNode : EventNodeElement
    {
        public InteropMessageEventNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }

        public static implicit operator GenericEventNode(InteropMessageEventNode v)
        {
            GenericEventNode outValue = new GenericEventNode(v.Parent, v.NodeType);
            outValue = (GenericEventNode)(v as EventNodeElement);
            outValue.EventArgData = v.EventArgData;
            return outValue;
        }
    }
}
