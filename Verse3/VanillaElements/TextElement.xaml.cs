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
    public partial class TextElementView : UserControl, IBaseElementView<TextElement>
    {
        #region IBaseElementView Members

        private TextElement _element;
        public TextElement Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as TextElement;
                }
                return _element;
            }
            private set
            {
                _element = value as TextElement;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render
        
        public TextElementView()
        {
            InitializeComponent();
        }

        public void Render()
        {
            if (this.Element != null)
            {
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
            Element = this.DataContext as TextElement;
            Render();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Element = this.DataContext as TextElement;
            Render();
        }

        #endregion
    }

    public class TextElement : BaseElement
    {
        #region Properties
                
        //public override ElementType ElementType => ElementType.DisplayUIElement;
        public override Type ViewType { get { return typeof(TextElementView); } }
        
        
        private TextAlignment textAlignment;
        public TextAlignment TextAlignment { get => textAlignment; set => SetProperty(ref textAlignment, value); }
        
        private double textRotation;
        public double TextRotation { get => textRotation; set => SetProperty(ref textRotation, value); }

        private string displayedText;
        public string DisplayedText { get => displayedText; set => SetProperty(ref displayedText, value); }

        private FontStyle fontStyle;
        public FontStyle FontStyle { get => fontStyle; set => SetProperty(ref fontStyle, value); }

        private FontFamily fontFamily;
        public FontFamily FontFamily { get => fontFamily; set => SetProperty(ref fontFamily, value); }

        private double fontSize;
        public double FontSize { get => fontSize; set => SetProperty(ref fontSize, value); }

        private FontWeight fontWeight;
        public FontWeight FontWeight { get => fontWeight; set => SetProperty(ref fontWeight, value); }

        private Brush foreground;
        public Brush Foreground { get => foreground; set => SetProperty(ref foreground, value); }

        private Brush background;
        public Brush Background { get => background; set => SetProperty(ref background, value); }

        #endregion

        #region Constructors

        public TextElement() : base()
        {
            this.FontFamily = new FontFamily("Maven Pro");
            this.FontSize = 12;
            this.FontStyle = FontStyles.Normal;
            this.FontWeight = FontWeights.Normal;
            this.Foreground = Brushes.White;
            this.Background = Brushes.Transparent;
            this.TextAlignment = TextAlignment.Center;
            this.TextRotation = 0;
        }


        #endregion
    }
}