using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        DraggingRectangles,

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
        /// The user is holding down shift and left-mouse-button-dragging to select a region to zoom to.
        /// </summary>
        BezierPenDown,

        /// <summary>
        /// The user is left-mouse-button-dragging on the viewport.
        /// </summary>
        Dragging
    }
}
