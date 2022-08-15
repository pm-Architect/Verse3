using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Core
{
    public interface IDataGoo : INotifyPropertyChanged
    {
        public Guid ID { get; }
        public IDataGoo Parent { get; }
        public DataStructure Children { get; }
        object Data { get; set; }
        bool IsValid { get; }
        string IsValidReason { get; }
        public DataAddress Address { get; }
        public Type DataType { get; }

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
        //int AddSource(IDataGooContainer source);
        //void RemoveSource(int index);
        //void RemoveSource(IDataGooContainer source);
        //void RemoveSource(Guid sourceID);
        //void RemoveAllSources();
        //void ReplaceSource(int index, IDataGooContainer newSource);
        //void ReplaceSource(IDataGooContainer oldSource, IDataGooContainer newSource);
        //void ReplaceSource(Guid oldSourceID, IDataGooContainer newSource);
        DataStructure DataGoo { get; set; }
    }

    public class DataStructure<D> : DataStructure
    {
        public new D Data { get; set; }
        public new Type DataType => typeof(D);

        public DataStructure() : base()
        {
        }
        public DataStructure(D data) : base()
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            Data = data;
        }
        public DataStructure(IDataGoo<D> data) : base(new List<IDataGoo<D>> { data })
        {
            ID = Guid.NewGuid();
        }
        public DataStructure(IEnumerable<IDataGoo<D>> data) : base(data)
        {
            ID = Guid.NewGuid();
        }
    }

    public class DataStructure : DataLinkedList<IDataGoo>, IDataGoo
    {
        public object Data { get; set; }
        public Guid ID { get; set; }
        public bool IsValid { get; }
        public string IsValidReason { get; }
        public IDataGoo Parent { get; }
        public DataStructure Children { get; }
        public bool IsEmpty { get; }
        public DataAddress Address { get; }
        public Type DataType { get; }


        public DataStructure() : base()
        {
            ID = Guid.NewGuid();
        }
        public DataStructure(object data) : base()
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            Data = data;
            ID = Guid.NewGuid();
        }
        public DataStructure(IDataGoo data) : base(new List<IDataGoo> { data })
        {
            ID = Guid.NewGuid();
        }
        public DataStructure(IEnumerable<IDataGoo> data) : base(data)
        {
            ID = Guid.NewGuid();
        }


        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        #endregion
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
            //TODO: IMPLEMENT DATA ADDRESS
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
