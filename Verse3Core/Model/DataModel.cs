using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse3.Core.Utilities;

namespace Verse3.Core.Model
{
    public class DataModel : Observable
    {
        public Guid Guid { get; private set; }
        public ElementDictionary Elements { get; private set; }

        public DataModel()
        {
            Guid = Guid.NewGuid();
            this.Elements = new ElementDictionary();

            //TODO: Add first component (Start and Update Events)
        }

        #region ViewModelProperties


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

        #endregion

    }

    public class ElementDictionary : ObservableCollection<ElementBase>, IDictionary<Guid, ElementBase>
    {
        //TODO
        public ElementBase this[Guid key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<Guid> Keys => throw new NotImplementedException();

        public ICollection<ElementBase> Values => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(Guid key, ElementBase value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<Guid, ElementBase> item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<Guid, ElementBase> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<Guid, ElementBase>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Guid key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<Guid, ElementBase> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(Guid key, out ElementBase value)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<Guid, ElementBase>> IEnumerable<KeyValuePair<Guid, ElementBase>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
