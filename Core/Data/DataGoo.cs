using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        public new object Data
        {
            get
            {
                if (volatileData == default)
                {
                    if (Children == default || Children.Count == 0)
                        return default;
                    else
                        return Children.ToArray();
                }
                else
                {
                    if (Children == default || Children.Count == 0) return volatileData;
                    else
                    {
                        if (volatileData is DSMetadata)
                        {
                            return Children.ToArray();
                        }
                        else
                        {
                            //Children.Clear();
                            return volatileData;
                        }
                    }
                }
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value is D castData)
                {
                    D old = default;
                    if (base.volatileData != null && base.volatileData is D castOldData) old = castOldData;
                    base.volatileData = castData;
                    if (DataChanged != null && DataChanged.GetInvocationList().Length > 0)
                        DataChanged(this, new DataChangedEventArgs<D>(old, castData));
                }
            }
            //get
            //{
            //    if (volatileData == default)
            //    {
            //        if (Children == default || Children.Count == 0)
            //            return default;
            //        else
            //            return Children.ToArray();
            //    }
            //    else
            //    {
            //        if (Children == default || Children.Count == 0) return volatileData;
            //        else
            //        {
            //            if (volatileData is DSMetadata)
            //            {
            //                return Children.ToArray();
            //            }
            //            else
            //            {
            //                //Children.Clear();
            //                return volatileData;
            //            }
            //        }
            //    }
            //}
            //set
            //{
            //    if (value == null) throw new ArgumentNullException("value");
            //    if (value is DataStructure)
            //    {
            //        DataStructure ds = ((DataStructure)value);
            //        this.Children.Add(ds);
            //    }
            //    else
            //    {
            //        volatileData = value;
            //    }
            //}
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
        public DataStructure(D[] data) : base()
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));
            foreach (D d in data)
            {
                this.Add(new DataStructure<D>(d));
            }
        }
        public DataStructure(IDataGoo<D> data) : base(new List<IDataGoo<D>> { data })
        {
            ID = Guid.NewGuid();
        }
        public DataStructure(IEnumerable<IDataGoo<D>> data) : base(data)
        {
            ID = Guid.NewGuid();
        }

        public new void Add(D data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            base.Add(new DataStructure<D>(data));
        }
        public new void Add(object data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            if (data is D castData)
            {
                base.Add(new DataStructure<D>(castData));
            }
            else if (data is DataStructure ds)
            {
                base.Add(ds);
            }
            else if (data.GetType().IsArray)
            {
                Type arrayType = data.GetType().GetElementType();
                if (arrayType == typeof(D))
                {
                    foreach (D d in (D[])data)
                    {
                        base.Add(new DataStructure<D>(d));
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
        public DataStructure<T> Duplicate<T>()
        {
            try
            {
                DataStructure<T> dsOut;
                if (volatileData is T castData)
                {
                    return new DataStructure<T>(castData);
                }
                else if (volatileData is DataStructure ds)
                {
                    dsOut = ds.Duplicate<T>();
                }
                else
                {
                    dsOut = new DataStructure<T>();
                }
                dsOut.ID = ID;
                if (this.Count > 0)
                {
                    foreach (IDataGoo goo in this)
                    {
                        if (goo is DataStructure<T>)
                        {
                            dsOut.Add(((DataStructure<T>)goo).Duplicate());
                        }
                        else if (goo.Data is T)
                        {
                            dsOut.Add(new DataStructure<T>((T)goo.Data));
                        }
                        else
                        {
                            DataStructure<T> gooT = new DataStructure<T>((T)goo.Data);
                            dsOut.Add(goo);
                        }
                    }
                }
                return dsOut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [Serializable]
    [DataContract]
    public class DataStructure : DataLinkedList<IDataGoo>, IDataGoo, ISerializable
    {
        public object[] ToArray()
        {
            if (this.Count > 0)
            {
                object[] objectsOut = new object[this.Count];
                for (int i = 0; i < this.Count; i++)
                {
                    objectsOut[i] = this[i].Data;
                }
                return objectsOut;
            }
            else
            {
                return null;
            }
        }
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
        public object Data
        {
            get
            {
                if (volatileData == default)
                {
                    if (Children == default || Children.Count == 0)
                        return default;
                    else if (Children.Count > 1)
                    {
                        volatileData = new DSMetadata(this);
                        return this.ToArray();
                    }
                    else
                        return this.ToArray();
                }
                else
                {
                    if (Children != default)
                    {
                        if (Children.Count > 0)
                        {
                            Children.Add(volatileData);
                            volatileData = new DSMetadata(this);
                            return this.ToArray();
                        }
                        else return volatileData;
                    }
                    else return volatileData;
                }
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value is DataStructure)
                {
                    DataStructure ds = ((DataStructure)value);
                    this.Add(ds);
                }
                else
                {
                    volatileData = value;
                }
            }
        }
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
            if (data.GetType().IsArray)
            {
                if (data is object[] dataArray)
                {
                    foreach (object o in dataArray)
                    {
                        if (o == null)
                            throw new ArgumentNullException(nameof(o));
                        this.Add(new DataStructure(o));
                    }
                }
            }
            volatileData = data;
            ID = Guid.NewGuid();
        }
        public DataStructure(object[] data) : base()
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));
            foreach (object o in data)
            {
                if (o == null)
                    throw new ArgumentNullException(nameof(data));
                this.Add(new DataStructure(o));
            }
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

        public new void Add(object data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data is IDataGoo)
                base.Add((IDataGoo)data);
            else
                base.Add(new DataStructure(data));
            if (this.Count > 0)
            {
                volatileData = new DSMetadata(this);
            }
        }

        public DataStructure Duplicate()
        {
            DataStructure dsOut = new DataStructure();
            if (volatileData != null) dsOut = new DataStructure(volatileData);
            dsOut.ID = ID;
            dsOut.overrideDataType = overrideDataType;
            if (this.Count > 0)
            {
                foreach (IDataGoo goo in this)
                {
                    if (goo is DataStructure)
                    {
                        dsOut.Add(((DataStructure)goo).Duplicate());
                    }
                }
            }
            //TODO: update DSMetadata's EditedAt timestamp
            return dsOut;
        }
        public DataStructure<T> Duplicate<T>()
        {
            DataStructure<T> dsOut = new DataStructure<T>();
            if (volatileData != null && volatileData is T) dsOut = new DataStructure<T>((T)volatileData);
            dsOut.ID = ID;
            if (this.Count > 0)
            {
                foreach (IDataGoo goo in this)
                {
                    if (goo is DataStructure<T>)
                    {
                        dsOut.Add(((DataStructure<T>)goo).Duplicate());
                    }
                    else if ((typeof(DataStructure)).IsAssignableFrom(this.DataType))
                    {
                        //TODO: Improve DataStructure depth with a while loop to iterate into branches
                        DataStructure currDS = (DataStructure)this.Data;
                        while ((typeof(DataStructure)).IsAssignableFrom(currDS.DataType))
                        {
                            currDS = (DataStructure)currDS.Data;
                        }
                        if (currDS.Data is T)
                        {
                            dsOut = currDS.Duplicate<T>();
                        }
                    }
                    else
                    {
                        DataStructure<T> gooT = new DataStructure<T>((T)goo.Data);
                        dsOut.Add(goo);
                    }
                }
            }
            return dsOut;
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

    internal class DSMetadata
    {
        public DSMetadata()
        {
            CreatedAt = DateTime.Now;
            EditedAt = DateTime.Now;
        }
        public DSMetadata(DataStructure dataReference)
        {
            CreatedAt = DateTime.Now;
            EditedAt = DateTime.Now;
            if (dataReference != null)
            {
                if (dataReference is EventArgData eData)
                {
                    //DSType = DataStructureType.EventArgData;
                    
                    //Iterate into DataStructure
                    DataStructure currDS = eData as DataStructure;
                    
                    if (currDS.Data is DSMetadata)
                    {
                        while (currDS.Data is DSMetadata)
                        {
                            if (currDS.Data is DataStructure innerDS)
                                currDS = innerDS;
                        }
                    }
                    
                    //if (eData.Data is DataStructure ds)
                    //{

                    //}
                    //else if (eData.Data is DSMetadata)
                    //{

                    //}
                }
            }
        }
        public DateTime CreatedAt { get; internal set; }
        public DateTime EditedAt { get; internal set; }
        public DataStructurePattern DataStructurePattern { get; internal set; }
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
