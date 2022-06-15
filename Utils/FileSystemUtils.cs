using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MiniatureGit.Utils
{
    public class FileSystemUtils
    {
        public static async Task WriteObjectAsync<T>(T obj, string fileName, string path)
        {
            using FileStream createStream = File.Create(Path.Join(path, fileName));
            await JsonSerializer.SerializeAsync(createStream, obj);
            await createStream.DisposeAsync();
        }

        public static async Task<T> ReadObjectAsync<T>(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            using FileStream openStream = File.OpenRead(path);
            T objectToReturn = await JsonSerializer.DeserializeAsync<T>(openStream);
            await openStream.DisposeAsync();

            return objectToReturn;
        }

        public static string GetSha1FromObject<T>(T obj)
        {
            var serializedObj = JsonSerializer.Serialize<T>(obj);
            
            using var sha1 = SHA1.Create();
            var objSha = Convert.ToHexString(sha1.ComputeHash(UnicodeEncoding.UTF8.GetBytes(serializedObj)));
            sha1.Dispose();

            return objSha;
        }

        public static async Task<string> GetShaOfFileContent(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            var c = await File.ReadAllTextAsync(filePath);
            
            using var sha1 = SHA1.Create();
            var fileContentSha = Convert.ToHexString(sha1.ComputeHash(UnicodeEncoding.UTF8.GetBytes(c)));
            sha1.Dispose();

            return fileContentSha;
        }   


        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }


    }
}