using Newtonsoft.Json;
using System.IO;

namespace CQRSGenerator.Shared;
public class SharedMethods
{
    public static T ParseJsonFile<T>(string filePath)
    {
        var fileContent = File.ReadAllText(filePath);
        var parsdedData = JsonConvert.DeserializeObject<T>(fileContent);
        return parsdedData!;
    }
}
