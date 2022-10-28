using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class Observable : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the 'PropertyChanged' event when the value of a property of the data model has changed.
        /// </summary>
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// 'PropertyChanged' event that is raised when the value of a property of the data model has changed.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
        
    public class HttpServer
    {
        private HttpListener _listener;

        public void Start()
        {
            if (!HttpListener.IsSupported)
            {
                CoreConsole.Log("Not Supported");
                return;
            }
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:3000/");
            _listener.Start();
            Receive();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private void Receive()
        {
            _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            if (_listener.IsListening)
            {
                var context = _listener.EndGetContext(result);
                string u = context.Request.ToString();
                var request = context.Request;

                CoreConsole.Log($"{request.RawUrl}");
                if (request.QueryString.Count > 0)
                {
                    string t = request.QueryString.Get(0);
                    Core.SetAuth(request.Url);
                    var response = context.Response;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "text/html";
                    using (var writer = new StreamWriter(response.OutputStream))
                    {
                        writer.WriteLine("<html><body>Authentication successful. Welcome to Project Verse! You may close this window." +
                            "<br><br>Data collected or submitted by you will be anonymized and used as part of a user study being conducted for a research paper." +
                            "<br><br>If you have any questions or concerns, feel free to reach out: <a href=\"mailto:prjvrs@iiterate.de\">prjvrs@iiterate.de</a>" +
                            "</body></html>");
                    }
                    response.OutputStream.Close();
                    _listener.Stop();
                    return;
                }
                else
                {
                    var response = context.Response;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "text/html";
                    using (var writer = new StreamWriter(response.OutputStream))
                    {
                        writer.WriteLine("<html><body><script> if (window.location.hash) { console.log(window.location.hash); window.location.href = window.location.toString().replace(\"#\", \"?\"); } </script></body></html>");
                    }
                    response.OutputStream.Close();
                }

                Receive();
            }
        }
    }

    /// <summary>
    /// Defines the current state of the mouse handling logic.
    /// </summary>
    public enum MouseHandlingMode
    {
        /// <summary>
        /// Not in any special mode.
        /// </summary>
        None,

        /// <summary>
        /// The user is left-dragging rectangles with the mouse.
        /// </summary>
        DraggingElements,

        /// <summary>
        /// The user is left-mouse-button-dragging to pan the viewport.
        /// </summary>
        Panning,

        /// <summary>
        /// The user is holding down shift and left-clicking or right-clicking to zoom in or out.
        /// </summary>
        Zooming,

        /// <summary>
        /// The user is holding down shift and left-clicking or right-clicking to zoom in or out.
        /// </summary>
        Selecting,

        /// <summary>
        /// The user is holding down shift and left-mouse-button-dragging to select a region to zoom to.
        /// </summary>
        DragZooming,

        /// <summary>
        /// The user is holding down shift and left-mouse-button-dragging to select a region to zoom to.
        /// </summary>
        DragSelecting,

        /// <summary>
        /// The user is left-mouse-button-dragging on the viewport.
        /// </summary>
        Dragging,
        ConnectionStarted,
        CopyDraggingElements
    }
}
