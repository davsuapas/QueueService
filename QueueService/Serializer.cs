using System;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Xml;

namespace QueueService {

    public static class Serializer {

        public static byte[] Serialize<T>(T instance, IEnumerable<Type> knownTypes = null) where T : class {

            var serializer = new DataContractSerializer(typeof(T), knownTypes, 1000000, false, true, null);

            return Serialization(instance, serializer);
        }

        public static byte[] Serialize(object instance, IEnumerable<Type> knownTypes = null) {
            var serializer = new DataContractSerializer(instance.GetType(), knownTypes);
            return Serialization(instance, serializer);
        }

        private static byte[] Serialization(object instance, DataContractSerializer serializer) {

            using (MemoryStream memStream = new MemoryStream()) {

                XmlDictionaryWriter binaryDictionaryWriter = XmlDictionaryWriter.CreateBinaryWriter(memStream);
                serializer.WriteObject(binaryDictionaryWriter, instance);
                binaryDictionaryWriter.Flush();

                return memStream.ToArray();
            }

        }

        public static T Unserialize<T>(byte[] bytes, IEnumerable<Type> knownTypes = null) where T : class {

            return Unserialize(typeof(T), bytes, knownTypes) as T;
        }

        public static object Unserialize(Type type, byte[] bytes, IEnumerable<Type> knownTypes = null) {

            var deserializer = new DataContractSerializer(type, knownTypes);

            using (MemoryStream memStream = new MemoryStream(bytes))
            {
                XmlDictionaryReader binaryDictionaryReader = XmlDictionaryReader.CreateBinaryReader(memStream, XmlDictionaryReaderQuotas.Max);
                return deserializer.ReadObject(binaryDictionaryReader);
            }
        }
    }
}
