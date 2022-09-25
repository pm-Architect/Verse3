using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public static class DataSerialization
    {
        public static byte[] SerializeToBytes<D>(D data) where D : DataStructure
        {
            return DataStructure.ToBytes(data);
        }

        public static DataStructure DeserializeFromBytes(byte[] data)
        {
            return DataStructure.FromBytes(data);
        }

        public static D DeserializeFromBytes<D>(byte[] data) where D : DataStructure
        {
            return DataStructure.FromBytes(data) as D;
        }
    }
}
