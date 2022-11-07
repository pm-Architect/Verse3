using Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Verse3.VanillaElements
{
    //[Serializable]
    public class GenericDataNode : DataNodeElement<object>
    {
        public GenericDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }


    //[Serializable]
    public class NumberDataNode : DataNodeElement<double>
    {
        //TODO: Properties like Accept Integers Only, Accept Decimals Only, etc.
        public NumberDataNode(IRenderable parent, NodeType type = NodeType.Unset, int decimalPlaces = 2) : base(parent, type)
        {
            this.DecimalPlaces = decimalPlaces;
        }

        private int _decimalPlaces = 2;
        public int DecimalPlaces { get => _decimalPlaces; private set => _decimalPlaces = value; }
        private DataStructure<double> _dataGoo = new DataStructure<double>();
        public new DataStructure<double> DataGoo
        {
            get => _dataGoo;
            set
            {
                try
                {
                    if (value != null && value is DataStructure<double>)
                    {
                        _dataGoo = value as DataStructure<double>;
                        //TODO: Round to decimal places
                        //double data = _dataGoo.Data;
                        //data = Math.Round(data, _decimalPlaces);
                    }
                    else
                    {
                        if (value != null)
                        {
                            if (value.Data.GetType().IsAssignableTo(typeof(double)))
                            {
                                _dataGoo = value.DuplicateAsType<double>();
                            }
                        }
                        _dataGoo = new DataStructure<double>();
                    }
                }
                catch (Exception ex)
                {
                    CoreConsole.Log(ex);
                }
            }
        }
    }

    //[Serializable]
    public class BooleanDataNode : DataNodeElement<bool>
    {
        public BooleanDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
    //[Serializable]
    public class TextDataNode : DataNodeElement<string>
    {
        public TextDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
    //[Serializable]
    public class DateTimeDataNode : DataNodeElement<DateTime>
    {
        public DateTimeDataNode(BaseComp parent, NodeType nodeType) : base(parent, nodeType)
        {
        }
    }

    //[Serializable]
    public class TimeZoneDataNode : DataNodeElement<TimeZoneInfo>
    {
        public TimeZoneDataNode(BaseComp parent, NodeType nodeType) : base(parent, nodeType)
        {
        }
    }

    //[Serializable]
    public class TimeSpanDataNode : DataNodeElement<TimeSpan>
    {
        public TimeSpanDataNode(BaseComp parent, NodeType nodeType) : base(parent, nodeType)
        {
        }
    }

    //[Serializable]
    public class GenericEventNode : EventNodeElement
    {
        public GenericEventNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }

    ////[Serializable]
    //public class ButtonClickedEventNode : EventNodeElement
    //{
    //    public ButtonClickedEventNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
    //    {
    //    }

    //    public static implicit operator GenericEventNode(ButtonClickedEventNode v)
    //    {
    //        GenericEventNode outValue = new GenericEventNode(v.Parent, v.NodeType);
    //        outValue = (GenericEventNode)(v as EventNodeElement);
    //        return outValue;
    //    }
    //}
    //[Serializable]
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
