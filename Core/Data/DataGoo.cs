using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Core
{
    public interface IDataGoo : INotifyPropertyChanged/*, ISerializable*/
    {
        public Guid ID { get; }
        //public IDataGoo Parent { get; }
        //public DataStructure Children { get; }
        //[DataMember]
        object Data { get; set; }
        bool IsValid { get; }
        //string IsValidReason { get; }
        //public DataAddress Address { get; }
        ////[DataMember]
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

    //[Serializable]
    public class DataStructure<D> : DataStructure/*, ISerializable*/
    {
        public new object[] ToArray()
        {
            if (this.Count > 0)
            {
                object[] objectsOut = new object[this.Count];
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i] is DataStructure<D> d)
                    {
                        if (d.Data is D castData) objectsOut[i] = castData;
                        else if (d.Data is D[] castDataArray) objectsOut[i] = castDataArray;
                    }
                }
                return objectsOut;
            }
            else
            {
                return null;
            }
        }
        //protected DataStructure(SerializationInfo info, StreamingContext context) : base()
        //{
        //    if (info.ObjectType == typeof(DataStructure<D>))
        //    {
        //        //if (DataType == (Type)info.GetValue("DataType", typeof(Type)))
        //        //{
        //        Data = (D)info.GetValue("Data", typeof(D));
        //        //DataType = typeof(D);
        //        //}
        //    }
        //    //info.GetValue("DataType", typeof(Type));
        //    //info.GetValue("Data", this.DataType);
        //}
        //public new void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    try
        //    {
        //        info.AddValue("Data", Data);
        //        //info.AddValue("DataType", DataType);
        //    }
        //    catch (Exception ex)
        //    {
        //        CoreConsole.Log(ex);
        //    }
        //}

        //Fire an event when Data is set
        //public new delegate void DataChangedEventHandler(DataStructure<D> sender, DataChangedEventArgs<D> e);
        //public new DataChangedEventHandler DataChanged;
        public new object Data
        {
            get
            {
                try
                {
                    if (base.Count == 1)
                    {
                        if (base._metadata != null)
                        {
                            if (base[0] is D data)
                            {
                                return data;
                            }
                            if (base[0] is DataStructure<D> ds)
                            {
                                return ds.Data;
                            }
                            else throw new Exception("Cast Error: DataStructure<D> item could not be cast to D");
                        }
                        else throw new Exception("Inconsistent Data Format. Volatile Data is not DSMetadata, but Data Structure has children.");
                    }
                    else if (base.Count > 0)
                    {
                        if (base._metadata != null)
                        {
                            if (this.ToArray() is D[] array)
                            {
                                return array;
                            }
                            else throw new Exception("Cast Error: DataStructure<D> data could not be cast to D[]");
                        }
                        else throw new Exception("Inconsistent Data Format. Volatile Data is not DSMetadata, but Data Structure has children.");
                    }
                    else
                    {
                        return base._data;
                    }
                }
                catch (Exception ex)
                {
                    CoreConsole.Log(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    if (value == null) throw new ArgumentNullException("value");
                    this.Clear();
                    this.Add(value);
                }
                catch (Exception ex)
                {
                    CoreConsole.Log(ex);
                }
            }
        }
        public new Type DataType => typeof(D);

        public DataStructurePattern DataStructurePattern
        {
            get
            {
                if (_metadata != null)
                {
                    return _metadata.DataStructurePattern;
                }
                else if (_data != null)
                {
                    return DataStructurePattern.Item;
                }
                else return DataStructurePattern.Unknown;
            }
        }

        public DataStructure() : base()
        {
        }
        public DataStructure(D data) : base(data)
        {
        }
        public DataStructure(D[] data) : base()
        {
            try
            {
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException(nameof(data));
                foreach (D d in data)
                {
                    this.Add(new DataStructure<D>(d));
                }
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
            }
        }
        public DataStructure(IDataGoo<D> data) : base(new List<IDataGoo<D>> { data })
        {
        }
        public DataStructure(IEnumerable<IDataGoo<D>> data) : base(data)
        {
        }

        public new void Add(object data)
        {
            try
            {
                if (data is null) throw new ArgumentNullException(nameof(data));
                if (data is D castData)
                {
                    base.Add(new DataStructure<D>(castData));
                    //if (DataChanged != null && DataChanged.GetInvocationList().Length > 0)
                    //    DataChanged(this, new DataChangedEventArgs<D>(castData));
                }
                else if (data is DataStructure ds)
                {
                    if (this.DataType.IsAssignableFrom(ds.DataType))
                    {
                        base.Add(ds);
                        //if (DataChanged != null && DataChanged.GetInvocationList().Length > 0)
                        //    DataChanged(this, new DataChangedEventArgs<D>((D)ds.Data));
                    }
                }
                else if (data.GetType().IsArray)
                {
                    Type arrayType = data.GetType().GetElementType();
                    if (arrayType == typeof(D))
                    {
                        foreach (D d in (D[])data)
                        {
                            base.Add(new DataStructure<D>(d));
                            //if (DataChanged != null && DataChanged.GetInvocationList().Length > 0)
                            //    DataChanged(this, new DataChangedEventArgs<D>(d));
                        }
                    }
                    else
                    {
                        //TODO: TryCast
                        throw new ArgumentException("Data type mismatch");
                    }
                }
                else
                {
                    //TODO: TryCast
                    throw new ArgumentException("Data is not of type " + typeof(D).ToString());
                }
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
            }
        }
        
        public static implicit operator D(DataStructure<D> v)
        {
            try
            {
                if (v.DataStructurePattern == DataStructurePattern.Item && v.Data is D data) return data;
                else throw new Exception("DataStructure<D> is not a single item.");
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
                return default;
            }
}
        public static implicit operator DataStructure<D>(D v)
        {
            return new DataStructure<D>(v);
        }

        public static implicit operator D[](DataStructure<D> v)
        {
            try
            {
                if (v.DataStructurePattern == DataStructurePattern.List && v.Data is D[] data) return data;
                else throw new Exception("DataStructure<D> is not a list.");
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
                return null;
            }
        }
        public static implicit operator DataStructure<D>(D[] v)
        {
            try
            {
                return new DataStructure<D>(v);
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
                return null;
            }
        }
    }

    //[Serializable]
    ////[DataContract]
    public class DataStructure : DataLinkedList<IDataGoo>, IDataGoo/*, ISerializable*/
    {
        public object[] ToArray()
        {
            if (this.Count > 0)
            {
                object[] objectsOut = new object[this.Count];
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i] is DataStructure d)
                    {
                        objectsOut[i] = d.Data;
                    }
                }
                return objectsOut;
            }
            else
            {
                return null;
            }
        }
        ////[DataMember]
        public byte[] Bytes
        {
            get => ToBytes(this);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
//            if (info.ObjectType == typeof(DataStructure))
//            {
//                try
//                {
//                    //byte[] bytes = { };
//                    //bytes = (byte[])info.GetValue("Bytes", bytes.GetType());
//                    //DataStructure temp = DataSerialization.DeserializeFromBytes(bytes);
//                    //this.volatileData = temp.volatileData;
//                    //this.Children = temp.Children;
//                    //this.Parent = temp.Parent;
//                    //this.Address = temp.Address;
//                    //this.DataType = temp.DataType;
//                    //this.ID = temp.ID;
//                    //this.Data = temp.Data;
//                    //Guid checkGuid = (Guid)info.GetValue("ID", typeof(Guid));
//                    //if (checkGuid != ID)
//                        //throw new Exception("DataStructure ID does not match");
//                    object checkData = info.GetValue("Data", DataType);
//                    this.Data = checkData;
//                    if (checkData is null)
//                        throw new Exception("DataStructure Data is Null");
//                    if (checkData.GetType() != DataType)
//                        throw new Exception("DataStructure DataType does not match Data");
//                    if (checkData != Data)
//                        throw new Exception("DataStructure Data does not match");
//                    //TODO: Duplicate DataStructure
//                }
//                catch (Exception ex)
//                {
//                    CoreConsole.Log(ex);
//                }
//            }
            base.GetObjectData(info, context);
            if (_data != default) info.AddValue("InternalData", _data);
        }

        public DataStructure(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        //public DataStructure(SerializationInfo info, StreamingContext context)
        //{
        //    if (info.ObjectType == typeof(DataStructure))
        //    {
        //        try
        //        {
        //            //byte[] bytes = { };
        //            //bytes = (byte[])info.GetValue("Bytes", bytes.GetType());
        //            //DataStructure temp = DataSerialization.DeserializeFromBytes(bytes);
        //            //this.volatileData = temp.volatileData;
        //            //this.Children = temp.Children;
        //            //this.Parent = temp.Parent;
        //            //this.Address = temp.Address;
        //            //this.DataType = temp.DataType;
        //            //this.ID = temp.ID;
        //            //this.Data = temp.Data;
        //            //Guid checkGuid = (Guid)info.GetValue("ID", typeof(Guid));
        //            //if (checkGuid != ID)
        //                //throw new Exception("DataStructure ID does not match");
        //            object checkData = info.GetValue("Data", DataType);
        //            this.Data = checkData;
        //            if (checkData.GetType() != DataType)
        //                throw new Exception("DataStructure DataType does not match Data");
        //            if (checkData != Data)
        //                throw new Exception("DataStructure Data does not match");
        //            //TODO: Duplicate DataStructure
        //        }
        //        catch (Exception ex)
        //        {
        //            CoreConsole.Log(ex);
        //        }
        //    }
        //}
        //public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("Data", Data);
        //    //info.AddValue("ID", ID);
        //    //info.AddValue("Bytes", Bytes);
        //    //info.AddValue("DataType", DataType);
        //    //if (this.Data != null)
        //    //{

        //    //}
        //}

        internal DSMetadata _metadata = default;
        protected object _data = default;
        //[DataMember]
        public object Data
        {
            get
            {
                try
                {
                    if (_data != default)
                    {
                        return _data;
                    }
                    else if (this.Count > 0)
                    {
                        if (this.Count == 1)
                        {
                            return this[0].Data;
                        }
                        else if (_metadata != null)
                        {
                            return this.ToArray();
                        }
                        else throw new Exception("Inconsistent Data Format. Metadata is null, but Data Structure has children.");
                    }
                    else return _data;
                }
                catch (Exception ex)
                {
                    CoreConsole.Log(ex);
                    return default;
                }
            }
            set
            {
                try
                {
                    if (value == null) throw new ArgumentNullException("value");
                    if (_metadata != null)
                    {
                        this.Clear();
                        this.Add(value);
                    }
                    else
                    {
                        object old = _data;
                        _data = value;
                        //if (DataChanged != null && DataChanged.GetInvocationList().Length > 0)
                        //    DataChanged(this, new DataChangedEventArgs(old, _data));
                    }
                }
                catch (Exception ex)
                {
                    CoreConsole.Log(ex);
                }
            }
        }
        public Guid ID { get; set; }
        public bool IsValid { get => (this.Data != default); }
        public bool IsEmpty { get => (this.Count == 0 || this.Data == default); }
        public DataStructurePattern DataStructurePattern
        {
            get
            {
                if (_metadata != null)
                {
                    return _metadata.DataStructurePattern;
                }
                else if (_data != null)
                {
                    return DataStructurePattern.Item;
                }
                else return DataStructurePattern.Unknown;
            }
        }
        //public DataAddress Address { get => new DataAddress(this); }
        private Type overrideDataType = default;
        public Type DataType
        {
            get
            {
                if (overrideDataType != default)
                    return overrideDataType;
                if (_metadata != null)
                {
                    return _metadata.DataType;
                }
                else if (this._data != default) return this._data.GetType();
                else return typeof(object);
            }
            set
            {
                try
                {
                    if (_metadata != null)
                    {
                        if (_metadata.DataType == value)
                            overrideDataType = value;
                        else throw new Exception($"Cannot override DataType. DataStructure already has Data of type {_metadata.DataType.Name}.");
                    }
                    else
                    {
                        if (this._data != default)
                        {
                            if (_data.GetType() == value) overrideDataType = value;
                            else throw new Exception($"Cannot override DataType. DataStructure already has Data of type {_data.GetType().Name}.");
                        }
                        else overrideDataType = value;
                    }
                }
                catch (Exception ex)
                {
                    CoreConsole.Log(ex);
                }
            }
        }
        
        //public delegate void DataChangedEventHandler(DataStructure sender, DataChangedEventArgs e);
        //public DataChangedEventHandler DataChanged;

        public DataStructure() : base()
        {
            ID = Guid.NewGuid();
        }
        public DataStructure(object data) : base()
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));
                ID = Guid.NewGuid();
                if (data is IDataGoo)
                {
                    if (data is DataStructure ds)
                    {
                        this.Add(ds);
                    }
                }
                else if (data is object[] array)
                {
                    foreach (object o in array)
                    {
                        if (o == null)
                            throw new ArgumentNullException(nameof(data));
                        this.Add(new DataStructure(o));
                    }
                }
                else
                {
                    Data = data;
                    _metadata = new DSMetadata(this);
                }
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
            }
        }
        public DataStructure(object[] data) : base()
        {
            try
            {
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException(nameof(data));
                ID = Guid.NewGuid();
                foreach (object o in data)
                {
                    if (o == null)
                        throw new ArgumentNullException(nameof(data));
                    this.Add(new DataStructure(o));
                }
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
            }
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
            if (this.DataStructurePattern == DataStructurePattern.Item)
            {
                return this.Data.ToString();
            }
            else if (this.DataStructurePattern == DataStructurePattern.List)
            {
                return $"List [{this.Count}]";
            }
            else if (this.DataStructurePattern == DataStructurePattern.Tree)
            {
                return $"Tree [{this.Count}]";
            }
            else if (this.DataStructurePattern == DataStructurePattern.Object)
            {
                return $"Object {{{this.Count}}}";
            }
            else if (this.DataStructurePattern == DataStructurePattern.EventArgData)
            {
                if (this.Count > 0) return this[0].ToString();
                else return this.Data?.ToString();
            }
            else if (this.DataStructurePattern == DataStructurePattern.Unknown)
            {
                if (this.Data is object[] array)
                {
                    if (array.Length == 1)
                    {
                        return array[0].ToString();
                    }
                    if (array.Length > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Array [");
                        foreach (object o in array)
                        {
                            sb.Append(o.ToString());
                            sb.Append(", ");
                        }
                        sb.Remove(sb.Length - 2, 2);
                        sb.Append("]");
                        return sb.ToString();
                    }
                    else return this.Data?.ToString();
                }
                else return this.Data?.ToString();
            }
            else return this.Data?.ToString();
        }

        public new void Add(object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));
                if (data is IDataGoo dg)
                {
                    if (this.DataType.IsAssignableFrom(dg.DataType))
                    {
                        base.Add(dg);
                        //if (DataChanged != null && DataChanged.GetInvocationList().Length > 0)
                        //    DataChanged(this, new DataChangedEventArgs(dg));
                    }
                    else if (this.DataType.IsAssignableFrom(typeof(DataStructure<>).MakeGenericType(dg.DataType)))
                    {
                        base.Add(dg);
                    }
                    else throw new InvalidCastException($"DataStructure only accepts values of type {this.DataType.Name}");
                }
                else
                {
                    if (this.DataType.IsAssignableFrom(data.GetType()))
                    {
                        base.Add(new DataStructure(data));
                        //if (DataChanged != null && DataChanged.GetInvocationList().Length > 0)
                        //    DataChanged(this, new DataChangedEventArgs(data));
                    }
                    else throw new InvalidCastException($"DataStructure only accepts values of type {this.DataType.Name}");
                }
                if (this.Count > 0)
                {
                    if (_metadata != null)
                    {
                        _metadata.EditedAt = DateTime.Now.ToFileTimeUtc().ToString();
                    }
                    else
                    {
                        _metadata = new DSMetadata(this);
                    }
                }
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
            }
        }

        public new void Insert(int i, object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));
                if (data is IDataGoo dg)
                {
                    if (this.DataType.IsAssignableFrom(dg.DataType))
                    {
                        base.Insert(i, dg);
                        //if (DataChanged != null && DataChanged.GetInvocationList().Length > 0)
                        //    DataChanged(this, new DataChangedEventArgs(dg));
                    }
                    else throw new InvalidCastException($"DataStructure only accepts values of type {this.DataType.Name}");
                }
                else
                {
                    if (this.DataType.IsAssignableFrom(data.GetType()))
                    {
                        base.Insert(i, new DataStructure(data));
                        //if (DataChanged != null && DataChanged.GetInvocationList().Length > 0)
                        //    DataChanged(this, new DataChangedEventArgs(data));
                    }
                    else throw new InvalidCastException($"DataStructure only accepts values of type {this.DataType.Name}");
                }
                if (this.Count > 0)
                {
                    if (_metadata != null)
                    {
                        _metadata.EditedAt = DateTime.Now.ToFileTimeUtc().ToString();
                    }
                    else
                    {
                        _metadata = new DSMetadata(this);
                    }
                }
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
            }
        }

        public DataStructure Duplicate()
        {
            DataStructure dsOut = new DataStructure(this.Data);
            dsOut.ID = ID;
            dsOut.overrideDataType = overrideDataType;
            if (dsOut._metadata != null)
            {
                _metadata.EditedAt = DateTime.Now.ToFileTimeUtc().ToString();
            }
            return dsOut;
        }
        public DataStructure<T> DuplicateAsType<T>()
        {
            try
            {
                DataStructure<T> dsOut = default;
                if (this.Data is null) return dsOut;
                if (this.Data is T castvData)
                {
                    dsOut = new DataStructure<T>(castvData);
                    dsOut.ID = ID;
                    dsOut.overrideDataType = typeof(T);
                    if (dsOut._metadata != null)
                    {
                        _metadata.EditedAt = DateTime.Now.ToFileTimeUtc().ToString();
                    }
                }
                else if (this.Data.GetType().IsArray)
                {
                    if (this.Data is T[] castArrData)
                    {
                        dsOut = new DataStructure<T>(castArrData);
                        dsOut.ID = ID;
                        dsOut.overrideDataType = typeof(T[]);
                        if (dsOut._metadata != null)
                        {
                            _metadata.EditedAt = DateTime.Now.ToFileTimeUtc().ToString();
                        }
                    }
                    else if (this.Data is object[] arr)
                    {
                        dsOut = new DataStructure<T>();
                        dsOut.ID = ID;
                        dsOut.overrideDataType = typeof(T);
                        foreach (object o in arr)
                        {
                            if (o == null) continue;
                            if (o is T castObjData)
                            {
                                dsOut.Add(castObjData);
                            }
                        }
                        if (dsOut._metadata != null)
                        {
                            _metadata.EditedAt = DateTime.Now.ToFileTimeUtc().ToString();
                        }
                    }
                }
                else if (this.Data is DataStructure dsItem)
                {
                    dsOut = new DataStructure<T>();
                    dsOut.ID = ID;
                    dsOut.overrideDataType = typeof(DataStructure<T>);
                    dsOut.Add(dsItem.DuplicateAsType<T>());
                }
                else if (this.Data is DataStructure[] dsArr)
                {
                    dsOut = new DataStructure<T>();
                    dsOut.ID = ID;
                    dsOut.overrideDataType = typeof(DataStructure<T>[]);
                    if (dsArr.Length > 0)
                    {
                        foreach (DataStructure ds in dsArr)
                        {
                            dsOut.Add(ds.DuplicateAsType<T>());
                        }
                    }
                }
                return dsOut;
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
                return default;
            }
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
                CoreConsole.Log(e);
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
                CoreConsole.Log(e);
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
                CoreConsole.Log(e);
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
                CoreConsole.Log(e);
            }

            return rc;
        }

        /// <summary>
        /// InteropSerializationBinder class
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

    internal class DSMetadata
    {
        public Type DataType { get; private set; } = typeof(object);

        public DSMetadata()
        {
            CreatedAt = DateTime.Now.ToFileTimeUtc().ToString();
            EditedAt = DateTime.Now.ToFileTimeUtc().ToString();
        }
        public DSMetadata(DataStructure dataReference)
        {
            CreatedAt = DateTime.Now.ToFileTimeUtc().ToString();
            EditedAt = DateTime.Now.ToFileTimeUtc().ToString();
            if (dataReference != null)
            {
                this.DataType = TryGetConsistentType(dataReference);
            }
        }
        
        private Type TryGetConsistentType(DataStructure dataReference)
        {
            try
            {
                DataStructurePattern valOut = DataStructurePattern.Unknown;
                Type consistentType = typeof(object);
                if (dataReference == default) { this.DataStructurePattern = valOut; return default; }
                if (dataReference.Data == default) { this.DataStructurePattern = valOut; return default; }
                if (dataReference.Data.GetType().IsArray)
                {
                    if (dataReference.Data is object[] array)
                    {
                        if (array.Length == 1)
                        {
                            valOut = DataStructurePattern.Item;
                            consistentType = array[0].GetType();
                        }
                        else if (array.Length > 1)
                        {

                            valOut = DataStructurePattern.List;
                            foreach (DataStructure ds in dataReference)
                            {
                                if (ds._metadata != null)
                                {
                                    valOut = DataStructurePattern.Tree;
                                    if (consistentType.IsAssignableFrom(ds._metadata.DataType))
                                    {
                                        if (consistentType == typeof(object) && ds._metadata.DataType != typeof(object))
                                        {
                                            if (!typeof(DataStructure).IsAssignableFrom(ds._metadata.DataType))
                                            {
                                                consistentType = ds._metadata.DataType;
                                            }
                                            else
                                            {
                                                consistentType = TryGetConsistentType(ds);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        valOut = DataStructurePattern.Object;
                                        consistentType = typeof(object);
                                    }
                                }
                                else throw new Exception("Inconsistent Data Structure : Missing Metadata.");
                            }
                        }
                    }
                }
                else if (dataReference.Data is DataStructure innerDS)
                {
                    consistentType = TryGetConsistentType(innerDS);
                }
                else
                {
                    valOut = DataStructurePattern.Item;
                    consistentType = dataReference.Data.GetType();
                }
                this.DataStructurePattern = valOut;
                return consistentType;
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
                return typeof(object);
            }
        }

        public string CreatedAt { get; internal set; }
        public string EditedAt { get; internal set; }
        public DataStructurePattern DataStructurePattern { get; internal set; } = DataStructurePattern.Unknown;
    }

    public class DataChangedEventArgs<D> : DataChangedEventArgs
    {
        public DataChangedEventArgs(D oldData, D newData) : base(oldData, newData)
        {
        }
        public DataChangedEventArgs(D newData) : base(newData)
        {
        }
        public DataChangedEventArgs(object oldData, D newData) : base(oldData, newData)
        {
        }
        public DataChangedEventArgs(D[] oldData, D[] newData) : base(oldData, newData)
        {
        }
        public DataChangedEventArgs(D[] newData) : base(newData)
        {
        }
        public DataChangedEventArgs(object oldData, D[] newData) : base(oldData, newData)
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
        public DataChangedEventArgs(object[] oldData, object[] newData)
        {
            OldData = oldData;
            NewData = newData;
        }
        public DataChangedEventArgs(object oldData, object[] newData)
        {
            OldData = oldData;
            NewData = newData;
        }
        public DataChangedEventArgs(object[] oldData, object newData)
        {
            OldData = oldData;
            NewData = newData;
        }
        public DataChangedEventArgs(object newData)
        {
            OldData = null;
            NewData = newData;
        }
        public DataChangedEventArgs(object[] newData)
        {
            OldData = null;
            NewData = newData;
        }
    }

    //public class DataAddress
    //{
    //    public IDataGoo AddressOf { get; private set; }
    //    public DataAddress(IDataGoo goo)
    //    {
    //        this.AddressOf = goo;
    //    }
    //    public override string ToString()
    //    {
    //        //TODO: IMPLEMENT DATA ADDRESS
    //        return base.ToString();
    //    }
    //}

    public enum DataState
    {
        Unknown = -1,
        IsNull = 0,
        Internal = 1,
        External = 2,
        Volatile = 3
    }

    public enum ContainerState
    {
        Unknown = -1,
        InputAndOutput = 0,
        InputOnly = 1,
        OutputOnly = 2,
        VolatileOnly = 3
    }

    public enum DataStructurePattern
    {
        Unknown = -1,
        Item = 0,
        List = 1,
        Tree = 2,
        Object = 3,
        EventArgData = 4
    }
}
