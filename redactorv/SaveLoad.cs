using Newtonsoft.Json;
using redactorv;
using System;
using System.Collections.Generic;
using System.IO;

namespace redactorv
{
    public static class SaveLoad
    {
        public static void SaveToFile(List<Shape> shapes, string filename)
        {
            string json = JsonConvert.SerializeObject(shapes, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filename, json);
        }

        public static List<Shape> LoadFromFile(string filename)
        {
            string json = File.ReadAllText(filename);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.DeserializeObject<List<Shape>>(json, settings);
        }
    }
}