using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

namespace Verse3.Elements
{
    /// <summary>
    /// Visual Interaction logic for TestElement.xaml
    /// </summary>
    public partial class TextElementView : UserControl, IRenderView
    {
        //public string DisplayedText { get; set; }

        //private ObservableCollection<IRenderable> _children;
        private IRenderable _element;
        
        //public ObservableCollection<IRenderable> Children
        //{
        //    get { return _children; }
        //    private set { _children = value; }
        //}
        
        public IRenderable Element
        {
            get { return _element; }
            private set
            {
                _element = value;
                //Update();
            }
        }
        public Guid? ElementGuid
        {
            get { return _element?.ID; }
        }

        public TextElementView()
        {
            _element = this.DataContext as IRenderable;

            InitializeComponent();
            Render();
        }

        public void Render()
        {
            //< TextBlock HorizontalAlignment = "Center"
            //       TextWrapping = "Wrap"
            //       Text = "{Binding ElementText}"
            //       VerticalAlignment = "Center"
            //       FontFamily = "Maven Pro"
            //       FontSize = "18"
            //       />

            //var textBlock = new TextBlock();
            //textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            //textBlock.TextWrapping = TextWrapping.Wrap;
            //TestElement? element = _element as TestElement;
            //if (element != null)
            //{
            //    textBlock.Text = element.ElementText;
            //}
            //textBlock.VerticalAlignment = VerticalAlignment.Center;
            //textBlock.FontFamily = new FontFamily("Maven Pro");
            //textBlock.FontSize = 18;

            //if (Children == null)
            //{
            //    Children = new ObservableCollection<IRenderable>();
            //    ListBox.ItemsSource = Children;
            //}
            //Children.Add(textBlock);

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

            TextElementView rectangle = (TextElementView)sender;
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

            TextElementView rectangle = (TextElementView)sender;
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

            TextElementView rectangle = (TextElementView)sender;
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

    public class TextElement : IRenderable
    {
        public string DisplayedText { get; set; }
        
        #region Data Members

        private BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        private static Type view = typeof(TextElementView);

        #endregion

        #region Properties

        public Type ViewType { get { return view; } }

        public Guid ID { get => _id; private set => _id = value; }

        public bool IsSelected { get; set; }

        public BoundingBox BoundingBox { get => boundingBox; private set => boundingBox = value; }

        public double X { get => boundingBox.Location.X; }

        public double Y { get => boundingBox.Location.Y; }

        public double Width => boundingBox.Size.Width;

        public double Height => boundingBox.Size.Height;

        public Guid ZPrev { get; }

        public Guid ZNext { get; }

        public Guid Parent { get; }

        public Guid[] Children { get; }

        public ElementState State { get; set; }

        public IRenderView ElementView { get; }

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get; set; }

        #endregion

        #region Constructors

        public TextElement()
        {
        }

        public TextElement(int x, int y, int width, int height)
        {
            this.boundingBox = new BoundingBox(x, y, width, height);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}