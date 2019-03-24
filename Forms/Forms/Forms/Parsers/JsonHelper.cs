using Newtonsoft.Json;
using System.IO;

namespace Forms.Parsers
{
    public class JsonHelper<T>
    {
        public static T Deserialize(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                string content = reader.ReadToEnd();
                var result = JsonConvert.DeserializeObject<T>(content);

                return result;
            }
        }

        public static string Serialize(T t)
        {
            var result = JsonConvert.SerializeObject(t);

            return result;
        }
    }
}