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
            bool rtl = (start.NodeType == NodeType.Input);
            BezierElement bezier = new BezierElement(start, end);
            DataTemplateManager.RegisterDataTemplate(bezier as IRenderable);
            DataViewModel.Instance.Elements.Add(bezier);
            //start.Connections.Add(bezier);
            //end.Connections.Add(bezier);
            return bezier;
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

            DataTemplate template = (DataTemplate)XamlReader.Parse(xaml, context);

            return template;

        }

        #endregion

        #region Public DataTemplate Management

        public static bool RegisterDataTemplate(IRenderable el)
        {
            if (el.ViewType == null) return false;
            if (!el.BoundingBox.IsValid())
            {
                //TODO: Log to console!
            }
            var template = CreateTemplate(el.GetType(), el.ViewType);
            //el.BoundingBox = new BoundingBox();
            //Element needs to know DataTemplateKey in order to make a reference to it
            el.ViewKey = template.DataTemplateKey;
            if (DataViewModel.WPFControl.Resources[el.ViewKey] != null) return false;
            else
            {
                DataViewModel.WPFControl.Resources.Add(el.ViewKey, template);
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

    #endregion
}
