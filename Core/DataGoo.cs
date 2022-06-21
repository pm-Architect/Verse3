using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IDataGoo : INotifyPropertyChanged
    {
        public Guid ID { get; }
        public Guid Parent { get; }
        public Guid[] Children { get; }
        object Data { get; set; }
        bool IsValid { get; }
        string IsValidReason { get; }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the 'PropertyChanged' event when the value of a property of the data model has changed.
        /// Be sure to Define a 'PropertyChanged' event that is raised when the value of a property of the data model has changed.
        /// eg. <code>public new abstract event PropertyChangedEventHandler PropertyChanged;</code>
        /// </summary>
        public abstract void OnPropertyChanged(string name);

        #endregion

    }
    public interface IDataGoo<D> : IDataGoo
    {
        new D Data { get; set; }
        string TypeName { get; }
        IDataGoo<D> Clone();
    }

    public interface IDataGooContainer : IComputable
    {
        public ContainerState ContainerState { get; }
        public DataState DataState { get; }
        Type DataValueType { get; }
        IList<IDataGooContainer> Sources { get; }
        int AddSource(IDataGooContainer source);
        void RemoveSource(int index);
        void RemoveSource(IDataGooContainer source);
        void RemoveSource(Guid sourceID);
        void RemoveAllSources();
        void ReplaceSource(int index, IDataGooContainer newSource);
        void ReplaceSource(IDataGooContainer oldSource, IDataGooContainer newSource);
        void ReplaceSource(Guid oldSourceID, IDataGooContainer newSource);
        DataStructure DataGoos { get; set; }

        //TODO:
        //VOLATILE DATA
    }

    public class DataStructure<D> : DataStructure
    {

    }

    public class DataStructure : DataLinkedList<DataBranch>
    {
        bool IsEmpty
        {
            get
            {
                return this.Count == 0;
            }
        }
    }

    public class DataBranch<D> : DataBranch
    {
        public new D Data { get; set; }
    }

    public class DataBranch : DataLinkedList<IDataGoo>, IDataGoo
    {
        public object Data { get; set; }
        public Guid ID { get; set; }
        public bool IsValid { get; }
        public string IsValidReason { get; }
        public Guid Parent { get; }
        public Guid[] Children { get; }
        public bool IsEmpty { get; }
        public void OnPropertyChanged(string name)
        {

        }

        //TODO : Type?, if any
    }

    public class DataAddress
    {
        public IDataGoo AddressOf { get; private set; }
        public DataAddress(IDataGoo goo)
        {
            this.AddressOf = goo;
        }
        public override string ToString()
        {
            //TODO
            return base.ToString();
        }
    }

    public enum DataState
    {
        Unknown = -1,
        IsNull = 0,
        Internal = 1,
        External = 2,
        Volatile = 3
    }

    public enum DataStructureType
    {
        Item = 0,
        List = 1,
        Tree = 2,
        Object = 3
    }

    public enum ContainerState
    {
        Unknown = -1,
        InputAndOutput = 0,
        InputOnly = 1,
        OutputOnly = 2,
        VolatileOnly = 3
    }
}
