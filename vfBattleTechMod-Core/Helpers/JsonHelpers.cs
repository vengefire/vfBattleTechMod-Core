namespace vfBattleTechMod_Core.Helpers
{
    using System.IO;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class JsonHelpers
    {
        public static string AddMissingCommas(string jsonString)
        {
            var rgx = new Regex(@"(\]|\}|""|[A-Za-z0-9])\s*\n\s*(\[|\{|"")", RegexOptions.Singleline);
            var commasAdded = rgx.Replace(jsonString, "$1,\n$2");
            return commasAdded;
        }

        public static JObject Deserialize(string jsonString)
        {
            var commasAdded = AddMissingCommas(jsonString);
            var jsonObject = (JObject)JsonConvert.DeserializeObject(commasAdded);
            return jsonObject;
        }

        public static TObject Deserialize<TObject>(string jsonString)
        {
            var commasAdded = AddMissingCommas(jsonString);
            var typedObject = JsonConvert.DeserializeObject<TObject>(commasAdded);
            return typedObject;
        }

        public static TObject DeserializeObject<TObject>(string jsonString)
        {
            return Deserialize<TObject>(jsonString);
        }

        public static JObject DeserializeFile(string filePath)
        {
            return Deserialize(File.ReadAllText(filePath));
        }

        public static string Prettify(string json)
        {
            using (var stringReader = new StringReader(json))
            {
                using (var stringWriter = new StringWriter())
                {
                    var jsonReader = new JsonTextReader(stringReader);
                    var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                    jsonWriter.WriteToken(jsonReader);
                    return stringWriter.ToString();
                }
            }
        }
    }
}