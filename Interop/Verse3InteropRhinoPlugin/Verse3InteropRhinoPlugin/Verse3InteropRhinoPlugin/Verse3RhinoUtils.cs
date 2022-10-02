extern alias R3dmIo;
extern alias RCommon;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

using R3 = R3dmIo::Rhino.Geometry;
using RC = RCommon::Rhino.Geometry;

namespace Verse3InteropRhinoPlugin
{
    public class RhCommon3dmConverterSingleton/* : IRhCommon3dmConverter*/
    {

        public bool IsR3dmType(Type type)
        {
            if (type == null)
                return false;
            var inputAssembly = type.Assembly;
            var rcAssembly = typeof(R3.GeometryBase).Assembly;
            return inputAssembly.Equals(rcAssembly);
        }

        public bool IsRCommonType(Type type)
        {
            if (type == null)
                return false;
            var inputAssembly = type.Assembly;
            var rcAssembly = typeof(RC.GeometryBase).Assembly;
            return inputAssembly.Equals(rcAssembly);
        }

        public bool IsRCommonObject(object input)
        {
            if (input == null)
                return false;

            var inputType = input.GetType();
            return IsRCommonType(inputType);
        }

        public bool IsR3dmObject(object input)
        {
            if (input == null)
                return false;
            var inputType = input.GetType();
            return IsR3dmType(inputType);
        }

        /// <summary>
        /// Converts a RhinoCommon Matrix to Rhino3dmIO Matrix
        /// </summary>
        /// <param name="rcommonMatrix">The RhinoCommon matrix</param>
        /// <returns>The converted Rhino3dmIO matrix</returns>
        private R3.Matrix ConvertMatrixFromRCommonToR3dm(RC.Matrix rcommonMatrix)
        {
            R3.Matrix r3dmMatrix = new R3.Matrix(rcommonMatrix.RowCount, rcommonMatrix.ColumnCount);
            for (int i = 0; i < rcommonMatrix.RowCount; i++)
            {
                for (int j = 0; j < rcommonMatrix.ColumnCount; j++)
                {
                    r3dmMatrix[i, j] = rcommonMatrix[i, j];
                }
            }
            return r3dmMatrix;
        }

        private R3.Polyline ConvertPolylineFromRCommonToR3dm(RC.Polyline pline)
        {
            IEnumerable<R3.Point3d> pts = pline.ToList().Select(p => new R3.Point3d(p.X, p.Y, p.Z));
            return new R3.Polyline(pts);
        }

        private RC.Matrix ConvertMatrixFromR3dmToRCommon(R3.Matrix r3dmMatrix)
        {
            RC.Matrix rcommonMatrix = new RC.Matrix(r3dmMatrix.RowCount, r3dmMatrix.ColumnCount);
            for (int i = 0; i < r3dmMatrix.RowCount; i++)
            {
                for (int j = 0; j < r3dmMatrix.ColumnCount; j++)
                {
                    rcommonMatrix[i, j] = r3dmMatrix[i, j];
                }
            }
            return rcommonMatrix;
        }

        private RC.Polyline ConvertPolylineFromR3dmToRCommon(R3.Polyline pline)
        {
            IEnumerable<RC.Point3d> pts = pline.ToList().Select(p => new RC.Point3d(p.X, p.Y, p.Z));
            return new RC.Polyline(pts);
        }

        public object ConvertObjectToRhinoCommon(object input)
        {
            //Matrices aren't supported 
            if (input is R3.Matrix)
            {
                return ConvertMatrixFromR3dmToRCommon((R3.Matrix)input);
            }
            else if (input is R3.Polyline)
            {
                return ConvertPolylineFromR3dmToRCommon((R3.Polyline)input);
            }

            var bytes = ToBytes(input);
            var output = ToR3dmGeometryBase(bytes, typeof(RC.GeometryBase).Assembly);
            return output;
        }

        public object ConvertObjectToR3dm(object input)
        {
            if (input is RC.Matrix)
            {
                return ConvertMatrixFromRCommonToR3dm((RC.Matrix)input);
            }
            else if (input is RC.Polyline)
            {
                return ConvertPolylineFromRCommonToR3dm((RC.Polyline)input);
            }

            var bytes = ToBytes(input);
            var output = ToR3dmGeometryBase(bytes, typeof(R3.GeometryBase).Assembly);
            return output;
        }

        public static RC.GeometryBase ConvertGeometryBaseToRhinoCommon(R3.GeometryBase input)
        {
            var bytes = ToBytes(input);
            var output = ToR3dmGeometryBase(bytes, typeof(RC.GeometryBase).Assembly);
            return output as RC.GeometryBase;
        }

        public static R3.GeometryBase ConvertGeometryBaseToR3dm(RC.GeometryBase input)
        {
            var bytes = ToBytes(input);
            var output = ToR3dmGeometryBase(bytes, typeof(R3.GeometryBase).Assembly);
            return output as R3.GeometryBase;
        }

        /// <summary>
        /// Called during serialization.
        /// Converts an object that inherits from GeometryBase
        /// to an array of bytes.
        /// </summary>
        private static byte[] ToBytes(object src)
        {
            var rc = new byte[0];

            if (null == src)
                return rc;

            try
            {
                var formatter = new BinaryFormatter();
                using (var stream = new MemoryStream())
                {
                    formatter.Serialize(stream, src);
                    rc = stream.ToArray();
                }
            }
            catch (Exception e)
            {
                throw e;
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
        public static object ToR3dmGeometryBase(byte[] bytes, Assembly assembly)
        {
            if (null == bytes || 0 == bytes.Length)
                return null;

            object rc = null;
            try
            {
                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter
                    {
                        Binder =
                        new RhCommon3dmDeserializationBinder(assembly)
                    };
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    var geometry = formatter.Deserialize(stream);
                    if (null != geometry)
                        rc = geometry;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return rc;
        }
    }
    
    public class RhCommon3dmDeserializationBinder : SerializationBinder
    {
        Assembly _assembly;
        public RhCommon3dmDeserializationBinder(Assembly assembly)
        {
            _assembly = assembly;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            var assembly = _assembly;
            assemblyName = assembly.ToString();
            var type_to_deserialize = Type.GetType($"{typeName}, {assemblyName}");
            return type_to_deserialize;
        }
    }
}
