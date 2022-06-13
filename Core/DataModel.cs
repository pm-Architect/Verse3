﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class DataModel : Observable
    {
        #region Data Members

        /// <summary>
        /// The singleton instance.
        /// This is a singleton for convenience.
        /// </summary>
        protected static DataModel instance = new DataModel();

        /// <summary>
        /// The list of rectangles that is displayed both in the main window and in the overview window.
        /// </summary>
        protected ObservableCollection<ElementData> elements = new ObservableCollection<ElementData>();

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

        /// <summary>
        /// Retreive the singleton instance.
        /// </summary>
        public static DataModel Instance
        {
            get
            {
                return DataModel.instance;
            }
            protected set
            {
                instance = value;
            }
        }

        public DataModel() : base()
        {
            //
            // Initialize the data model.
            //
            DataModel.Instance = this;
            DataModel.Instance.Elements.Add(new ElementData(50, 50, 80, 150));
            DataModel.Instance.Elements.Add(new ElementData(550, 350, 80, 150));
            DataModel.Instance.Elements.Add(new ElementData(850, 850, 30, 20));
            DataModel.Instance.Elements.Add(new ElementData(1200, 1200, 80, 150));
        }

        public static double ContentCanvasMarginOffset = 200.0;

        /// <summary>
        /// The list of rectangles that is displayed both in the main window and in the overview window.
        /// </summary>
        public ObservableCollection<ElementData> Elements
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
    }

    /// <summary>
    /// Defines the data-model for a simple displayable rectangle.
    /// </summary>
    public class ElementData : Observable
    {
        #region Data Members

        /// <summary>
        /// The Bounding Box of the Element (in content coordinates).
        /// </summary>
        private BoundingBox boundingBox = BoundingBox.Unset;

        /// <summary>
        /// The color of the rectangle.
        /// </summary>
        //private Color color;

        /// <summary>
        /// Set to 'true' when the rectangle is selected in the ListBox.
        /// </summary>
        private bool isSelected = false;

        #endregion Data Members

        public ElementData()
        {
        }

        //public RectangleData(double x, double y, double width, double height, Color color)
        public ElementData(double x, double y, double width, double height)

        {
            this.boundingBox = new BoundingBox(x, y, width, height);
            //this.color = color;
        }

        /// <summary>
        /// The Bounding Box of the Element (in content coordinates).
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                return boundingBox;
            }
            set
            {
                boundingBox = value;

                OnPropertyChanged("BoundingBox");
            }
        }

        /// <summary>
        /// The X coordinate of the location of the rectangle (in content coordinates).
        /// </summary>
        public double X
        {
            get
            {
                return boundingBox.Location.X;
            }
            set
            {
                boundingBox.Location.X = value;

                OnPropertyChanged("X");
            }
        }

        /// <summary>
        /// The Y coordinate of the location of the rectangle (in content coordinates).
        /// </summary>
        public double Y
        {
            get
            {
                return boundingBox.Location.Y;
            }
            set
            {
                boundingBox.Location.Y = value;
                
                OnPropertyChanged("Y");
            }
        }

        /// <summary>
        /// The width of the rectangle (in content coordinates).
        /// </summary>
        public double Width
        {
            get
            {
                return boundingBox.Size.Width;
            }
            set
            {
                boundingBox.Size.Width = value;

                OnPropertyChanged("Width");
            }
        }

        /// <summary>
        /// The height of the rectangle (in content coordinates).
        /// </summary>
        public double Height
        {
            get
            {
                return boundingBox.Size.Height;
            }
            set
            {
                boundingBox.Size.Height = value;

                OnPropertyChanged("Height");
            }
        }

        ///// <summary>
        ///// The color of the rectangle.
        ///// </summary>
        //public Color Color
        //{
        //    get
        //    {
        //        return color;
        //    }
        //    set
        //    {
        //        color = value;

        //        OnPropertyChanged("Color");
        //    }
        //}

        /// <summary>
        /// Set to 'true' when the rectangle is selected in the ListBox.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;

                OnPropertyChanged("IsSelected");
            }
        }
    }

}
