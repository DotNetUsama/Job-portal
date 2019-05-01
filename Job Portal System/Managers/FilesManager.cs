using System.IO;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Hosting;

namespace Job_Portal_System.Managers
{
    public class FilesManager
    {
        public static string Store<T>(IHostingEnvironment env, string directory, T objectToWrite)
        {
            var fileName = Path.GetRandomFileName();
            Store(env, directory, fileName, objectToWrite);
            return fileName;
        }
        
        public static void Store<T>(IHostingEnvironment env, string directory, 
            string fileName, T objectToWrite)
        {
            directory = Path.Combine("Files", directory);
            var filePath = Path.Combine(env.ContentRootPath, directory, fileName);
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        public static T Read<T>(IHostingEnvironment env, string directory, string fileName)
        {
            directory = Path.Combine("Files", directory);
            var filePath = Path.Combine(env.ContentRootPath, directory, fileName);
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
    }
}
