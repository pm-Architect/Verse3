using Core;
using System;
using System.Collections.Generic;
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

namespace VanillaElements
{
    /// <summary>
    /// Interaction logic for IDEElement.xaml
    /// </summary>
    public partial class IDEElementView : UserControl, IBaseElementView<IDEElement>
    {
        #region IBaseElementView Members

        private IDEElement _element;
        public IDEElement Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as IDEElement;
                }
                return _element;
            }
            private set
            {
                _element = value as IDEElement;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render

        public IDEElementView()
        {
            InitializeComponent();
            EmulatedIDEBrowser.WebMessageReceived += WebMessageReceived;
            //Get path of MonacoEditor folder in AppData
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Verse3\\MonacoEditor";
            this.EmulatedIDEBrowser.Source = new Uri(System.IO.Path.Combine(path, "index.html"));
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
        /// Event raised when the mouse is moved over a Rectangle.
        /// </summary>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
        }

        #endregion

        #region KeyboardEvents

        /// <summary>
        /// Event raised when a key is pressed down.
        /// </summary>
        void OnKeyDown(object sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when a key is released.
        /// </summary>
        void OnKeyUp(object sender, KeyEventArgs e)
        {
        }

        #endregion

        #region UserControlEvents

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //DependencyPropertyChangedEventArgs
            Element = this.DataContext as IDEElement;
            Render();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Element = this.DataContext as IDEElement;
            Render();
        }

        #endregion

        public void ExecuteJS(string js)
        {
            //start task of EmulatedIDEBrowser.ExecuteScriptAsync(js) in a parallel thread and wait for it to finish
            Task<string> task = EmulatedIDEBrowser.ExecuteScriptAsync(js)/*.ContinueWith(t => OnExecutionCompleted(t))*/;
            //task.Start();
            task.WaitAsync(new TimeSpan(5000)).ContinueWith(t => OnExecutionCompleted(t));
            //return task.Result;
        }

        public string OnExecutionCompleted(Task<string> t)
        {
            if (t.IsFaulted)
            {
                Console.WriteLine(t.Exception.ToString());
                return null;
            }
            else
            {
                Console.WriteLine(t.Result.ToString());
                this.Element._script = t.Result.ToString();
                return t.Result.ToString();
            }
        }

        public void GetScript()
        {
            ExecuteJS("getValue();");
        }

        private void WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            Console.WriteLine(e.TryGetWebMessageAsString());
            this.Element._script = e.TryGetWebMessageAsString();
        }
    }

    public class IDEElement : BaseElement
    {
        public override Type ViewType => typeof(IDEElementView);

        public IDEElement() : base()
        {
        }

        internal string _script = "";
        public string Script
        {
            get
            {
                if (this.RenderView != null)
                {
                    (this.RenderView as IDEElementView).GetScript();
                }
                return _script;
            }
        }
    }
}
