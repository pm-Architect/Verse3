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
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xaml;
using System.Xaml.Schema;
using Core;

namespace Verse3
{
    /// <summary>
    /// A simple example of a data-model.  
    /// The purpose of this data-model is to share display data between the main window and overview window.
    /// </summary>
    public class DataViewModel : DataModel
    {
        public static void CreateDataViewModel(string path)
        {
            //TODO: Open a file here!!!!

            DataViewModel.Instance = new DataViewModel();
            //
            // TODO: Populate the data model with file data
            //
            DataViewModel.Instance.Elements.Add(new ElementData(50, 50, 80, 150));
            DataViewModel.Instance.Elements.Add(new ElementData(550, 350, 80, 150));
            DataViewModel.Instance.Elements.Add(new ElementData(850, 850, 30, 20));
            DataViewModel.Instance.Elements.Add(new ElementData(1200, 1200, 80, 150));
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
