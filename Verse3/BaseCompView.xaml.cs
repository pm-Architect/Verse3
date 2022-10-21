using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Verse3.VanillaElements;

namespace Verse3
{
    /// <summary>
    /// Interaction logic for BaseCompView.xaml
    /// </summary>
    public partial class BaseCompView : UserControl, IBaseCompView<BaseComp>
    {


        #region IBaseElementView Members

        private Type Y = default;
        //DEV NOTE: CAUTION! DYNAMIC TYPE IS USED
        private dynamic _element;
        public BaseComp Element
        {
            get
            {
                if (this._element == null)
                {
                    if (this.DataContext != null)
                    {
                        Y = this.DataContext.GetType();
                        if (Y.BaseType.Name == (typeof(BaseComp).Name))
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

        #endregion
        public BaseCompView()
        {
            if (this.DataContext is BaseComp) this.Element = (BaseComp)this.DataContext;
            InitializeComponent();
            Render();
        }

        public void Render()
        {
            if (this.Element != null)
            {
                if (this.Element.RenderView != this) this.Element.RenderView = this;
                this.Element.RenderComp();
                
                //InputsList.ItemsSource = this.Element.ChildElementManager.InputSide;
                //OutputsList.ItemsSource = this.Element.ChildElementManager.OutputSide;
                //if (this.Element.Width != (InputsList.ActualWidth + OutputsList.ActualWidth + CenterBar.Width))
                //{


                //    else
                //    {
                //        CenterBar.Width = 50;
                //        this.Element.Width = (InputsList.ActualWidth + OutputsList.ActualWidth + CenterBar.Width);
                //    }
                //}
                //if (this.Element.Height != MainStackPanel.ActualHeight) this.Element.Height = MainStackPanel.ActualHeight;
                //TODO: else log to console
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

            BaseCompView compView = (BaseCompView)sender;
            BaseComp comp = compView.Element;



            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 && e.ChangedButton == MouseButton.Left)
            {
                DataViewModel.WPFControl.AddToSelection(comp);
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) != 0 && e.ChangedButton == MouseButton.Left)
            {
                DataViewModel.WPFControl.Deselect(comp);
            }
            else
            {
                if (!comp.IsSelected) DataViewModel.WPFControl.Select(comp);
            }

            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0 && e.ChangedButton == MouseButton.Left && DataViewModel.WPFControl.ContentElements.SelectedItems.Count > 0)
            {
                DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.CopyDraggingElements;
                DataViewModel.WPFControl.origContentMouseDownPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);
                DataViewModel.WPFControl.mouseButtonDown = e.ChangedButton;

                compView.CaptureMouse();

                e.Handled = true;
                return;
            }

            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.None)
            {
                //
                // We are in some other mouse handling mode, don't do anything.
                return;
            }

            DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.DraggingElements;
            DataViewModel.WPFControl.origContentMouseDownPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);
            DataViewModel.WPFControl.mouseButtonDown = e.ChangedButton;

            compView.CaptureMouse();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //MouseButtonEventArgs
            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingElements)
            {
                //
                // We are not in rectangle dragging mode.
                //
                return;
            }

            DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.None;

            BaseCompView rectangle = (BaseCompView)sender;
            rectangle.ReleaseMouseCapture();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            //MouseEventArgs
            if (DataViewModel.WPFControl.MouseHandlingMode == MouseHandlingMode.CopyDraggingElements && DataViewModel.WPFControl.ContentElements.SelectedItems.Count > 0)
            {
                List<BaseComp> copiedComps = new List<BaseComp>();
                foreach (BaseComp comp in DataViewModel.WPFControl.ContentElements.SelectedItems)
                {
                    BaseComp comp1 = Activator.CreateInstance(comp.GetType()) as BaseComp;
                    ParameterInfo[] pi = comp1.GetCompInfo().ConstructorInfo.GetParameters();
                    object[] args = new object[pi.Length];
                    for (int i = 0; i < pi.Length; i++)
                    {
                        if (!(pi[i].DefaultValue is DBNull)) args[i] = pi[i].DefaultValue;
                        else
                        {
                            if (pi[i].ParameterType == typeof(int) && pi[i].Name.ToLower() == "x")
                                args[i] = DataViewModel.WPFControl.GetMouseRelPosition().X;
                            //args[i] = InfiniteCanvasWPFControl.GetMouseRelPosition().X;
                            else if (pi[i].ParameterType == typeof(int) && pi[i].Name.ToLower() == "y")
                                args[i] = DataViewModel.WPFControl.GetMouseRelPosition().Y;
                            //args[i] = InfiniteCanvasWPFControl.GetMouseRelPosition().Y;
                        }
                    }
                    BaseComp comp1instance = comp1.GetCompInfo().ConstructorInfo.Invoke(args) as BaseComp;
                    DataTemplateManager.RegisterDataTemplate(comp1instance);
                    DataViewModel.Instance.Elements.Add(comp1instance);

                    copiedComps.Add(comp1instance);
                }
                DataViewModel.WPFControl.ClearSelection();
                foreach (BaseComp comp in copiedComps) DataViewModel.WPFControl.AddToSelection(comp);

                DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.DraggingElements;
            }
            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingElements)
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

            //TODO: If other BaseComps are also selected, Render all selected BaseComps together with rectangleDragVector translation
            if (DataViewModel.WPFControl.LBcontent.SelectedItems.Count > 1)
            {
                foreach (IRenderable renderable in DataViewModel.WPFControl.LBcontent.SelectedItems)
                {
                    if (renderable != null)
                    {
                        RenderPipeline.RenderRenderable(renderable, rectangleDragVector.X, rectangleDragVector.Y);
                    }
                }
            }
            else
            {
                RenderPipeline.RenderRenderable(this.Element, rectangleDragVector.X, rectangleDragVector.Y);
            }

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
            Render();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Render();
        }

        #endregion
    }
}
