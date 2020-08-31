using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
namespace task2.DataManager
{
    class JsonDataManager : IDataManager
    {
        public IEnumerable<T> LoadAndDeserialize<T>() where T : class, new()
        {
            string fileName = typeof(T).Name + ".json";
            if (File.Exists(fileName))
            {
                return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(fileName, Encoding.UTF8));
            }
            else
            {
                File.Create(fileName);
                return new List<T>();                
            }
        }

        public void SaveToFile<T>(IEnumerable<T> ts) where T : class
        {
            string fileName = typeof(T).Name + ".json";
            if (!File.Exists(fileName)) File.Create(fileName);
            File.WriteAllText(
                fileName,
                JsonConvert.SerializeObject(ts, Formatting.Indented),
                Encoding.UTF8); 
        }
    }
}
