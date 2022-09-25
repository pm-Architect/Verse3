using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

namespace Core
{
    [ProtoContract]
    public interface IDataGoo : INotifyPropertyChanged, ISerializable
    {
        [DataMember]
        [ProtoMember(1)]
        [XmlElement("Data")]
        public string DataXml { get; }
        //public byte[] Bytes { get; }
        public Guid ID { get; }
        //[ProtoMember(2)]
        public IDataGoo Parent { get; }
        //[ProtoMember(3)]
        public DataStructure Children { get; }
        bool IsValid { get; }
        string IsValidReason { get; }
        //[ProtoMember(4)]
        public DataAddress Address { get; }
        [ProtoMember(5)]
        [DataMember]
        public DataType DataType { get; }
        //[DataMember]
        //[ProtoMember(6)]
        [IgnoreDataMember]
        [XmlIgnore]
        [ProtoIgnore]
        object Data { get; set; }

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
            catch (Exception)
            {

                throw;
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
        [DataMember]
        [ProtoMember(5)]
        public new DataType DataType => typeof(D);

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
    [ProtoContract]
    public class DataStructure : DataLinkedList<IDataGoo>, IDataGoo, ISerializable
    {
        #region ISerializable Members

        //[DataMember]
        //[ProtoMember(1)]
        [DataMember]
        [ProtoMember(1)]
        [XmlElement("Data")]
        public string DataXml
        {
            get
            {
                try
                {
                    //Serialize the data to XML string
                    XmlSerializer serializer = new XmlSerializer(this.Data.GetType());
                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, this.Data);
                        return writer.ToString();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return ("<deserialization_error>" + ex.Message + "</deserialization_error>");
                }
            }
            set
            {
                try
                {
                    //Deserialize the data from XML string
                    XmlSerializer serializer = new XmlSerializer(this.Data.GetType());
                    using (StringReader reader = new StringReader(value))
                    {
                        object temp = serializer.Deserialize(reader);
                        if (temp.GetType() == this.Data.GetType())
                        {
                            this.Data = temp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Data = "<deserialization_error>" + ex.Message + "</deserialization_error>";
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
        //public byte[] Bytes
        //{
        //    get
        //    {
        //        return DataStructure.ToBytes(this.Data);
        //    }
        //    set
        //    {
        //        try
        //        {
        //            this.Data = DataStructure.FromBytes(value, DataType);
        //        }
        //        catch (Exception ex)
        //        {
        //            this.Data = "<deserialization_error>" + ex.Message + "</deserialization_error>";
        //            System.Diagnostics.Debug.WriteLine(ex.Message);
        //        }
        //    }
        //}
        public DataStructure(SerializationInfo info, StreamingContext context)
        {
            if (info.ObjectType == typeof(DataStructure))
            {
                try
                {
                    //byte[] bytes = { };
                    //bytes = (byte[])info.GetValue("Bytes", bytes.GetType());
                    //DataStructure temp = DataStructure.FromBytes(bytes);
                    //this.volatileData = temp.volatileData;
                    //this.Children = temp.Children;
                    //this.Parent = temp.Parent;
                    //this.Address = temp.Address;
                    //this.DataType = temp.DataType;
                    //this.ID = temp.ID;
                    //this.Data = temp.Data;
                    Guid checkGuid = (Guid)info.GetValue("ID", typeof(Guid));
                    if (checkGuid != ID)
                        throw new Exception("DataStructure ID does not match");
                    object checkData = info.GetValue("Data", DataType.Type);
                    if (checkData.GetType() != DataType.Type)
                        throw new Exception("DataStructure DataType does not match Data");
                    if (checkData != Data)
                        throw new Exception("DataStructure Data does not match");
                    //TODO: Duplicate DataStructure
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Data", Data);
            info.AddValue("ID", ID);
            //info.AddValue("Bytes", Bytes);
            //info.AddValue("DataType", DataType);
            //if (this.Data != null)
            //{

            //}
        }

        #endregion

        protected object volatileData = default;
        //[DataMember]
        //[ProtoMember(4)]
        [IgnoreDataMember]
        [XmlIgnore]
        [ProtoIgnore]
        public object Data { get => volatileData; set => volatileData = value; }
        public Guid ID { get; set; }
        public bool IsValid { get => (this.Data != default); }
        public string IsValidReason { get; }
        //[DataMember]
        //[ProtoMember(2)]
        public IDataGoo Parent { get; }
        //[DataMember]
        //[ProtoMember(3)]
        public DataStructure Children { get; }
        public bool IsEmpty { get; }
        //[DataMember]
        //[ProtoMember(4)]
        public DataAddress Address { get; }
        private Type overrideDataType = default;
        //[DataMember]
        //[ProtoMember(5)]
        public DataType DataType
        {
            get
            {
                if (overrideDataType != default)
                    return overrideDataType;
                return volatileData.GetType();
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
            return this.Data.ToString();
        }


        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        #endregion

        ///// <summary>
        ///// Called during serialization.
        ///// Converts an object that inherits from GeometryBase
        ///// to an array of bytes.
        ///// </summary>
        //public static byte[] ToBytes(DataStructure src)
        //{
        //    var rc = new byte[0];

        //    if (src == null || src.Data == null)
        //        return rc;

        //    try
        //    {
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            Serializer.Serialize(ms, src);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine(e.Message);
        //    }

        //    return rc;
        //}

        /// <summary>
        /// Called during serialization.
        /// Converts an object that inherits from GeometryBase
        /// to an array of bytes.
        /// </summary>
        public static byte[] ToBytes(object src)
        {
            var rc = new byte[0];

            if (src == null)
                return rc;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //Serializer.Serialize(ms, src);
                    XmlSerializer writer = new XmlSerializer(src.GetType());
                    writer.Serialize(ms, src);
                    rc = ms.ToArray();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
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
                using (MemoryStream ms = new MemoryStream())
                {
                    Serializer.Serialize<D>(ms, src);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
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
        public static object FromBytes(byte[] bytes, Type type)
        {
            if (null == bytes || 0 == bytes.Length)
                return null;

            object rc = null;
            try
            {
                using (var stream = new MemoryStream())
                {
                    XmlSerializer deserializer = new XmlSerializer(type);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Position = 0;
                    object temp = deserializer.Deserialize(stream);
                    if (temp.GetType() == type)
                    {
                        rc = temp;
                    }
                    else throw new InvalidCastException("Deserialized object " + temp.GetType().ToString() + " is not of type " + type.ToString());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
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
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Position = 0;
                    rc = Serializer.Deserialize<DataStructure>(stream);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
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
        public static Type FromBytes(byte[] bytes, ref DataStructure deserialized)
        {
            if (null == bytes || 0 == bytes.Length)
                deserialized = null;

            DataStructure rc = null;
            try
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Position = 0;
                    rc = Serializer.Deserialize<DataStructure>(stream);
                }
                deserialized = rc;
                return deserialized.GetType();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
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
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Position = 0;
                    rc = Serializer.Deserialize<D>(stream);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return rc;
        }

        ///// <summary>
        ///// RockfishDeserializationBinder class
        ///// </summary>
        ///// <remarks>
        ///// Both RhinoCommon and Rhino3dmIO have a Rhino.Geometry.GeometryBase
        ///// class. This serialization binder help deserialize the equivalent 
        ///// objects across the different assemblies.
        ///// </remarks>
        //internal class InteropSerializationBinder<D> : SerializationBinder where D : DataStructure
        //{
        //    public override Type BindToType(string assemblyName, string typeName)
        //    {
        //        var assembly = typeof(D).Assembly;
        //        if (typeof(D).ContainsGenericParameters)
        //        {
        //            //foreach (Type t in typeof(D).GetGenericParameterConstraints())
        //            //{
        //            //}
        //        }
        //        assemblyName = assembly.ToString();
        //        var type_to_deserialize = Type.GetType($"{typeName}, {assemblyName}");
        //        return type_to_deserialize;
        //    }
        //}
    }

    public class SerializableDataWrapper<T>
    {

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

    [ProtoContract]
    [Serializable]
    [DataContract]
    public class DataAddress
    {
        [ProtoMember(1)]
        [DataMember]
        public string Address { get => AddressOf.ToString(); set => AddressOf = new Guid(value); }
        public Guid AddressOf { get; private set; }
        public DataAddress(IDataGoo goo)
        {
            this.AddressOf = goo.ID;
        }
        public override string ToString()
        {
            //TODO: IMPLEMENT DATA ADDRESS
            return Address;
        }
    }

    [ProtoContract]
    [Serializable]
    [DataContract]
    public class DataType
    {
        public Type Type { get; private set; }
        [ProtoMember(1)]
        [DataMember]
        public byte[] Bytes
        {
            get
            {
                return DataStructure.ToBytes(Type);
            }
            set
            {
                try
                {
                    Type = (Type)DataStructure.FromBytes(value, typeof(Type));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    this.Type = null;
                }
            }
        }

        public DataType()
        {
            //this.Type = null;
        }

        public DataType(Type type)
        {
            this.Type = type;
        }

        public static implicit operator Type(DataType v)
        {
            return v.Type;
        }
        public static implicit operator DataType(Type v)
        {
            return new DataType(v);
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
