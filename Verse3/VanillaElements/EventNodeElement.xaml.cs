using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using static Core.Geometry2D;

namespace Verse3.VanillaElements
{
    /// <summary>
    /// Visual Interaction logic for TestElement.xaml
    /// </summary>
    public partial class EventNodeElementView : UserControl, IBaseElementView<IRenderable>
    {
        #region IBaseElementView Members

        private Type Y = default;
        //DEV NOTE: CAUTION! DYNAMIC TYPE IS USED
        private dynamic _element;
        public IRenderable Element
        {
            get
            {
                if (this._element == null)
                {
                    if (this.DataContext != null)
                    {
                        Y = this.DataContext.GetType();
                        if (Y.BaseType.Name == (typeof(EventNodeElement).Name))
                        {
                            //TODO: Log to Console and process
                            //if (this.DataContext.GetType().GenericTypeArguments.Length == 1)
                            //Y = this.DataContext.GetType().MakeGenericType(Y);
                            //_element = Convert.ChangeType(this.DataContext, U) as IRenderable;
                            Y = this.DataContext.GetType()/*.MakeGenericType(this.DataContext.GetType().GenericTypeArguments[0].GetType())*/;
                            _element = this.DataContext;
                            return _element;
                        }
                        else if (Y.BaseType.Name == typeof(EventNodeElement).Name)
                        {
                            //TODO: Log to Console and process
                            //if (this.DataContext.GetType().GenericTypeArguments.Length == 1)
                            //Y = this.DataContext.GetType().MakeGenericType(Y);
                            //_element = Convert.ChangeType(this.DataContext, U) as IRenderable;
                            Y = this.DataContext.GetType()/*.MakeGenericType(this.DataContext.GetType().GenericTypeArguments[0].GetType())*/;
                            _element = this.DataContext;
                            return _element;
                        }
                    }
                }
                return _element;
            }
            private set
            {
                if (Y != default)
                {
                    if (value.GetType().IsAssignableTo(Y))
                    {
                        _element = value;
                    }
                }
            }
        }
        IRenderable IRenderView.Element => Element;
        //public static T ForceCast<T>(object obj)
        //{
        //    try
        //    {
        //        return
        //    }
        //    catch/* (Exception ex)*/
        //    {
        //        //CoreConsole.Log(ex);
        //    }
        //}

        #endregion

        #region Constructor and Render

        public EventNodeElementView()
        {
            InitializeComponent();
        }

        public void Render()
        {
            if (this.Element != null && this.Element is IEventNode)
            {
                IEventNode node = this.Element as IEventNode;
                //if (!string.IsNullOrEmpty(this._element.Text))
                //{
                //    if (this.NodeRightText.Text != node.Name && this.NodeLeftText.Text != node.Name)
                //    {
                //        if (node.NodeType == NodeType.Input)
                //        {
                //            this.NodeRightText.Text = node.Name;
                //        }
                //        else if (node.NodeType == NodeType.Output)
                //        {
                //            this.NodeLeftText.Text = node.Name;
                //        }
                //    }
                //}
                if (Element.RenderView != this) Element.RenderView = this;
                if (node.Connections != null)
                {
                    if (node.Connections.Count > 0)
                    {
                        (this._element as EventNodeElement).NodeContentColor = Brushes.White;
                        foreach (BezierElement bezier in node.Connections)
                        {
                            if (bezier != null)
                            {
                                if (bezier.Origin == this.Element)
                                {

                                }
                                else if (bezier.Destination == this.Element)
                                {

                                }
                                bezier.RedrawBezier(bezier.Origin, bezier.Destination);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            (this._element as EventNodeElement).NodeContentColor = Brushes.Transparent;
                            //RenderingCore.Render((this._element as EventNodeElement).Parent);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    try
                    {
                        (this._element as EventNodeElement).NodeContentColor = Brushes.Transparent;
                        //RenderingCore.Render((this._element as EventNodeElement).Parent);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        #endregion

        #region MouseEvents

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
        }

        void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        #endregion

        #region UserControlEvents

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //DependencyPropertyChangedEventArgs
            if (this.DataContext != null)
            {
                this.Element = this.DataContext as IRenderable;
                Render();
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            if (this.DataContext != null)
            {
                //this._element = this.DataContext as NodeElement;
                //Render();
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.Element != null)
            {
                (this.Element as IEventNode).ToggleActive();
            }
            //DEV NOTE: CAUTION! DYNAMIC TYPE IS USED
            //try
            //{
            //    dynamic element = Convert.ChangeType(this.Element, Y);
            //    element.ToggleActive(this, e);
            //}
            //catch (Exception)
            //{
            //    //Log to Console and process
            //    throw;
            //}
        }
    }

    public class EventNodeElement : EventNode
    {
        [JsonIgnore]
        public override Type ViewType => typeof(EventNodeElementView);

        #region Constructor

        public EventNodeElement(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
            //_computationPipelineInfo = new ComputationPipelineInfo(this);
            this.RenderPipelineInfo.Parent = parent as IRenderable;
            double x = DataViewModel.ContentCanvasMarginOffset + this.RenderPipelineInfo.Parent.X;
            double y = DataViewModel.ContentCanvasMarginOffset + this.RenderPipelineInfo.Parent.Y;
            base.boundingBox = new BoundingBox(x, y, this.RenderPipelineInfo.Parent.Width, 50);
            (this as IRenderable).RenderPipelineInfo.SetParent(this.RenderPipelineInfo.Parent);
            //this.DisplayedText = "Node";
            this.PropertyChanged += NodeElement_PropertyChanged;
            if (type == NodeType.Input)
            {
                this.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (type == NodeType.Output)
            {
                this.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                this.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }
        public EventNodeElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.PropertyChanged += NodeElement_PropertyChanged;
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion

        private void NodeElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (IRenderable renderable in this.Connections)
            {
                //renderable.Render();
            }
        }

        public override void ToggleActive()
        {
            //if (this.IsActive)
            //{
            //    this.IsActive = false;
            //}
            //else
            //{
            //    this.IsActive = true;
            //}
            //Set as active Node
            BezierElement b = (BezierElement)DataViewModel.ActiveConnection;
            if (DataViewModel.ActiveConnection == default)
            {
                DataViewModel.ActiveNode = this as INode;
                DataViewModel.ActiveConnection = DataViewModel.CreateConnection(DataViewModel.ActiveNode);
                b = (BezierElement)DataViewModel.ActiveConnection;
                if (b != null)
                {
                    this.RenderPipelineInfo.AddChild(b);
                    this.Connections.Add(b);
                    //this.nodeContentColor = System.Windows.Media.Brushes.White;
                    MousePositionNode.Instance.Connections.Add(b);
                    //b.RedrawBezier(b.Origin, b.Destination);
                    //b.RenderView.Render();
                }
            }
            else
            {
                if (b != null)
                {
                    if (DataViewModel.ActiveNode.NodeType != this.NodeType)
                    {
                        if (DataViewModel.ActiveNode.GetType().BaseType == this.GetType().BaseType)
                        {
                            if (b.SetDestination(this as INode))
                            {
                                if (DataViewModel.ActiveNode is IComputable && DataViewModel.ActiveNode != this && DataViewModel.ActiveNode.NodeType == NodeType.Output)
                                {
                                    this.ComputationPipelineInfo.AddEventUpStream(DataViewModel.ActiveNode as IComputable);
                                }
                                DataViewModel.ActiveNode = this as INode;
                                if (MousePositionNode.Instance.Connections.Contains(b))
                                    MousePositionNode.Instance.Connections.Remove(b);
                                this.RenderPipelineInfo.AddChild(b);
                                this.Connections.Add(b);
                                //this.nodeContentColor = System.Windows.Media.Brushes.White;
                                DataViewModel.ActiveConnection = default;
                                DataViewModel.ActiveNode = default;
                            }
                        }
                    }
                }
            }
            //ComputationPipeline.Compute();
            //RenderPipeline.Render();
            //this.Element.OnPropertyChanged("BoundingBox");
            if (this.RenderPipelineInfo.Parent is IComputable)
            {
                IComputable computable = (IComputable)this.RenderPipelineInfo.Parent;
                ComputationCore.Compute(computable);
            }
            RenderingCore.Render(this.RenderPipelineInfo.Parent);
        }

        private string _name = "";
        public override string Name { get => _name; set => _name = value; }

        private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center;
        [JsonIgnore]
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                //if (this.NodeType == NodeType.Input)
                //{
                //    if (horizontalAlignment != HorizontalAlignment.Left)
                //    {
                //        horizontalAlignment = HorizontalAlignment.Left;
                //        //SetProperty(ref horizontalAlignment, HorizontalAlignment.Left);
                //        OnPropertyChanged("HorizontalAlignment");
                //    }
                //}
                //else if (this.NodeType == NodeType.Output)
                //{
                //    if (horizontalAlignment != HorizontalAlignment.Right)
                //    {
                //        horizontalAlignment = HorizontalAlignment.Right;
                //        //SetProperty(ref horizontalAlignment, HorizontalAlignment.Right);
                //        OnPropertyChanged("HorizontalAlignment");
                //    }
                //}
                return horizontalAlignment;
            }
            private set => SetProperty(ref horizontalAlignment, value);
        }

        private System.Windows.Media.Brush nodeColor = System.Windows.Media.Brushes.Transparent;
        [JsonIgnore]
        public System.Windows.Media.Brush NodeColor
        {
            get
            {
                return nodeColor;
            }
            set => SetProperty(ref nodeColor, value);
        }

        private System.Windows.Media.Brush nodeContentColor = System.Windows.Media.Brushes.Transparent;
        [JsonIgnore]
        public System.Windows.Media.Brush NodeContentColor
        {
            get
            {
                return nodeContentColor;
            }
            set => SetProperty(ref nodeContentColor, value);
        }

        [JsonIgnore]
        public bool IsActive { get; protected set; }
    }

    //public class NodeNameDisplaySideConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is NodeType)
    //        {
    //            if ((NodeType)value == NodeType.Input)
    //            {
    //                if (parameter is bool)
    //                {
    //                    if (!(bool)parameter)
    //                    {
    //                        return true;
    //                    }
    //                }
    //            }
    //            else if ((NodeType)value == NodeType.Output)
    //            {
    //                if (parameter is bool)
    //                {
    //                    if ((bool)parameter)
    //                    {
    //                        return true;
    //                    }
    //                }
    //            }
    //        }
    //        return false;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    public class NodeNameDisplayTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is FrameworkElement)
            {
                FrameworkElement f = values[0] as FrameworkElement;
                //bool.TryParse((string)parameter, out bool b);
                bool b = (f.Name.Contains("Right"));
                if (values[1] is RenderPipelineInfo)
                {
                    IRenderable renderable = ((RenderPipelineInfo)values[1]).Renderable;
                    if (renderable != null && renderable is INode)
                    {
                        INode n = (INode)renderable;
                        if (n.NodeType == NodeType.Input)
                        {
                            if (b)
                            {
                                //Set Width
                                if (renderable.RenderView is EventNodeElementView)
                                {
                                    TextBlock t = (renderable.RenderView as EventNodeElementView).NodeRightText;
                                    t.Text = n.Name;
                                    t.UpdateLayout();
                                    renderable.Width = t.ActualWidth + 50;
                                }
                                else if (renderable.RenderView is DataNodeElementView)
                                {
                                    TextBlock t = (renderable.RenderView as DataNodeElementView).NodeRightText;
                                    t.Text = n.Name;
                                    t.UpdateLayout();
                                    renderable.Width = t.ActualWidth + 50;
                                }
                                return n.Name;
                            }
                        }
                        else if (n.NodeType == NodeType.Output)
                        {
                            if (!b)
                            {
                                //Set Width
                                if (renderable.RenderView is EventNodeElementView)
                                {
                                    TextBlock t = (renderable.RenderView as EventNodeElementView).NodeLeftText;
                                    t.Text = n.Name;
                                    t.UpdateLayout();
                                    renderable.Width = t.ActualWidth + 50;
                                }
                                else if (renderable.RenderView is DataNodeElementView)
                                {
                                    TextBlock t = (renderable.RenderView as DataNodeElementView).NodeLeftText;
                                    t.Text = n.Name;
                                    t.UpdateLayout();
                                    renderable.Width = t.ActualWidth + 50;
                                }
                                return n.Name;
                            }
                        }
                    }
                }
            }
            return "";
            //if (parameter is bool)
            //{
            //    if ((bool)parameter)
            //    {
            //        if (value is string)
            //        {
            //            return (string)value;
            //        }
            //    }
            //}
            //return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}