//using Newtonsoft.Json;
//using System;
//using System.IO;
//using System.Text;

//namespace Movies.Client.Util
//{
//    public static class StreamExtensions
//    {
//        public static T JsonDecode<T>(this Stream stream)
//        {
//            if (stream == null) throw new ArgumentNullException(nameof(stream));

//            if (!stream.CanRead) throw new NotSupportedException("Unable to read from stream");

//            using (StreamReader streamReader = new(stream))
//            {
//                using (JsonTextReader jsonTextReader = new(streamReader))
//                {
//                    return new JsonSerializer().Deserialize<T>(jsonTextReader);
//                }
//            }
//        }

//        public static void JsonEncode<T>(this Stream stream, T obj)
//        {
//            if (stream == null) throw new ArgumentNullException(nameof(stream));

//            if (!stream.CanWrite) throw new NotSupportedException("Unable to write into stream.");

//            using (StreamWriter streamWriter = new(stream, new UTF8Encoding(), 1024, true))
//            {
//                using (JsonTextWriter jsonTextWriter = new(streamWriter))
//                {
//                    new JsonSerializer().Serialize(jsonTextWriter, obj);
//                    jsonTextWriter.Flush();
//                }
//            }
//        }
//    }
//}
