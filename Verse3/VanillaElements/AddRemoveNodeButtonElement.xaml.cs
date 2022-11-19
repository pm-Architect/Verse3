using Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Core.Geometry2D;

namespace Verse3.VanillaElements
{
    /// <summary>
    /// Visual Interaction logic for TestElement.xaml
    /// </summary>
    public partial class AddRemoveNodeButtonElementView : UserControl, IBaseElementView<AddRemoveNodeButtonElement>
    {
        #region IBaseElementView Members

        private AddRemoveNodeButtonElement _element;
        public AddRemoveNodeButtonElement Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as AddRemoveNodeButtonElement;
                }
                return _element;
            }
            private set
            {
                _element = value as AddRemoveNodeButtonElement;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render

        public AddRemoveNodeButtonElementView()
        {
            InitializeComponent();
        }

        public void Render()
        {
            if (this.Element != null)
            {
                if (this.Element.AllowRearrangement)
                {
                    this.MoveUpBtn.Visibility = Visibility.Visible;
                    this.MoveDownBtn.Visibility = Visibility.Visible;
                }
                else
                {
                    this.MoveUpBtn.Visibility = Visibility.Hidden;
                    this.MoveDownBtn.Visibility = Visibility.Hidden;
                }
                if (this.Element.IsFirst)
                {
                    this.RemoveBtn.Visibility = Visibility.Hidden;
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
            Element = this.DataContext as AddRemoveNodeButtonElement;
            Render();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Element = this.DataContext as AddRemoveNodeButtonElement;
            Render();
        }

        #endregion

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Element.RemoveClicked(sender, e);
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Element.AddClicked(sender, e);
        }
        private void MoveUpBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Element.MoveUpClicked(sender, e);
        }
        private void MoveDownBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Element.MoveDownClicked(sender, e);
        }
    }

    [Serializable]
    public class AddRemoveNodeButtonElement : BaseElement
    {

        #region Properties

        public event EventHandler<RoutedEventArgs> OnRemoveClicked;
        public event EventHandler<RoutedEventArgs> OnAddClicked;
        public event EventHandler<RoutedEventArgs> OnMoveUpClicked;
        public event EventHandler<RoutedEventArgs> OnMoveDownClicked;

        public override Type ViewType => typeof(AddRemoveNodeButtonElementView);

        public override ElementType ElementType { get => ElementType.Node; set => base.ElementType = ElementType.Node; }

        public bool AllowRearrangement { get; internal set; }
        public bool IsFirst { get; private set; }

        private INode _owner;

        #endregion

        #region Constructors
        
        //public AddRemoveNodeButtonElement() : base()
        //{
        //    this.AllowRearrangement = false;
        //}
        
        public AddRemoveNodeButtonElement(IDataNode ownerNode, bool allowRearrangement = false, bool isFirst = true) : base()
        {
            this.AllowRearrangement = allowRearrangement;
            this.IsFirst = isFirst;
            this._owner = ownerNode;
        }

        #endregion
        
        internal void RemoveClicked(object sender, RoutedEventArgs e)
        {
            if (!this.IsFirst) OnRemoveClicked.Invoke(sender, e);
        }
        internal void AddClicked(object sender, RoutedEventArgs e)
        {
            OnAddClicked.Invoke(sender, e);
        }
        internal void MoveUpClicked(object sender, RoutedEventArgs e)
        {
            if (this.AllowRearrangement) OnMoveUpClicked.Invoke(sender, e);
        }
        internal void MoveDownClicked(object sender, RoutedEventArgs e)
        {
            if (this.AllowRearrangement) OnMoveDownClicked.Invoke(sender, e);
        }

    }
}