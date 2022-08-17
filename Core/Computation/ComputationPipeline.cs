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
        
        public static int Compute(IComputable sender = null)
        {
            int count = 0;
            try
            {
                count = ComputeComputable(ComputationPipeline.Instance._current);
            }
            catch /*(Exception e)*/
            {
                //TODO: Log to console
            }
            return count;
        }

        
        

        public static int ComputeComputable(IComputable computable, bool recursive = true, bool upstream = false)
        {
            if (computable.ComputableElementState == ComputableElementState.Computing) return -1;
            else computable.ComputableElementState = ComputableElementState.Computing;
            int count = 0;
            try
            {
                bool computeSuccess = true;
                if (computable != null)
                {
                    
                    ComputationPipeline.Instance._current = computable;
                    computable.CollectData();
                    computable.Compute();
                    computable.DeliverData();
                    
                    count++;
                    if (recursive)
                    {
                        if (!upstream)
                        {
                            if (computable.ComputationPipelineInfo.DataDS != null && computable.ComputationPipelineInfo.DataDS.Count > 0)
                            {
                                foreach (IComputable compDS in computable.ComputationPipelineInfo.DataDS)
                                {
                                    //TODO: Log to console
                                    computeSuccess = computeSuccess && (ComputeComputable(compDS) > 0);
                                }
                            }
                        }
                        else
                        {
                            if (computable.ComputationPipelineInfo.DataUS != null && computable.ComputationPipelineInfo.DataUS.Count > 0)
                            {
                                foreach (IComputable compUS in computable.ComputationPipelineInfo.DataUS)
                                {
                                    //TODO: Log to console
                                    computeSuccess = computeSuccess && (ComputeComputable(compUS) > 0);
                                }
                            }
                        }
                    }
                }
                if (computeSuccess) return count;
                else return -1;
            }
            catch /*(Exception e)*/
            {
                computable.ComputableElementState = ComputableElementState.Failed;
                //TODO: Log to console
            }
            finally
            {
                computable.ComputableElementState = ComputableElementState.Computed;
            }
            return count;
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

        void CollectData();

        public ComputableElementState ComputableElementState { get; set; }
        //public ElementConsole Console { get; }
        //public bool Enabled { get; set; }
        //void ClearData();
        //{
        //TODO: Populate DataDS and DataUS and Collect data from nodes
        //}
        void Compute();
        void DeliverData();
    }
}
