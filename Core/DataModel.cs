using System;
using System.ComponentModel;
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
        protected ElementsLinkedList<IElement> elements = new ElementsLinkedList<IElement>();

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

        public static double ContentCanvasMarginOffset = 0.0;

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
        public ElementsLinkedList<IElement> Elements
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

        public IElement GetElementWithGuid(Guid guid)
        {
            return this.Elements.Find(guid).Value;
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

        public CanvasPoint ContentOffset
        {
            get
            {
                return new CanvasPoint(ContentOffsetX, ContentOffsetY);
            }
            set
            {
                ContentOffsetX = value.X;
                ContentOffsetY = value.Y;
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

        public CanvasSize ContentSize
        {
            get
            {
                return new CanvasSize(ContentWidth, ContentHeight);
            }
            set
            {
                ContentWidth = value.Width;
                ContentHeight = value.Height;
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


        public CanvasSize ContentViewportSize
        {
            get
            {
                return new CanvasPoint(ContentViewportWidth, ContentViewportHeight);
            }
            set
            {
                ContentViewportWidth = value.Width;
                ContentViewportHeight = value.Height;
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

        public ElementState ElementState { get; set; }

        public ElementType ElementType { get; set; }

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
    
    #region Enums

    public enum ElementState
    {
        /// <summary>
        /// No state.
        /// </summary>
        Unset = -1,
        /// <summary>
        /// Default state.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Hidden state.
        /// </summary>
        Hidden = 1,
        /// <summary>
        /// Disabled state.
        /// </summary>
        Disabled = 2
    }
    public enum ComputableElementState
    {
        /// <summary>
        /// No state.
        /// </summary>
        Unset = -1,
        /// <summary>
        /// Default state.
        /// </summary>
        Default = 0,
        Computing = 1,
        Computed = 2,
        Failed = 3
    }
    public enum ElementType
    {
        /// <summary>
        /// No type.
        /// </summary>
        Unset = -1,
        /// <summary>
        /// Default type.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Default type.
        /// </summary>
        Connection = 1,
        /// <summary>
        /// Default type.
        /// </summary>
        Node = 2
    }
    public enum NodeType
    {
        Unset = -1,
        Default = 0,
        Input = 1,
        Output = 2
    }
    public enum ConnectionType
    {
        Unset = -1,
        Default = 0,
        Data = 1,
        Event = 2
    }

    #endregion
}
