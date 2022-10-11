using Core;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Supabase;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Verse3.VanillaElements;
using static Core.Geometry2D;
using XamlReader = System.Windows.Markup.XamlReader;

namespace Verse3
{
    [Serializable]
    public class VFSerializable : ISerializable
    {
        public DataViewModel DataViewModel { get; set; }

        public VFSerializable(DataViewModel dataViewModel)
        {
            DataViewModel = dataViewModel;
        }

        public VFSerializable(SerializationInfo info, StreamingContext context)
        {
            DataViewModel = (DataViewModel)info.GetValue("DataViewModel", typeof(DataViewModel));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("DataViewModel", DataViewModel);
        }
        
        public void Serialize(string path)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            //finally
            //{
            //    try
            //    {
            //        Supabase.Gotrue.User user = Client.Instance.Auth.CurrentUser;
                    
            //        var file = ShellFile.FromFilePath(path);

            //        // Read and Write:

            //        //string[] oldAuthors = file.Properties.System.Author.Value;
            //        //string oldTitle = file.Properties.System.Title.Value;

            //        //file.Properties.System.Author.Value = new string[] { "Author #1", "Author #2" };
            //        //file.Properties.System.Title.Value = "Example Title";

            //        // Alternate way to Write:

            //        ShellPropertyWriter propertyWriter = file.Properties.GetPropertyWriter();

            //        string authorId = "DEVELOPER";
            //        if (user != null)
            //        {
            //            authorId = user.Id;
            //        }
            //        propertyWriter.WriteProperty(SystemProperties.System.Author, new string[] { "AuthorID::" + authorId });
            //        propertyWriter.Close();
            //    }
            //    catch (Exception ex1)
            //    {
            //        throw ex1;
            //    }
            //}
        }

        public static VFSerializable Deserialize(string path)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                    formatter.FilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                    //Supabase.Gotrue.User user = Client.Instance.Auth.CurrentUser;

                    //var file = ShellFile.FromFilePath(path);

                    //string oldAuthor = file.Properties.System.Author.Value[0];
                    //string authorId = "DEVELOPER";
                    //if (user != null)
                    //{
                    //    authorId = user.Id;
                    //}
                    //if (oldAuthor == ("AuthorID::" + authorId))
                    //{
                    //    System.Diagnostics.Debug.WriteLine("File created by " + oldAuthor);
                    //    if (authorId != "DEVELOPER") return null;
                    //}
                    return (VFSerializable)formatter.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
    
    /// <summary>
    /// A simple example of a data-model.  
    /// The purpose of this data-model is to share display data between the main window and overview window.
    /// </summary>
    [Serializable]
    public class DataViewModel : DataModel
    {
        public static InfiniteCanvasWPFControl WPFControl { get; private set; }
        public static INode ActiveNode { get; internal set; }
        public static IConnection ActiveConnection { get; internal set; }

        private static Dispatcher dispatcher = null;
        public static Dispatcher Dispatcher { get => dispatcher; }

        //protected static DataViewModel instance = new DataViewModel();
        public new static DataModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataViewModel();
                    DataModel.Instance = instance;
                }
                return DataModel.instance;
            }
            internal set
            {
                instance = value;
                //DataModel.Instance = instance;
            }
        }

        //public static void AddElement(IElement e)
        //{
        //    try
        //    {
        //        Action addElement = () =>
        //        {
        //            DataViewModel.Instance.Elements.Add(e);
        //        };
        //        if (dispatcher != null)
        //        {
        //            dispatcher.BeginInvoke(addElement);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        
        private DataViewModel() : base()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public static void InitDataViewModel(InfiniteCanvasWPFControl c)
        {
            if (DataViewModel.WPFControl == null)
                DataViewModel.WPFControl = c;
            //if (Program.Dispatcher != null && dispatcher == null)
            //    dispatcher = Program.Dispatcher;


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


    [Serializable]
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
        
        public IRenderable Parent => this.RenderPipelineInfo.Parent;

        public ElementsLinkedList<IRenderable> Children => this.RenderPipelineInfo.Children;

        public void SetX(double x)
        {
            BoundingBox.Location.X = x;
            OnPropertyChanged("X");
        }

        public void SetY(double y)
        {
            BoundingBox.Location.Y = y;
            OnPropertyChanged("Y");
        }

        public void SetWidth(double x)
        {
            BoundingBox.Size.Width = x;
            OnPropertyChanged("Width");
        }

        public void SetHeight(double x)
        {
            BoundingBox.Size.Height = x;
            OnPropertyChanged("Height");
        }

        public void Render()
        {
            if (RenderView != null)
                RenderView.Render();
        }

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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                //info.AddValue("ID", this.ID);
                info.AddValue("X", this.X);
                info.AddValue("Y", this.Y);
                info.AddValue("Width", this.Width);
                info.AddValue("Height", this.Height);
                info.AddValue("ElementType", this.ElementType);
                info.AddValue("State", this.State);
                info.AddValue("IsSelected", this.IsSelected);
                info.AddValue("BoundingBox", this.BoundingBox);
                info.AddValue("ElementState", this.ElementState);
                info.AddValue("Parent", this.Parent);
                info.AddValue("Children", this.Children);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        //TODO: Allow calling this method from another thread
        public static bool RegisterDataTemplate(IRenderable el)
        {
            if (el == null) return false;
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
                    return true;
                }
                else if (el.ViewType.IsAssignableTo(typeof(EventNodeElementView)))
                {
                    DataViewModel.WPFControl.Resources.Add(el.ViewKey, template);
                    return true;
                }
                return false;
            }
            else
            {
                try
                {
                    Action addTemplate = () =>
                    {
                        DataViewModel.WPFControl.Resources.Add(el.ViewKey, template);
                    };
                    DataViewModel.WPFControl.Dispatcher.Invoke(addTemplate);
                    //ERROR: The calling thread cannot access this object because a different thread owns it.
                    //DataViewModel.WPFControl.Resources.Add(el.ViewKey, template);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
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
