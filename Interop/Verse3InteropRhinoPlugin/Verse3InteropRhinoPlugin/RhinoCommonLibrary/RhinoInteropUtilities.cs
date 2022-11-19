using System;
using System.IO;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
using Core;
using CoreInterop;
using Rhino;
using Rhino.Geometry;

namespace Verse3RhinoInterop
{
    [DataContract]
    public class RhinoGeometryWrapper
    {
        private GeometryBase _geometry;

        public RhinoGeometryWrapper(GeometryBase geometry)
        {
            _geometry = geometry;
        }
        [DataMember]
        public byte[] Data
        {
            get => ToBytes(_geometry);
            set => _geometry = ToGeometryBase(value);
        }

        /// <summary>
        /// Called during serialization.
        /// Converts an object that inherits from GeometryBase
        /// to an array of bytes.
        /// </summary>
        private static byte[] ToBytes(GeometryBase src)
        {
            var rc = new byte[0];

            if (null == src)
                return rc;

            try
            {
                //var formatter = new BinaryFormatter { Binder = new RockfishDeserializationBinder() };
                //using (var stream = new MemoryStream())
                //{
                //    formatter.Serialize(stream, src);
                //    rc = stream.ToArray();
                //}
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
        public static GeometryBase ToGeometryBase(byte[] bytes)
        {
            if (null == bytes || 0 == bytes.Length)
                return null;

            GeometryBase rc = null;
            try
            {
                using (var stream = new MemoryStream())
                {
                    //var formatter = new BinaryFormatter { Binder = new RockfishDeserializationBinder() };
                    //stream.Write(bytes, 0, bytes.Length);
                    //stream.Seek(0, SeekOrigin.Begin);
                    //var geometry = formatter.Deserialize(stream) as GeometryBase;
                    //if (null != geometry && geometry.IsValid)
                    //    rc = geometry;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
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
        internal class RockfishDeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                var assembly = typeof(GeometryBase).Assembly;
                assemblyName = assembly.ToString();
                var type_to_deserialize = Type.GetType($"{typeName}, {assemblyName}");
                return type_to_deserialize;
            }
        }
    }
}
