using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Core
{
    public interface IDataGoo : INotifyPropertyChanged, ISerializable
    {
        public Guid ID { get; }
        public IDataGoo Parent { get; }
        public DataStructure Children { get; }
        [DataMember]
        object Data { get; set; }
        bool IsValid { get; }
        string IsValidReason { get; }
        public DataAddress Address { get; }
        //[DataMember]
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
        //public ContainerState ContainerState { get; }
        //public DataState DataState { get; }
        Type DataValueType { get; }
        //IList<IDataGooContainer> Sources { get; }
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
    public interface IDataGooContainer<D> : IDataGooContainer
    {
        //public ContainerState ContainerState { get; }
        //public DataState DataState { get; }
        //Type DataValueType { get; }
        //ElementsLinkedList<IDataGooContainer> Sources { get; }
        //int AddSource(IDataGooContainer source);
        //void RemoveSource(int index);
        //void RemoveSource(IDataGooContainer source);
        //void RemoveSource(Guid sourceID);
        //void RemoveAllSources();
        //void ReplaceSource(int index, IDataGooContainer newSource);
        //void ReplaceSource(IDataGooContainer oldSource, IDataGooContainer newSource);
        //void ReplaceSource(Guid oldSourceID, IDataGooContainer newSource);
        new DataStructure<D> DataGoo { get; set; }
    }

    [Serializable]
    public class DataStructure<D> : DataStructure, ISerializable
    {
        protected DataStructure(SerializationInfo info, StreamingContext context) : base()
        {
            if (info.ObjectType == typeof(DataStructure<D>))
            {
                //if (DataType == (Type)info.GetValue("DataType", typeof(Type)))
                //{
                Data = (D)info.GetValue("Data", typeof(D));
                //DataType = typeof(D);
                //}
            }
            //info.GetValue("DataType", typeof(Type));
            //info.GetValue("Data", this.DataType);
        }
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("Data", Data);
                //info.AddValue("DataType", DataType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Fire an event when Data is set
        public delegate void DataChangedEventHandler(DataStructure<D> sender, DataChangedEventArgs<D> e);
        public DataChangedEventHandler DataChanged;
        public new D Data
        {
            get
            {
                if (base.volatileData == null)
                    return default(D);
                if (base.volatileData is D)
                    return (D)base.volatileData;
                else
                    throw new Exception("Data is not of type " + typeof(D).Name);
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                D old = default;
                if (base.volatileData != null && base.volatileData is D) old = (D)base.volatileData;
                base.volatileData = value;
                if (DataChanged != null && DataChanged.GetInvocationList().Length > 0)
                    DataChanged(this, new DataChangedEventArgs<D>(old, value));
            }
        }
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

    [Serializable]
    [DataContract]
    public class DataStructure : DataLinkedList<IDataGoo>, IDataGoo, ISerializable
    {
        //[DataMember]
        public byte[] Bytes
        {
            get => ToBytes(this);
        }
        public DataStructure(SerializationInfo info, StreamingContext context)
        {
            if (info.ObjectType == typeof(DataStructure))
            {
                try
                {
                    //byte[] bytes = { };
                    //bytes = (byte[])info.GetValue("Bytes", bytes.GetType());
                    //DataStructure temp = DataSerialization.DeserializeFromBytes(bytes);
                    //this.volatileData = temp.volatileData;
                    //this.Children = temp.Children;
                    //this.Parent = temp.Parent;
                    //this.Address = temp.Address;
                    //this.DataType = temp.DataType;
                    //this.ID = temp.ID;
                    //this.Data = temp.Data;
                    //Guid checkGuid = (Guid)info.GetValue("ID", typeof(Guid));
                    //if (checkGuid != ID)
                        //throw new Exception("DataStructure ID does not match");
                    object checkData = info.GetValue("Data", DataType);
                    this.Data = checkData;
                    if (checkData.GetType() != DataType)
                        throw new Exception("DataStructure DataType does not match Data");
                    if (checkData != Data)
                        throw new Exception("DataStructure Data does not match");
                    //TODO: Duplicate DataStructure
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    //throw ex;
                }
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Data", Data);
            //info.AddValue("ID", ID);
            //info.AddValue("Bytes", Bytes);
            //info.AddValue("DataType", DataType);
            //if (this.Data != null)
            //{

            //}
        }

        protected object volatileData = default;
        [DataMember]
        public object Data { get => volatileData; set => volatileData = value; }
        public Guid ID { get; set; }
        public bool IsValid { get => (this.Data != default); }
        public string IsValidReason { get; }
        public IDataGoo Parent { get; }
        public DataStructure Children { get; }
        public bool IsEmpty { get; }
        public DataAddress Address { get; }
        private Type overrideDataType = default;
        public Type DataType
        {
            get
            {
                if (overrideDataType != default)
                    return overrideDataType;
                if (volatileData != default)
                    return volatileData.GetType();
                else return typeof(object);
            }
            set
            {
                if (overrideDataType == default)
                    overrideDataType = value;
            }
        }

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

        public override string ToString()
        {
            return this.Data?.ToString();
        }


        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        #endregion

        /// <summary>
        /// Called during serialization.
        /// Converts an object that inherits from GeometryBase
        /// to an array of bytes.
        /// </summary>
        public static byte[] ToBytes(DataStructure src)
        {
            var rc = new byte[0];

            if (src == null || src.Data == null)
                return rc;

            try
            {
                var formatter = new BinaryFormatter { Binder = new InteropSerializationBinder<DataStructure>() };
                using (var stream = new MemoryStream())
                {
                    formatter.Serialize(stream, src);
                    rc = stream.ToArray();
                }
            }
            catch (Exception e)
            {
                //Debug.WriteLine(e.Message);
            }

            return rc;
        }

        /// <summary>
        /// Called during serialization.
        /// Converts an object that inherits from GeometryBase
        /// to an array of bytes.
        /// </summary>
        public static byte[] ToBytes<D>(D src) where D : DataStructure
        {
            var rc = new byte[0];

            if (src == null || src.Data == null)
                return rc;

            try
            {
                var formatter = new BinaryFormatter { Binder = new InteropSerializationBinder<D>() };
                using (var stream = new MemoryStream())
                {
                    formatter.Serialize(stream, src);
                    rc = stream.ToArray();
                }
            }
            catch (Exception e)
            {
                //Debug.WriteLine(e.Message);
            }

            return rc;
        }

        /// <summary>
        /// Called during de-serialization.
        /// Creates an object that inherits from GeometryBase
        /// from an array of bytes.
        /// </summary>
        /// <param name="bytes">The array of bytes.</param>
        /// <returns>The geometry if successful.</returns>
        public static DataStructure FromBytes(byte[] bytes)
        {
            if (null == bytes || 0 == bytes.Length)
                return null;

            DataStructure rc = null;
            try
            {
                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter { Binder = new InteropSerializationBinder<DataStructure>() };
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    object data = formatter.Deserialize(stream);
                    if (data != null && data is DataStructure)
                    {
                        //TODO: Check for data validity
                        rc = data as DataStructure;
                    }
                }
            }
            catch (Exception e)
            {
                //Debug.WriteLine(e.Message);
            }

            return rc;
        }

        /// <summary>
        /// Called during de-serialization.
        /// Creates an object that inherits from GeometryBase
        /// from an array of bytes.
        /// </summary>
        /// <param name="bytes">The array of bytes.</param>
        /// <returns>The geometry if successful.</returns>
        public static D FromBytes<D>(byte[] bytes) where D : DataStructure
        {
            if (null == bytes || 0 == bytes.Length)
                return null;

            D rc = null;
            try
            {
                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter { Binder = new InteropSerializationBinder<D>() };
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    object data = formatter.Deserialize(stream);
                    if (data != null && data is D)
                    {
                        //TODO: Check for data validity
                        rc = data as D;
                    }
                }
            }
            catch (Exception e)
            {
                //Debug.WriteLine(e.Message);
            }

            return rc;
        }

        /// <summary>
        /// RockfishDeserializationBinder class
        /// </summary>
        /// <remarks>
        /// Both RhinoCommon and Rhino3dmIO have a Rhino.Geometry.GeometryBase
        /// class. This serialization binder help deserialize the equivalent 
        /// objects across the different assemblies.
        /// </remarks>
        internal class InteropSerializationBinder<D> : SerializationBinder where D : DataStructure
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                var assembly = typeof(D).Assembly;
                if (typeof(D).ContainsGenericParameters)
                {
                    //foreach (Type t in typeof(D).GetGenericParameterConstraints())
                    //{
                    //}
                }
                assemblyName = assembly.ToString();
                var type_to_deserialize = Type.GetType($"{typeName}, {assemblyName}");
                return type_to_deserialize;
            }
        }
    }

    public class DataChangedEventArgs<D> : DataChangedEventArgs
    {
        public new D OldData { get; private set; }
        public new D NewData { get; private set; }
        public DataChangedEventArgs(D oldData, D newData) : base(oldData, newData)
        {
        }
    }
    public class DataChangedEventArgs : EventArgs
    {
        public object OldData { get; private set; }
        public object NewData { get; private set; }
        public DataChangedEventArgs(object oldData, object newData)
        {
            OldData = oldData;
            NewData = newData;
        }
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
