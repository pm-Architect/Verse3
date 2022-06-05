using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse3.Core.Types;
using Verse3.Core.Utilities;

namespace Verse3.Core.Model
{
    public class ElementBase : Observable
    {
        #region RenderData

        private BoundingBox boundingBox = new BoundingBox();
        public BoundingBox BoundingBox
        {
            get
            {
                if (boundingBox.IsValid()) return boundingBox;
                else return new BoundingBox();
            }
            set
            {
                if (value.IsValid()) boundingBox = value;
                else boundingBox = new BoundingBox();
                OnPropertyChanged("BoundingBox");
            }
        }

        #endregion

        #region LinkedRenderData

        private LinkedRenderData renderData = new LinkedRenderData();
        public LinkedRenderData RenderData
        {
            get { return renderData; }
            private set 
            { 
                renderData = value;
                OnPropertyChanged("RenderData");
            }
        }

        #endregion

        #region LinkedComputeData

        //TODO

        //DataDS

        //DataUS

        //EventDS

        //EventUS

        #endregion
    }

    public struct LinkedRenderData
    {
        public Guid ZPrev { get; set; }
        public Guid ZNext { get; set; }
        public Guid Parent { get; set; }
        public List<Guid> Children { get; set; }
    }
}
