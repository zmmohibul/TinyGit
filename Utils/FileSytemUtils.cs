using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MiniatureGit.Utils
{
    public class FileSytemUtils
    {
        public static async Task WriteObjectAsync<T>(T obj, string fileName, string path)
        {
            using FileStream createStream = File.Create(Path.Join(path, fileName));
            await JsonSerializer.SerializeAsync(createStream, obj);
            await createStream.DisposeAsync();
        }

        public static string GetSha1FromObject<T>(T obj)
        {
            var serializedObj = JsonSerializer.Serialize<T>(obj);
            
            using var sha1 = SHA1.Create();
            var objSha = Convert.ToHexString(sha1.ComputeHash(UnicodeEncoding.UTF8.GetBytes(serializedObj)));
            sha1.Dispose();

            return objSha;
        }


    }
}