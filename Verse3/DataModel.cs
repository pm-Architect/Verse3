using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Verse3
{
    /// <summary>
    /// A simple example of a data-model.  
    /// The purpose of this data-model is to share display data between the main window and overview window.
    /// </summary>
    public class DataModel : INotifyPropertyChanged
    {
        #region Data Members

        /// <summary>
        /// The singleton instance.
        /// This is a singleton for convenience.
        /// </summary>
        private static DataModel instance = new DataModel();

        /// <summary>
        /// The list of rectangles that is displayed both in the main window and in the overview window.
        /// </summary>
        private ObservableCollection<RectangleData> rectangles = new ObservableCollection<RectangleData>();

        ///
        /// The current scale at which the content is being viewed.
        /// 
        private double contentScale = 1;

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private double contentOffsetX = 0;

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private double contentOffsetY = 0;

        ///
        /// The width of the content (in content coordinates).
        /// 
        private double contentWidth = 0;

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        private double contentHeight = 0;

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        private double contentViewportWidth = 0;

        ///
        /// The heigth of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        private double contentViewportHeight = 0;

        #endregion Data Members

        /// <summary>
        /// Retreive the singleton instance.
        /// </summary>
        public static DataModel Instance
        {
            get
            {
                return instance;
            }
        }

        public DataModel()
        {
            //
            // Populate the data model with some example data.
            //
            rectangles.Add(new RectangleData(50, 50, 80, 150, Colors.Blue));
            rectangles.Add(new RectangleData(550, 350, 80, 150, Colors.Green));
            rectangles.Add(new RectangleData(850, 850, 30, 20, Colors.Purple));
            rectangles.Add(new RectangleData(1200, 1200, 80, 150, Colors.Red));
        }

        /// <summary>
        /// The list of rectangles that is displayed both in the main window and in the overview window.
        /// </summary>
        public ObservableCollection<RectangleData> Rectangles
        {
            get
            {
                return rectangles;
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
                contentWidth = value;

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
                contentHeight = value;

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

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the 'PropertyChanged' event when the value of a property of the data model has changed.
        /// </summary>
        private void OnPropertyChanged(string name)
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
    }

    /// <summary>
    /// Defines the data-model for a simple displayable rectangle.
    /// </summary>
    public class RectangleData : INotifyPropertyChanged
    {
        #region Data Members

        /// <summary>
        /// The X coordinate of the location of the rectangle (in content coordinates).
        /// </summary>
        private double x = 0;

        /// <summary>
        /// The Y coordinate of the location of the rectangle (in content coordinates).
        /// </summary>
        private double y = 0;

        /// <summary>
        /// The width of the rectangle (in content coordinates).
        /// </summary>
        private double width = 0;

        /// <summary>
        /// The height of the rectangle (in content coordinates).
        /// </summary>
        private double height = 0;

        /// <summary>
        /// The color of the rectangle.
        /// </summary>
        private Color color;

        /// <summary>
        /// Set to 'true' when the rectangle is selected in the ListBox.
        /// </summary>
        private bool isSelected = false;

        #endregion Data Members

        public RectangleData()
        {
        }

        public RectangleData(double x, double y, double width, double height, Color color)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.color = color;
        }

        /// <summary>
        /// The X coordinate of the location of the rectangle (in content coordinates).
        /// </summary>
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;

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
                return y;
            }
            set
            {
                y = value;

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
                return width;
            }
            set
            {
                width = value;

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
                return height;
            }
            set
            {
                height = value;

                OnPropertyChanged("Height");
            }
        }

        /// <summary>
        /// The color of the rectangle.
        /// </summary>
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;

                OnPropertyChanged("Color");
            }
        }

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

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the 'PropertyChanged' event when the value of a property of the data model has changed.
        /// </summary>
        private void OnPropertyChanged(string name)
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
    }

    /// <summary>
    /// Converts a color value to a brush.
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
