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
        public static void InitDataViewModel(InfiniteCanvasWPFControl c)
        {
            DataViewModel.WPFControl = c;
            
            //TODO Properly Load all available plugins            

            //TODO: Open a file here!!!!

            //
            // TODO: Populate the data model with file data
            //
        }
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

        private static void RegisterDataTemplate<TViewModel, TView>() where TView : FrameworkElement
        {
            RegisterDataTemplate(typeof(TViewModel), typeof(TView));
        }

        private static void RegisterDataTemplate(Type viewModelType, Type viewType)
        {
            var template = CreateTemplate(viewModelType, viewType);

            if (DataViewModel.WPFControl.Resources[template.DataTemplateKey] != null) return;
            else
            {
                DataViewModel.WPFControl.Resources.Add(template.DataTemplateKey, template);
            }
        }

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

            var template = (DataTemplate)XamlReader.Parse(xaml, context);
            //TODO: Inform template class about its owner

            return template;

        }

        #endregion

        #region Public DataTemplate Management

        public static bool RegisterDataTemplate(IRenderable el)
        {
            if (el.ViewType == null) return false;
            var template = CreateTemplate(el.GetType(), el.ViewType);

            if (DataViewModel.WPFControl.Resources[template.DataTemplateKey] != null) return false;
            else
            {
                DataViewModel.WPFControl.Resources.Add(template.DataTemplateKey, template);
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

    //public class ElementHeightPseudoConverter : IValueConverter
    //{
    //    public static double Padding { get; set; } = 2.5;
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        double h = 0.0;
    //        if (parameter is ListBox)
    //        {
    //            ListBox listBox = parameter as ListBox;
    //            foreach (object o in listBox.ItemsSource)
    //            {
    //                if (o is IRenderable)
    //                {
    //                    IRenderable r = o as IRenderable;
    //                    if (r.BoundingBox.IsValid())
    //                    {
    //                        h += Padding;
    //                        h += r.Height;
    //                        h += Padding;
    //                    }
    //                }
    //            }
    //            //object o = listBox.Template.FindName("CompCanvas", listBox);
    //            //if (o is Canvas)
    //            //{
    //            //    Canvas canvas = o as Canvas;
    //            //    return canvas.ActualHeight;
    //            //}
    //        }
    //        return h;
    //    }
    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    #endregion
}
