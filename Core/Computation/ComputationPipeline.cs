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
            //TODO: PARALLEL COMPUTATION
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
                            //if (computable.ComputationPipelineInfo.DataUS != null && computable.ComputationPipelineInfo.DataUS.Count > 0)
                            //{
                            //    foreach (IComputable compUS in computable.ComputationPipelineInfo.DataUS)
                            //    {
                            //        //TODO: Log to console
                            //        computeSuccess = computeSuccess && (ComputeComputable(compUS) > 0);
                            //    }
                            //}
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
    
    public interface IComputable : IElement
    {
        public ComputationPipelineInfo ComputationPipelineInfo { get; }

        //public ElementsLinkedList<INode> Nodes { get; }

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

    public class ComputationPipelineInfo
    {
        private IComputable _computable;
        private IOManager _ioManager;
        public IOManager IOManager => _ioManager;
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
            this._ioManager = new IOManager(computable);
        }
        public void AddDataUpStream(IComputable dataUS)
        {
            if (dataUS is null) return;
            if (!this._dataUS.Contains(dataUS))
            {
                this._dataUS.Add(dataUS);
                dataUS.ComputationPipelineInfo.AddDataDownStream(_computable);
            }
        }
        public void AddDataDownStream(IComputable dataDS)
        {
            if (dataDS is null) return;
            if (!this._dataDS.Contains(dataDS))
            {
                this._dataDS.Add(dataDS);
                dataDS.ComputationPipelineInfo.AddDataUpStream(_computable);
            }
        }
        public void AddEventUpStream(IComputable eventUS)
        {
            if (eventUS is null) return;
            if (!this._eventUS.Contains(eventUS))
            {
                this._eventUS.Add(eventUS);
                eventUS.ComputationPipelineInfo.AddEventDownStream(_computable);
            }
        }
        public void AddEventDownStream(IComputable eventDS)
        {
            if (eventDS is null) return;
            if (!this._eventDS.Contains(eventDS))
            {
                this._eventDS.Add(eventDS);
                eventDS.ComputationPipelineInfo.AddEventUpStream(_computable);
            }
        }
        public void CollectData()
        {
            if (this._dataUS.Count > 0)
            {
                foreach (IComputable dataUS in this._dataUS)
                {
                    if (dataUS.ComputableElementState == ComputableElementState.Computed)
                    {
                        IOManager.CollectData();
                    }
                    //else ComputationPipeline.ComputeComputable(dataUS);
                }
            }
        }
        public void DeliverData()
        {
            if (this._computable.ComputableElementState == ComputableElementState.Computed)
            {
                foreach (IComputable dataDS in this._dataDS)
                {
                    if (dataDS.ComputableElementState == ComputableElementState.Computed)
                    {
                        IOManager.DeliverData();
                    }
                }
            }
            //else ComputationPipeline.ComputeComputable(this._computable);
        }
    }

    public class IOManager
    {
        private IComputable _computable;
        private ElementsLinkedList<IDataNode> _dataInputNodes = new ElementsLinkedList<IDataNode>();
        public ElementsLinkedList<IDataNode> DataInputNodes => _dataInputNodes;

        private ElementsLinkedList<IDataNode> _dataOutputNodes = new ElementsLinkedList<IDataNode>();
        public ElementsLinkedList<IDataNode> DataOutputNodes => _dataOutputNodes;
        private ElementsLinkedList<IEventNode> _eventInputNodes = new ElementsLinkedList<IEventNode>();
        public ElementsLinkedList<IEventNode> EventInputNodes => _eventInputNodes;
        private ElementsLinkedList<IEventNode> _eventOutputNodes = new ElementsLinkedList<IEventNode>();
        public ElementsLinkedList<IEventNode> EventOutputNodes => _eventOutputNodes;
        
        //public int ConnectionCount
        //{
        //    get
        //    {
        //        int count = 0;
        //        try
        //        {
        //            foreach (IDataNode<object> dataInputNode in _dataInputNodes)
        //            {
        //                if (dataInputNode.Connections.Count > 0) count += dataInputNode.Connections.Count;
        //            }
        //            foreach (IDataNode<object> dataOutputNode in _dataOutputNodes)
        //            {
        //                if (dataOutputNode.Connections.Count > 0) count += dataOutputNode.Connections.Count;
        //            }
        //        }
        //        catch
        //        { 
        //            //TODO: LOG to console
        //        }
        //        return count;
        //    }
        //}
        
        public IOManager(IComputable computable)
        {
            this._computable = computable;
            //this._dataInputNodes = computable.ComputationPipelineInfo.DataUS;
            //this._dataOutputNodes = computable.ComputationPipelineInfo.DataDS;
            //this._eventInputNodes = computable.ComputationPipelineInfo.EventUS;
            //this._eventOutputNodes = computable.ComputationPipelineInfo.EventDS;
        }

        public void AddDataInputNode<T>(IDataNode<T> dataInputNode)
        {
            if (dataInputNode is null) return;
            if (!this._dataInputNodes.Contains(dataInputNode))
            {
                if (dataInputNode.NodeType == NodeType.Input)
                {
                    dataInputNode.Parent = _computable;
                    this._dataInputNodes.Add(dataInputNode);
                    //if (dataInputNode is IComputable)
                    //{
                    //    this._computable.ComputationPipelineInfo.AddDataUpStream(dataInputNode as IComputable);
                    //}
                }
            }
        }
        public void AddDataOutputNode<T>(IDataNode<T> dataOutputNode)
        {
            if (dataOutputNode is null) return;
            if (!this._dataOutputNodes.Contains(dataOutputNode))
            {
                if (dataOutputNode.NodeType == NodeType.Output)
                {
                    dataOutputNode.Parent = _computable;
                    this._dataOutputNodes.Add(dataOutputNode);
                    //if (dataOutputNode is IComputable)
                    //{
                    //    this._computable.ComputationPipelineInfo.AddDataDownStream(dataOutputNode as IComputable);
                    //}
                }
            }
        }
        public void AddEventInputNode(IEventNode eventInputNode)
        {
            if (eventInputNode is null) return;
            if (!this._eventInputNodes.Contains(eventInputNode))
            {
                if (eventInputNode.NodeType == NodeType.Input)
                {
                    //eventInputNode.Parent = _computable;
                    this._eventInputNodes.Add(eventInputNode);
                    if (eventInputNode is IComputable)
                    {
                        this._computable.ComputationPipelineInfo.AddEventUpStream(eventInputNode as IComputable);
                    }
                }
            }
        }
        public void AddEventOutputNode(IEventNode eventOutputNode)
        {
            if (eventOutputNode is null) return;
            if (!this._eventOutputNodes.Contains(eventOutputNode))
            {
                if (eventOutputNode.NodeType == NodeType.Output)
                {
                    //eventOutputNode.Parent = _computable;
                    this._eventOutputNodes.Add(eventOutputNode);
                    if (eventOutputNode is IComputable)
                    {
                        this._computable.ComputationPipelineInfo.AddEventDownStream(eventOutputNode as IComputable);
                    }
                }
            }
        }
        
        public object GetData(int index)
        {
            return this._dataOutputNodes[index].DataGoo.Data;
        }
        public Type GetData(out object output, int index)
        {
            output = this._dataOutputNodes[index].DataGoo.Data;
            return this._dataOutputNodes[index].DataGoo.Data.GetType();
        }
        public T GetData<T>(int index)
        {
            if (this._dataOutputNodes[index].DataGoo.DataType == typeof(T))
                return (T)this._dataOutputNodes[index].DataGoo.Data;
            else return default;
        }
        public bool GetData<T>(out T output, int index)
        {
            if (this._dataOutputNodes[index].DataGoo.DataType == typeof(T))
            {
                output = (T)this._dataOutputNodes[index].DataGoo.Data;
                return true;
            }
            else
            {
                output = default;
                return false;
            }
        }
        
        public bool SetData(object data, int index)
        {
            if (this._dataOutputNodes[index].DataGoo.DataType == data.GetType())
            {
                this._dataOutputNodes[index].DataGoo.Data = data;
                return true;
            }
            else return false;
        }
        public bool SetData<T>(T data, int index)
        {
            if (this._dataOutputNodes[index].DataGoo.DataType == typeof(T))
            {
                this._dataOutputNodes[index].DataGoo.Data = data;
                return true;
            }
            else return false;
        }

        public void CollectData()
        {
            foreach (IDataNode<object> dataInputNode in this._dataInputNodes)
            {
                dataInputNode.CollectData();
            }
        }
        public void DeliverData()
        {
            foreach (IDataNode<object> dataOutputNode in this._dataOutputNodes)
            {
                dataOutputNode.DeliverData();
            }
        }
    }
}
