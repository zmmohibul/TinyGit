using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MiniatureGit.Utils
{
    public class FileSystemUtils
    {
        /*
        * Writes File To Given Path By Its Content SHA Name
        */
        // public static async Task CloneFileFromPath(string fileToClonePath, string clonnedFilePath)
        // {
        //     System.Console.WriteLine(fileToClonePath);
        //     System.Console.WriteLine(clonnedFilePath);
        //     var fileToCloneContentSha = await GetShaFromPathAsync(fileToClonePath);
        //     System.Console.WriteLine(fileToCloneContentSha);
        //     await WriteFileFromByteArrayAsync(fileToCloneContentSha, clonnedFilePath, await ReadFileAsByteArrayAsync(fileToClonePath));
        // }

        public static async Task WriteFileFromByteArrayAsync(string fileName, string filePath, byte[] content)
        {
            await File.WriteAllBytesAsync(Path.Join(filePath, fileName), content);
        }

        public static async Task<byte[]> ReadFileAsByteArrayAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            var content = await File.ReadAllBytesAsync(filePath);
            return content;
        }

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

        public static string GetSha1FromByteArray(byte[] input)
        {
            using var sha1 = SHA1.Create();
            var contentSha = Convert.ToHexString(sha1.ComputeHash(input));
            sha1.Dispose();

            return contentSha;
        }

        public static string GetSha1FromString(string input)
        {
            using var sha1 = SHA1.Create();
            var contentSha = Convert.ToHexString(sha1.ComputeHash(UnicodeEncoding.UTF8.GetBytes(input)));
            sha1.Dispose();

            return contentSha;
        }


        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }


    }
}