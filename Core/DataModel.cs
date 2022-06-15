using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Geometry2D;

namespace Core
{
    /// <summary>
    /// A simple example of a data-model.  
    /// The purpose of this data-model is to share display data between the main window and overview window.
    /// </summary>
    public class DataModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the 'PropertyChanged' event when the value of a property of the data model has changed.
        /// </summary>
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// 'PropertyChanged' event that is raised when the value of a property of the data model has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        #region Data Members

        /// <summary>
        /// The singleton instance.
        /// This is a singleton for convenience.
        /// </summary>
        protected static DataModel instance = new DataModel();

        /// <summary>
        /// The list of rectangles that is displayed both in the main window and in the overview window.
        /// </summary>
        protected ObservableCollection<IElement> elements = new ObservableCollection<IElement>();

        ///
        /// The current scale at which the content is being viewed.
        /// 
        protected double contentScale = 1;

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        protected double contentOffsetX = 0;

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        protected double contentOffsetY = 0;

        ///
        /// The width of the content (in content coordinates).
        /// 
        protected double contentWidth = 0;

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        protected double contentHeight = 0;

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        protected double contentViewportWidth = 0;

        ///
        /// The heigth of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        protected double contentViewportHeight = 0;

        #endregion Data Members

        #region Constructors

        /// <summary>
        /// Retreive the singleton instance.
        /// </summary>
        public static DataModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataModel();
                }
                return DataModel.instance;
            }
            protected set
            {
                instance = value;
            }
        }
        
        public static double ContentCanvasMarginOffset = 200.0;

        public DataModel() : base()
        {
            //
            // Initialize the data model.
            //
            DataModel.instance = this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The list of rectangles that is displayed both in the main window and in the overview window.
        /// </summary>
        public ObservableCollection<IElement> Elements
        {
            get
            {
                return elements;
            }
            protected set
            {
                elements = value;
            }
        }

        ///
        /// The current scale at which the content is being viewed.
        /// 
        public double ContentScale
        {
            get
            {
                return contentScale;
            }
            set
            {
                contentScale = value;

                OnPropertyChanged("ContentScale");
            }
        }

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public double ContentOffsetX
        {
            get
            {
                return contentOffsetX;
            }
            set
            {
                contentOffsetX = value;

                OnPropertyChanged("ContentOffsetX");
            }
        }

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public double ContentOffsetY
        {
            get
            {
                return contentOffsetY;
            }
            set
            {
                contentOffsetY = value;

                OnPropertyChanged("ContentOffsetY");
            }
        }

        ///
        /// The width of the content (in content coordinates).
        /// 
        public double ContentWidth
        {
            get
            {
                return contentWidth;
            }
            set
            {
                contentWidth = value + (ContentCanvasMarginOffset * 2);

                OnPropertyChanged("ContentWidth");
            }
        }

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        public double ContentHeight
        {
            get
            {
                return contentHeight;
            }
            set
            {
                contentHeight = value + (ContentCanvasMarginOffset * 2);

                OnPropertyChanged("ContentHeight");
            }
        }

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        public double ContentViewportWidth
        {
            get
            {
                return contentViewportWidth;
            }
            set
            {
                contentViewportWidth = value;

                OnPropertyChanged("ContentViewportWidth");
            }
        }

        ///
        /// The heigth of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        public double ContentViewportHeight
        {
            get
            {
                return contentViewportHeight;
            }
            set
            {
                contentViewportHeight = value;

                OnPropertyChanged("ContentViewportHeight");
            }
        }

        #endregion
    }

    public interface IElement : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// GUID of the element.
        /// </summary>
        public Guid ID { get; }

        public ElementState State { get; set; }
        
        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the 'PropertyChanged' event when the value of a property of the data model has changed.
        /// Be sure to Define a 'PropertyChanged' event that is raised when the value of a property of the data model has changed.
        /// eg. <code>public new abstract event PropertyChangedEventHandler PropertyChanged;</code>
        /// </summary>
        public abstract void OnPropertyChanged(string name);

        #endregion
    }

    public interface IRenderable : IElement
    {
        public Guid ZPrev { get; }
        public Guid ZNext { get; }
        public Guid Parent { get; }
        public Guid[] Children { get; }

        /// <summary>
        /// Data View Type for the inheritor Element class.
        /// Can be a null value.
        /// Useful for defining the display properties of the element in the WPF UI Environment.
        /// </summary>
        public abstract Type View { get; }

        #region BoundingBox

        /// <summary>
        /// Bounding Box of the Element
        /// </summary>
        public abstract BoundingBox BoundingBox { get; }

        /// <summary>
        /// The X coordinate of the location of the element Bounding Box (in content coordinates).
        /// </summary>
        double X { get; }
        /// <summary>
        /// Set the X coordinate of the location of the element Bounding Box (in content coordinates).
        /// </summary>
        void SetX(double x) { BoundingBox.Location.X = x; OnPropertyChanged("X"); }

        /// <summary>
        /// The Y coordinate of the location of the element Bounding Box (in content coordinates).
        /// </summary>
        double Y { get; }
        /// <summary>
        /// Set the Y coordinate of the location of the element Bounding Box (in content coordinates).
        /// </summary>
        void SetY(double x) { BoundingBox.Location.Y = x; OnPropertyChanged("Y"); }

        /// <summary>
        /// The width of the element Bounding Box (in content coordinates).
        /// </summary>
        double Width { get; }
        /// <summary>
        /// Set the width of the element Bounding Box (in content coordinates).
        /// </summary>
        void SetWidth(double x) { BoundingBox.Size.Width = x; OnPropertyChanged("Width"); }

        /// <summary>
        /// The height of the element Bounding Box (in content coordinates).
        /// </summary>
        double Height { get; }
        /// <summary>
        /// Set the height of the element Bounding Box (in content coordinates).
        /// </summary>
        void SetHeight(double x) { BoundingBox.Size.Height = x; OnPropertyChanged("Height"); }

        #endregion
    }

    public interface IComputable : IElement
    {
        public Guid[] DataDS { get; }
        public Guid[] DataUS { get; }
        public Guid[] EventDS { get; }
        public Guid[] EventUS { get; }

        public abstract void Compute();
    }

    public class ElementCollection<T> : LinkedList<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : IElement
    {
        #region INotifyCollectionChanged and INotifyPropertyChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raises the 'CollectionChanged' event when the value of a property of the data model has changed.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }
        /// <summary>
        /// Raises the 'PropertyChanged' event when the value of a property of the data model has changed.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        /// <summary>
        /// Raises the 'PropertyChanged' event when the value of a property of the data model has changed.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        #endregion

        public ElementCollection() : base()
        {
        }

        #region Properties

        /// <summary>
        /// Gets the last node of the ElementCollection.
        /// </summary>
        public new T Last { get => base.Last.Value; }
        
        /// <summary>
        /// Gets the first node of the ElementCollection.
        /// </summary>
        public new T First { get => base.First.Value; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified new node after the specified existing node in the System.Collections.Generic.LinkedList`1.
        /// </summary>
        /// <param name="reference">The System.Collections.Generic.LinkedListNode`1 after which to insert newNode.</param>
        /// <param name="added">The new System.Collections.Generic.LinkedListNode`1 to add to the System.Collections.Generic.LinkedList`1.</param>
        public void AddAfter(Guid reference, Guid added)
        {
            if (this.Contains(reference))
            {
                base.AddAfter(this.Find(this[reference]), this.Find(this[added]));
            }
        }

        /// <summary>
        /// Adds the specified new node Before the specified existing node in the System.Collections.Generic.LinkedList`1.
        /// </summary>
        /// <param name="reference">The System.Collections.Generic.LinkedListNode`1 Before which to insert newNode.</param>
        /// <param name="added">The new System.Collections.Generic.LinkedListNode`1 to add to the System.Collections.Generic.LinkedList`1.</param>
        public void AddBefore(Guid reference, Guid added)
        {
            if (this.Contains(reference))
            {
                base.AddBefore(this.Find(this[reference]), this.Find(this[added]));
            }
        }

        /// <summary>
        /// Adds the specified new node Before the specified existing node in the System.Collections.Generic.LinkedList`1.
        /// </summary>
        /// <param name="reference">The System.Collections.Generic.LinkedListNode`1 Before which to insert newNode.</param>
        /// <param name="added">The new System.Collections.Generic.LinkedListNode`1 to add to the System.Collections.Generic.LinkedList`1.</param>
        public void AddFirst(Guid added)
        {
            base.AddFirst(this.Find(this[added]));
        }

        /// <summary>
        /// Adds the specified new node Before the specified existing node in the System.Collections.Generic.LinkedList`1.
        /// </summary>
        /// <param name="reference">The System.Collections.Generic.LinkedListNode`1 Before which to insert newNode.</param>
        /// <param name="added">The new System.Collections.Generic.LinkedListNode`1 to add to the System.Collections.Generic.LinkedList`1.</param>
        public void AddLast(Guid added)
        {
            base.AddLast(this.Find(this[added]));
        }
        
        /// <summary>
        /// Determines whether a value is in the System.Collections.Generic.LinkedList`1.
        /// </summary>
        /// <param name="id">The value to locate in the System.Collections.Generic.LinkedList`1. The value can be null for reference types.</param>
        /// <returns></returns>
        public bool Contains(Guid id)
        {
            return this.Find(this[id]) != null;
        }

        public T this[Guid id]
        {
            get
            {
                foreach (T element in this)
                {
                    if (element.ID == id)
                    {
                        return element;
                    }
                }
                return default;
            }
        }

        /// <summary>
        /// Finds the first node that contains the specified value.
        /// </summary>
        /// <param name="id">The value to locate in the System.Collections.Generic.LinkedList`1.</param>
        /// <returns>The first System.Collections.Generic.LinkedListNode`1 that contains the specified value, if found; otherwise, null.</returns>
        public IElement Find(Guid id)
        {
            return this.Find(this[id]).Value;
        }
        //
        // Summary:
        //     Removes the first occurrence of the specified value from the System.Collections.Generic.LinkedList`1.
        //
        // Parameters:
        //   value:
        //     The value to remove from the System.Collections.Generic.LinkedList`1.
        //
        // Returns:
        //     true if the element containing value is successfully removed; otherwise, false.
        //     This method also returns false if value was not found in the original System.Collections.Generic.LinkedList`1.
        public bool Remove(Guid id)
        {
            var o = this.Find(this[id]);
            if (o != null)
            {
                base.Remove(o);
                return true;
            }
            else return false;
        }

        #endregion
    }

    public enum ElementState
    {
        /// <summary>
        /// No state.
        /// </summary>
        Unset = -1,
        /// <summary>
        /// Default state.
        /// </summary>
        Default = 0
    }
}
