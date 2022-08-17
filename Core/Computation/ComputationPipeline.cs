using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ComputationPipeline
    {
        private static ComputationPipeline instance = new ComputationPipeline();
        public static ComputationPipeline Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ComputationPipeline();
                }
                return ComputationPipeline.instance;
            }
            protected set
            {
                instance = value;
            }
        }
        internal IComputable _current;
        public IComputable Current => _current;
        private ComputationPipeline()
        {
            this._current = default;
        }
        
        public static int Compute()
        {
            int count = 0;
            try
            {
                //Parallel.ForEach(DataModel.Instance.Elements, e => { });
                foreach (IElement e in DataModel.Instance.Elements)
                {
                    if (e != null && e is IComputable)
                    {
                        IComputable computable = e as IComputable;
                        //if (RenderPipeline.Instance._current != default)
                        //{
                        //    RenderPipeline.Instance._current.ZNext = renderable;
                        //}
                        ComputationPipeline.Instance._current = computable;
                        if (ComputeComputable(computable))
                        {
                            count++;
                        }
                        //renderable.Render();
                        //count++;
                        //if (renderable.Children.Count > 0)
                        //{
                        //    foreach (IRenderable child in renderable.Children)
                        //    {
                        //        child.Render();
                        //        count++;
                        //    }
                        //}
                    }
                }
            }
            catch /*(Exception e)*/
            {
                //TODO: Log to console
            }
            return count;
        }

        //TODO: Output int
        public static bool ComputeComputable(IComputable computable, bool recursive = true)
        {
            bool computeSuccess = true;
            if (computable != null)
            {
                computable.Compute();
                if (recursive)
                {
                    if (computable.ComputationPipelineInfo.DataDS != null && computable.ComputationPipelineInfo.DataDS.Count > 0)
                    {
                        foreach (IComputable compDS in computable.ComputationPipelineInfo.DataDS)
                        {
                            computeSuccess = computeSuccess && ComputeComputable(compDS);
                        }
                    }
                }
            }
            return computeSuccess;
        }
    }
    
    public class ComputationPipelineInfo
    {
        private IComputable _computable;
        private ElementsLinkedList<IComputable> _dataDS = new ElementsLinkedList<IComputable>();
        public ElementsLinkedList<IComputable> DataDS => _dataDS;

        private ElementsLinkedList<IComputable> _dataUS = new ElementsLinkedList<IComputable>();
        public ElementsLinkedList<IComputable> DataUS => _dataUS;

        private ElementsLinkedList<IComputable> _eventDS = new ElementsLinkedList<IComputable>();
        public ElementsLinkedList<IComputable> EventDS => _eventDS;

        private ElementsLinkedList<IComputable> _eventUS = new ElementsLinkedList<IComputable>();
        public ElementsLinkedList<IComputable> EventUS => _eventUS;
        public ComputationPipelineInfo(IComputable computable)
        {
            this._computable = computable;
        }
        public void AddDataUpStream(IComputable dataUS)
        {
            if (!this._dataUS.Contains(dataUS))
            {
                this._dataUS.Add(dataUS);
                dataUS.ComputationPipelineInfo.AddDataDownStream(_computable);
            }
        }
        public void AddDataDownStream(IComputable dataDS)
        {
            if (!this._dataDS.Contains(dataDS))
            {
                this._dataDS.Add(dataDS);
                dataDS.ComputationPipelineInfo.AddDataUpStream(_computable);
            }
        }
        public void AddEventUpStream(IComputable eventUS)
        {
            if (!this._eventUS.Contains(eventUS))
            {
                this._eventUS.Add(eventUS);
                eventUS.ComputationPipelineInfo.AddEventDownStream(_computable);
            }
        }
        public void AddEventDownStream(IComputable eventDS)
        {
            if (!this._eventDS.Contains(eventDS))
            {
                this._eventDS.Add(eventDS);
                eventDS.ComputationPipelineInfo.AddEventUpStream(_computable);
            }
        }
    }

    public interface IComputable : IElement
    {
        public ComputationPipelineInfo ComputationPipelineInfo { get; }

        public ElementsLinkedList<INode> Nodes { get; }
        //public ComputableElementState ComputableElementState { get; set; }
        //public ElementConsole Console { get; }
        //public bool Enabled { get; set; }
        //void ClearData();
        //void CollectData()
        //{
            //TODO: Populate DataDS and DataUS and Collect data from nodes
        //}
        void Compute();
    }
}
