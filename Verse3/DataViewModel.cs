using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xaml;
using System.Xaml.Schema;
using Core;
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
        public static void CreateDataViewModel(InfiniteCanvasWPFControl c)
        {
            DataViewModel.WPFControl = c;
            
            //TODO Properly Load all available plugins

            

            //TODO: Open a file here!!!!

            //
            // TODO: Populate the data model with file data
            //
            //DataModel.Instance.Elements.Add(new ElementWrapper(50, 50, 80, 150));
            //DataModel.Instance.Elements.Add(new ElementWrapper(550, 350, 80, 150));
            //DataModel.Instance.Elements.Add(new ElementWrapper(850, 850, 30, 20));
            //DataModel.Instance.Elements.Add(new ElementWrapper(1200, 1200, 80, 150));
        }
    }

    //public partial class ElementDataViewWrapper : ElementData
    //{
    //    public ElementDataViewTemplate<ElementDataViewWrapper> Template
    //    {
    //        get;
    //        private set;
    //    }

    //    public ElementDataViewWrapper() : base()
    //    {
    //        //this.Template = new ElementDataViewTemplate<ElementDataViewWrapper>();
    //        Instantiate();
    //    }

    //    public ElementDataViewWrapper(double x, double y, double width, double height) : base(x, y, width, height)
    //    {
    //        //this.Template = new ElementDataViewTemplate<ElementDataViewWrapper>();
    //        Instantiate();
    //    }

    //    public void Instantiate()
    //    {

    //        if (LBcontent == null)
    //        {
    //            try
    //            {
    //                ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.Resources.Add("DefaultElementDataViewTemplate", this);
    //                LBcontent = (ListBox)ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.FindName("LBcontent");
    //                LBcontent.ItemsSource = DataViewModel.Instance.Elements;
    //            }
    //            catch (Exception)
    //            {
    //                //TODO
    //                Console.WriteLine("Error");
    //                return;
    //            }
    //        }
    //    }


    //    #region RectangleTEMP


    //    private static ListBox LBcontent = null;        

    //    /// <summary>
    //    /// Event raised when a mouse button is clicked down over a Rectangle.
    //    /// </summary>
    //    private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
    //    {
    //        LBcontent.Focus();
    //        Keyboard.Focus(LBcontent);

    //        Rectangle rectangle = (Rectangle)sender;
    //        ElementData myRectangle = (ElementData)rectangle.DataContext;

    //        //myRectangle.IsSelected = true;

    //        //mouseButtonDown = e.ChangedButton;

    //        if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
    //        {
    //            //
    //            // When the shift key is held down special zooming logic is executed in content_MouseDown,
    //            // so don't handle mouse input here.
    //            //
    //            return;
    //        }

    //        if (ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.MouseHandlingMode != MouseHandlingMode.None)
    //        {
    //            //
    //            // We are in some other mouse handling mode, don't do anything.
    //            return;
    //        }

    //        ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.MouseHandlingMode = MouseHandlingMode.DraggingRectangles;
    //        ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.origContentMouseDownPoint = e.GetPosition(LBcontent);

    //        rectangle.CaptureMouse();

    //        e.Handled = true;
    //    }

    //    /// <summary>
    //    /// Event raised when a mouse button is released over a Rectangle.
    //    /// </summary>
    //    private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
    //    {
    //        if (ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.MouseHandlingMode != MouseHandlingMode.DraggingRectangles)
    //        {
    //            //
    //            // We are not in rectangle dragging mode.
    //            //
    //            return;
    //        }

    //        ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.MouseHandlingMode = MouseHandlingMode.None;

    //        Rectangle rectangle = (Rectangle)sender;
    //        rectangle.ReleaseMouseCapture();

    //        e.Handled = true;
    //    }

    //    /// <summary>
    //    /// Event raised when the mouse cursor is moved when over a Rectangle.
    //    /// </summary>
    //    private void Rectangle_MouseMove(object sender, MouseEventArgs e)
    //    {
    //        if (ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.MouseHandlingMode != MouseHandlingMode.DraggingRectangles)
    //        {
    //            //
    //            // We are not in rectangle dragging mode, so don't do anything.
    //            //
    //            return;
    //        }

    //        Point curContentPoint = e.GetPosition(LBcontent);
    //        Vector rectangleDragVector = curContentPoint - ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.origContentMouseDownPoint;

    //        //
    //        // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
    //        //

    //        ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.origContentMouseDownPoint = curContentPoint;

    //        Rectangle rectangle = (Rectangle)sender;
    //        ElementData myRectangle = (ElementData)rectangle.DataContext;
    //        myRectangle.X += rectangleDragVector.X;
    //        myRectangle.Y += rectangleDragVector.Y;

    //        ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.ExpandContent();

    //        e.Handled = true;
    //    }

    //    #endregion
    //}

    //public class DataTemplate<T> : DataTemplate
    //{
    //    //public T Owner
    //    //{
    //    //    get;
    //    //    protected set;
    //    //}
    //    public DataTemplate(T owner)
    //    {
    //        //this.Owner = owner;
    //        this.DataType = typeof(T);
    //        this.VisualTree = new FrameworkElementFactory();
    //        FrameworkElementFactory wrapper = new FrameworkElementFactory(typeof(DataTemplate<T>));
    //    }
    //    public DataTemplate()
    //    {
    //        this.DataType = typeof(T);
    //        this.VisualTree = new FrameworkElementFactory();
    //        FrameworkElementFactory wrapper = new FrameworkElementFactory(typeof(DataTemplate<T>));
    //        //DefaultStyleKeyProperty.OverrideMetadata(typeof(InfiniteCanvasControl), new FrameworkPropertyMetadata(typeof(InfiniteCanvasControl)));
    //        //FrameworkElementFactory wrapper = new FrameworkElementFactory(typeof(T));
    //        //this.VisualTree = wrapper;
    //        //this.VisualTree = new FrameworkElementFactory(typeof(T));
    //    }

    //    public static void CreateBinding(FrameworkElementFactory BindTo, DependencyProperty BindToProperty, PropertyPath BindFromProperty, BindingMode Mode)
    //    {
    //        Binding binding = new Binding();
    //        binding.Path = BindFromProperty;
    //        binding.Mode = Mode;
    //        BindTo.SetBinding(BindToProperty, binding);
    //    }

    //    public static void CreateBinding(DependencyObject BindTo, DependencyProperty BindToProperty, object BindFrom, PropertyPath BindFromProperty, BindingMode Mode = BindingMode.TwoWay)
    //    {
    //        Binding binding = new Binding();
    //        binding.Source = BindFrom;
    //        binding.Path = BindFromProperty;
    //        binding.Mode = Mode;
    //        BindingOperations.SetBinding(BindTo, BindToProperty, binding);
    //    }

    //    public static InfiniteCanvasWPFControl InfiniteCanvasWPFControl { get; internal set; }
    //}


    //public class ElementDataViewTemplate<ElementDataViewWrapper> : DataTemplate<ElementDataViewWrapper>
    //{
    //    private static ListBox LBcontent = null;

    //    public ElementDataViewTemplate() : base()
    //    {
    //        //TODO: Create runtime data template
    //        FrameworkElementFactory r = new FrameworkElementFactory(typeof(Rectangle));
    //        ElementDataViewTemplate<ElementDataViewWrapper>.CreateBinding(r, Rectangle.WidthProperty, new PropertyPath("Width"), BindingMode.TwoWay);
    //        ElementDataViewTemplate<ElementDataViewWrapper>.CreateBinding(r, Rectangle.HeightProperty, new PropertyPath("Height"), BindingMode.TwoWay);
    //        r.SetValue(Rectangle.FillProperty, new SolidColorBrush(Colors.Teal));
    //        r.SetValue(Rectangle.CursorProperty, Cursors.Hand);
    //        //r.AddHandler(Rectangle.MouseMoveEvent, new MouseEventHandler(Rectangle_MouseMove));
    //        //r.AddHandler(Rectangle.MouseDownEvent, new MouseButtonEventHandler(Rectangle_MouseDown));
    //        //r.AddHandler(Rectangle.MouseUpEvent, new MouseButtonEventHandler(Rectangle_MouseUp));
    //        //this.VisualTree = r;
    //        this.VisualTree.AppendChild(r);
    //        if (LBcontent == null)
    //        {
    //            try
    //            {
    //                ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.Resources.Add("DefaultElementDataViewTemplate", this);
    //                LBcontent = (ListBox)ElementDataViewTemplate<ElementDataViewWrapper>.InfiniteCanvasWPFControl.FindName("LBcontent");
    //                LBcontent.ItemsSource = DataViewModel.Instance.Elements;
    //                LBcontent.ItemTemplate = this;
    //            }
    //            catch (Exception)
    //            {
    //                //TODO
    //                Console.WriteLine("Error");
    //                return;
    //            }
    //        }
    //    }



    //}
    
    public class ElementWrapper : FrameworkElement, IElement, IAddChild
    {
        private BoundingBox boundingBox = BoundingBox.Unset;

        private static Type view = null;

        public Type View { get { return view; } }

        public bool IsSelected { get; set; }

        public ElementWrapper()
        {
            ElementTemplate.CreateElementTemplate(this);
        }

        public ElementWrapper(int x, int y, int width, int height)
        {
            this.boundingBox = new BoundingBox(x, y, width, height);
            ElementTemplate.CreateElementTemplate(this);
        }

        public BoundingBox BoundingBox { get => boundingBox; private set => boundingBox = value; }

        public double X { get => boundingBox.Location.X; }

        public double Y { get => boundingBox.Location.Y; }

        public new double Width
        {
            get
            {
                base.Width = boundingBox.Size.Width;
                return boundingBox.Size.Width;
            }
            set
            {
                boundingBox.Size.Width = value;
                base.Width = boundingBox.Size.Width;
            }
        }

        public new double Height
        {
            get
            {
                base.Height = boundingBox.Size.Height;
                return boundingBox.Size.Height;
            }
            set
            {
                boundingBox.Size.Height = value;
                base.Height = boundingBox.Size.Height;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public void AddChild(object value)
        {
            base.AddLogicalChild(value);
        }

        public void AddText(string text)
        {
        }

        public DataTemplate Template { get; internal set; }
    }

    public class ElementTemplate : DataTemplate
    {
        private ElementWrapper _owner;
        public ElementWrapper Owner { get { return _owner; } }
        public static void CreateElementTemplate(ElementWrapper owner)
        {
            ElementTemplate t = new ElementTemplate();
            t._owner = owner;
            //t._owner.Template = DataTemplateManager.CreateTemplateCustom(DataViewModel.WPFControl);
            DataTemplateManager.RegisterDataTemplate<ElementWrapper, Rectangle>();
        }
    }

    #region DataTemplateManager
    public class DataTemplateManager
    {
        public static DataTemplate CreateTemplateCustom(InfiniteCanvasWPFControl c)
        {
            //string xaml = "<DataTemplate DataType=\"{x:Type core:IElement}\"></DataTemplate>";
            //string xaml = "<DataTemplate DataType=\"{x:Type local:ElementWrapper}\"></DataTemplate>";
            string xaml = "<DataTemplate DataType=\"{x:Type local:ElementWrapper}\">" +
                "<Rectangle Width = \"{Binding Width}\" Height = \"{Binding Height}\" Fill = \"#408080\" Cursor = \"Hand\"" +
                //"MouseDown = \"Rectangle_MouseDown\" MouseUp = \"Rectangle_MouseUp\" MouseMove = \"Rectangle_MouseMove\"" +
                "/></DataTemplate>";


            ParserContext context = new ParserContext();

            context.XamlTypeMapper = new XamlTypeMapper(Array.Empty<string>());
            context.XamlTypeMapper.AddMappingProcessingInstruction("core", "Core", "Core");
            context.XamlTypeMapper.AddMappingProcessingInstruction("local", "Verse3", "Verse3");

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("core", "core");
            context.XmlnsDictionary.Add("local", "local");
            //context.XmlnsDictionary.Add("v", "v");

            DataTemplate template = (DataTemplate)XamlReader.Parse(xaml, context);
            template.VisualTree = new FrameworkElementFactory(typeof(ElementWrapper));

            //TODO: Create runtime data template
            //FrameworkElementFactory r = new FrameworkElementFactory(typeof(Rectangle));
            //DataTemplateManager.CreateBinding(r, Rectangle.WidthProperty, new PropertyPath("Width"), BindingMode.TwoWay);
            //DataTemplateManager.CreateBinding(r, Rectangle.HeightProperty, new PropertyPath("Height"), BindingMode.TwoWay);
            //r.SetValue(Rectangle.FillProperty, new SolidColorBrush(Colors.Teal));
            //r.SetValue(Rectangle.CursorProperty, Cursors.Hand);
            ////r.AddHandler(Rectangle.MouseMoveEvent, new MouseEventHandler(Rectangle_MouseMove));
            ////r.AddHandler(Rectangle.MouseDownEvent, new MouseButtonEventHandler(Rectangle_MouseDown));
            ////r.AddHandler(Rectangle.MouseUpEvent, new MouseButtonEventHandler(Rectangle_MouseUp));
            //template.VisualTree.AppendChild(r);

            if (c.Resources[template.DataTemplateKey] != null)
            {
                //template.Seal();
                //template.LoadContent();
                return c.Resources[template.DataTemplateKey] as DataTemplate;
            }
            else
            {
                c.Resources.Add(template.DataTemplateKey, template);
                //template.Seal();
                //template.LoadContent();
                return template;
            }
        }

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
        ////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TView"></typeparam>
        public static void RegisterDataTemplate<TViewModel, TView>() where TView : FrameworkElement
        {
            RegisterDataTemplate(typeof(TViewModel), typeof(TView));
        }

        public static void RegisterDataTemplate(Type viewModelType, Type viewType)
        {
            var template = CreateTemplate(viewModelType, viewType);

            if (DataViewModel.WPFControl.Resources[template.DataTemplateKey] != null)
            {
                //template.Seal();
                //template.LoadContent();
                //return DataViewModel.WPFControl.Resources[template.DataTemplateKey] as DataTemplate;
            }
            else
            {
                DataViewModel.WPFControl.Resources.Add(template.DataTemplateKey, template);
                //template.Seal();
                //template.LoadContent();
                //return template;
            }
        }

        public static DataTemplate CreateTemplate(Type viewModelType, Type viewType)
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

            var template = (DataTemplate)XamlReader.Parse(xaml, context);
            return template;

        }

        internal static void RegisterDataTemplate(IElement el)
        {
            if (el.View == null) return;
            var template = CreateTemplate(el.GetType(), el.View);

            if (DataViewModel.WPFControl.Resources[template.DataTemplateKey] != null)
            {
                //template.Seal();
                //template.LoadContent();
                //return DataViewModel.WPFControl.Resources[template.DataTemplateKey] as DataTemplate;
            }
            else
            {
                DataViewModel.WPFControl.Resources.Add(template.DataTemplateKey, template);
                //template.Seal();
                //template.LoadContent();
                //return template;
            }
        }
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }

    #endregion
}
