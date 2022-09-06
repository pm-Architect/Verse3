using Core;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Verse3.VanillaElements;
using static Core.Geometry2D;
using XamlReader = System.Windows.Markup.XamlReader;

namespace Verse3
{
    /// <summary>
    /// A simple example of a data-model.  
    /// The purpose of this data-model is to share display data between the main window and overview window.
    /// </summary>
    public class DataViewModel : DataModel
    {
        public static InfiniteCanvasWPFControl WPFControl { get; private set; }
        public static INode ActiveNode { get; internal set; }
        public static IConnection ActiveConnection { get; internal set; }
        public static void InitDataViewModel(InfiniteCanvasWPFControl c)
        {
            DataViewModel.WPFControl = c;



            //TODO Properly Load all available plugins            

            //TODO: Open a file here!!!!

            //
            // TODO: Populate the data model with file data
            //
        }
        public static IConnection CreateConnection(INode start, INode end = default)
        {
            if (end == default)
            {
                end = MousePositionNode.Instance;
            }
            BezierElement bezier = new BezierElement(start, end);
            DataTemplateManager.RegisterDataTemplate(bezier as IRenderable);
            DataViewModel.Instance.Elements.Add(bezier);
            //start.Connections.Add(bezier);
            //end.Connections.Add(bezier);
            return bezier;
        }
    }

    public interface IBaseElementView<R> : IRenderView where R : IRenderable
    {
        public new R Element
        {
            get
            {
                if (Element == null)
                {
                    if (this.GetType().IsAssignableTo(typeof(UserControl)))
                    {
                        object dc = ((UserControl)this).DataContext;
                        if (dc.GetType().IsAssignableTo(typeof(R)))
                        {
                            Element = (R)dc;
                        }
                    }
                }
                return Element;
            }
            private set
            {
                if (value is R)
                {
                    Element = (R)value;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
        }
        public Guid? ElementGuid
        {
            get { return Element?.ID; }
        }


        public new void Render()
        {
            if (this.Element != null)
            {
                if (this.Element.RenderView != this) this.Element.RenderView = this;
            }
        }

        #region MouseEvents

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        public void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        public void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        public void OnMouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when the mouse wheel is moved.
        /// </summary>
        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        #endregion

        #region UserControlEvents

        //public void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (this.GetType().IsAssignableTo(typeof(UserControl)))
        //    {
        //        UserControl uc = this as UserControl;
        //        if (uc.DataContext is R)
        //        {
        //            Element = (R)uc.DataContext;
        //        }
        //    }
        //    Render();
        //}

        //public void OnLoaded(object sender, RoutedEventArgs e)
        //{
        //    if (this.GetType().IsAssignableTo(typeof(UserControl)))
        //    {
        //        UserControl uc = this as UserControl;
        //        if (uc.DataContext is R)
        //        {
        //            Element = (R)uc.DataContext;
        //        }
        //    }
        //    Render();
        //}

        #endregion
    }

    public abstract class BaseElement : IRenderable
    {
        #region Data Members

        private RenderPipelineInfo renderPipelineInfo;
        protected BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        internal IRenderView elView;

        #endregion

        #region Properties

        public RenderPipelineInfo RenderPipelineInfo => renderPipelineInfo;
        public IRenderView RenderView
        {
            get
            {
                return elView;
            }
            set
            {
                if (ViewType.IsAssignableFrom(value.GetType()))
                {
                    elView = value;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
        }
        public abstract Type ViewType { get; }
        public object ViewKey { get; set; }

        public Guid ID { get => _id; private set => _id = value; }

        //public bool IsSelected { get; set; }

        public BoundingBox BoundingBox { get => boundingBox; private set => SetProperty(ref boundingBox, value); }

        public double X { get => boundingBox.Location.X; }

        public double Y { get => boundingBox.Location.Y; }

        public double Width
        {
            get => boundingBox.Size.Width;
            set => boundingBox.Size.Width = value;
        }

        public double Height
        {
            get => boundingBox.Size.Height;
            set => boundingBox.Size.Height = value;
        }

        public ElementState State { get; set; }

        //public IRenderView ElementView { get; internal set; }

        public ElementState ElementState { get; set; }
        
        private ElementType _elementType = ElementType.UIElement;
        public virtual ElementType ElementType { get => _elementType; set => _elementType = value; }
        bool IRenderable.Visible { get; set; }
        private bool sel = false;
        public bool IsSelected { get => sel; set => sel = false; }
        public bool RenderExpired { get; set; }

        #endregion

        public BaseElement()
        {
            this.renderPipelineInfo = new RenderPipelineInfo(this);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        #endregion

        public void Dispose()
        {
            if (this.RenderPipelineInfo.Children != null && this.RenderPipelineInfo.Children.Count > 0)
            {
                foreach (var child in this.RenderPipelineInfo.Children)
                {
                    if (child != null) child.Dispose();
                }
            }
            DataViewModel.Instance.Elements.Remove(this);
            GC.SuppressFinalize(this);
        }
        ~BaseElement() => Dispose();
    }

    #region DataTemplateManager
    public class DataTemplateManager
    {

        #region Binding Management

        public static void CreateBinding(FrameworkElementFactory BindTo, DependencyProperty BindToProperty, PropertyPath BindFromProperty, BindingMode Mode)
        {
            Binding binding = new Binding();
            binding.Path = BindFromProperty;
            binding.Mode = Mode;
            BindTo.SetBinding(BindToProperty, binding);
        }

        public static void CreateBinding(DependencyObject BindTo, DependencyProperty BindToProperty, object BindFrom, PropertyPath BindFromProperty, BindingMode Mode = BindingMode.TwoWay)
        {
            Binding binding = new Binding();
            binding.Source = BindFrom;
            binding.Path = BindFromProperty;
            binding.Mode = Mode;
            BindingOperations.SetBinding(BindTo, BindToProperty, binding);
        }

        #endregion

        #region Private DataTemplate Management

        //private static void RegisterDataTemplate<TViewModel, TView>() where TView : FrameworkElement
        //{
        //    RegisterDataTemplate(typeof(TViewModel), typeof(TView));
        //}

        //private static void RegisterDataTemplate(Type viewModelType, Type viewType)
        //{
        //    var template = CreateTemplate(viewModelType, viewType);

        //    if (DataViewModel.WPFControl.Resources[template.DataTemplateKey] != null) return;
        //    else
        //    {
        //        DataViewModel.WPFControl.Resources.Add(template.DataTemplateKey, template);
        //    }
        //}

        private static DataTemplate CreateTemplate(Type viewModelType, Type viewType)
        {
            const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
            var xaml = String.Format(xamlTemplate, viewModelType.Name, viewType.Name, viewModelType.Namespace, viewType.Namespace);

            var context = new ParserContext();

            context.XamlTypeMapper = new XamlTypeMapper(new string[0]);
            context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
            context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("vm", "vm");
            context.XmlnsDictionary.Add("v", "v");

            if (xaml.Contains("`1"))
            {
                if (/*viewModelType.GenericTypeArguments.Length == 1 && */viewModelType.IsAssignableTo(typeof(DataNodeElement<>)))
                {
                    //TODO: Log to Console
                    string t = viewModelType.Name;
                    while (xaml.Contains("DataNodeElement`1"))
                    {
                        xaml = xaml.Replace("DataNodeElement`1", t);
                        //xaml = xaml.Replace("`1", (""));
                    }
                }
            }
            //if (/*viewModelType.GenericTypeArguments.Length == 1 && */viewModelType.BaseType == (typeof(EventNodeElement)))
            //{
            //    //TODO: Log to Console
            //    string t = viewModelType.Name;
            //    while (xaml.Contains("EventNodeElement"))
            //    {
            //        xaml = xaml.Replace("EventNodeElement", t);
            //        //xaml = xaml.Replace("`1", (""));
            //    }
            //}
            if (/*viewModelType.GenericTypeArguments.Length == 1 && */viewModelType.BaseType == (typeof(BaseComp)))
            {
                //TODO: Log to Console
                string t = viewModelType.Name;
                //while (xaml.Contains("BaseComp"))
                //{
                //    //xaml = xaml.Replace("BaseComp", t);
                //    //xaml = xaml.Replace("`1", (""));
                //}
            }

            DataTemplate template = (DataTemplate)XamlReader.Parse(xaml, context);

            return template;

        }

        #endregion

        #region Public DataTemplate Management

        public static bool RegisterDataTemplate(IRenderable el)
        {
            if (el.ViewType == null) return false;
            var template = CreateTemplate(el.GetType(), el.ViewType);
            //el.BoundingBox = new BoundingBox();
            //Element needs to know DataTemplateKey in order to make a reference to it
            el.ViewKey = template.DataTemplateKey;
            if (DataViewModel.WPFControl == null) return false;
            if (DataViewModel.WPFControl.Resources[el.ViewKey] != null)
            {
                if (DataViewModel.WPFControl.Resources.Contains(el.ViewKey)) return false;
                if (el.ViewType.IsAssignableTo(typeof(DataNodeElementView)))
                {
                    DataViewModel.WPFControl.Resources.Add(el.ViewKey, template);
                    //el.RenderView = (IRenderView)DataViewModel.WPFControl.Resources[el.ViewKey];
                    return true;
                }
                else if (el.ViewType.IsAssignableTo(typeof(EventNodeElementView)))
                {
                    DataViewModel.WPFControl.Resources.Add(el.ViewKey, template);
                    //el.RenderView = (IRenderView)DataViewModel.WPFControl.Resources[el.ViewKey];
                    return true;
                }
                //el.RenderView = (IRenderView)DataViewModel.WPFControl.Resources[el.ViewKey];
                return false;
            }
            else
            {
                DataViewModel.WPFControl.Resources.Add(el.ViewKey, template);
                //el.RenderView = (IRenderView)DataViewModel.WPFControl.Resources[el.ViewKey];
                return true;
            }
        }

        #endregion
    }
    #endregion

    #region Converters and Utilities

    /// <summary>
    /// Converts a color value to a brush.
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// PseudoConverter to offset X and Y Positions for setting the canvas origin in the viewport
    /// https://stackoverflow.com/a/4973289
    /// </summary>
    public class CanvasSizeOffsetPseudoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double OffsetValue = DataViewModel.ContentCanvasMarginOffset;
            double Val = ((double)value);

            return Val + OffsetValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    #endregion
}
