using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Verse3;
using static Core.Geometry2D;
using Verse3.VanillaElements;
using TextElement = Verse3.VanillaElements.TextElement;

namespace TestPlugin
{
    /// <summary>
    /// Visual Interaction logic for TestElement.xaml
    /// </summary>
    public partial class TestElementView : UserControl, IRenderView
    {
        private ObservableCollection<IRenderable>? _children;
        private TestElement? _element;
        
        public ObservableCollection<IRenderable>? Children
        {
            get { return _children; }
            private set { _children = value; }
        }
        
        public IRenderable? Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as TestElement;
                }
                return _element;
            }
            private set
            {
                _element = value as TestElement;
                //Update();
            }
        }
        public Guid? ElementGuid
        {
            get { return Element?.ID; }
        }

        public TestElementView()
        {
            InitializeComponent();
            Render();
        }

        TextElement textBlock = new TextElement();

        public void Render()
        {
            //< TextBlock HorizontalAlignment = "Center"
            //       TextWrapping = "Wrap"
            //       Text = "{Binding ElementText}"
            //       VerticalAlignment = "Center"
            //       FontFamily = "Maven Pro"
            //       FontSize = "18"
            //       />

            if (Children == null)
            {
                Children = new ObservableCollection<IRenderable>();
                InputsList.ItemsSource = Children;
            }

            if (this.Element is TestElement)
            {
                TestElement testelement = (TestElement)this.Element;

                var nodeBlock = new NodeElement(testelement);
                DataTemplateManager.RegisterDataTemplate(nodeBlock);                
                Children.Add(nodeBlock);


                string? txt = testelement.ElementText;
                textBlock = new TextElement();
                textBlock.DisplayedText = txt;
                textBlock.TextAlignment = TextAlignment.Left;
                DataTemplateManager.RegisterDataTemplate(textBlock);
                Children.Add(textBlock);

                var sliderBlock = new SliderElement();
                sliderBlock.Minimum = 0;
                sliderBlock.Maximum = 100;
                sliderBlock.Value = 50;
                DataTemplateManager.RegisterDataTemplate(sliderBlock);
                Children.Add(sliderBlock);

                var buttonBlock = new ButtonElement();
                buttonBlock.DisplayedText = "Click me";
                buttonBlock.OnButtonClicked += ButtonBlock_OnButtonClicked;
                DataTemplateManager.RegisterDataTemplate(buttonBlock);
                Children.Add(buttonBlock);

                var textBoxBlock = new TextBoxElement();
                textBoxBlock.InputText = "Enter text";
                DataTemplateManager.RegisterDataTemplate(textBoxBlock);
                Children.Add(textBoxBlock);
            }

        }

        private void ButtonBlock_OnButtonClicked(object? sender, RoutedEventArgs e)
        {
            TestElement? testelement = this.Element as TestElement;
            if (testelement != null)
            {
                string? txt = testelement.ElementText;
                textBlock.DisplayedText = txt;
            }
        }

        #region MouseEvents

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //MouseButtonEventArgs
            DataViewModel.WPFControl.ContentElements.Focus();
            Keyboard.Focus(DataViewModel.WPFControl.ContentElements);

            TestElementView rectangle = (TestElementView)sender;
            IRenderable myRectangle = (IRenderable)rectangle.DataContext;

            //myRectangle.IsSelected = true;

            //mouseButtonDown = e.ChangedButton;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                //
                // When the shift key is held down special zooming logic is executed in content_MouseDown,
                // so don't handle mouse input here.
                //
                return;
            }

            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.None)
            {
                //
                // We are in some other mouse handling mode, don't do anything.
                return;
            }

            DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.DraggingRectangles;
            DataViewModel.WPFControl.origContentMouseDownPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);

            rectangle.CaptureMouse();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //MouseButtonEventArgs
            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingRectangles)
            {
                //
                // We are not in rectangle dragging mode.
                //
                return;
            }

            DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.None;

            TestElementView rectangle = (TestElementView)sender;
            rectangle.ReleaseMouseCapture();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            //MouseEventArgs
            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingRectangles)
            {
                //
                // We are not in rectangle dragging mode, so don't do anything.
                //
                return;
            }

            Point curContentPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);
            Vector rectangleDragVector = curContentPoint - DataViewModel.WPFControl.origContentMouseDownPoint;

            //
            // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
            //

            DataViewModel.WPFControl.origContentMouseDownPoint = curContentPoint;

            TestElementView rectangle = (TestElementView)sender;
            IRenderable myRectangle = (IRenderable)rectangle.DataContext;
            myRectangle.SetX(myRectangle.X + rectangleDragVector.X);
            myRectangle.SetY(myRectangle.Y + rectangleDragVector.Y);

            DataViewModel.WPFControl.ExpandContent();

            e.Handled = true;
        }

        void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //MouseWheelEventArgs
        }

        #endregion

        #region UserControlEvents

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //DependencyPropertyChangedEventArgs
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Render();
        }

        #endregion
    }

    public class TestElement : IRenderable
    {
        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                //string? zindex = DataViewModel.WPFControl.Content.
                //TODO: Z Index control for IRenderable
                return $"Name: {name}" +
                    $"\nView: {viewname}" +
                    $"\nID: {this.ID}" +
                    $"\nX: {this.X}" +
                    $"\nY: {this.Y}";
            }
        }
        
        #region Data Members

        private BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        private static Type view = typeof(TestElementView);
        internal TestElementView elView;
        public IRenderView RenderView
        {
            get
            {
                return elView;
            }
            set
            {
                if (value is TestElementView)
                {
                    elView = (TestElementView)value;
                }
            }
        }

        #endregion

        #region Properties

        public Type ViewType { get { return view; } }
        public object ViewKey { get; set; }

        public Guid ID { get => _id; private set => _id = value; }

        public bool IsSelected { get; set; }

        public BoundingBox BoundingBox
        {
            get => boundingBox;
            private set => SetProperty(ref boundingBox, value);
        }

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

        public Guid ZPrev { get; }

        public Guid ZNext { get; }

        public Guid Parent { get; }

        public Guid[]? Children { get; }

        public ElementState State { get; set; }

        //public IRenderView? ElementView { get; }

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get; set; }
        bool IRenderable.Visible { get; set; }

        #endregion

        #region Constructors

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        
        public TestElement()
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public TestElement(int x, int y, int width, int height)
        {
            this.BoundingBox = new BoundingBox(x, y, width, height);

            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            Random rnd = new Random();
            byte rc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            byte gc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            byte bc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            this.backgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
        }
        
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
                //this.boundingBox.PropertyChanged += this.PropertyChanged;
            }
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        private Brush background;

        public Brush Background { get => background; set => SetProperty(ref background, value); }

        private Brush backgroundTint;

        public Brush BackgroundTint { get => backgroundTint; set => SetProperty(ref backgroundTint, value); }

        #endregion
    }
}