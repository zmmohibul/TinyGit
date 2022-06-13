using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class StagingRepository
    {
        public static async void StageFile(string filePath)
        {
            if (!FileSystemUtils.FileExists(filePath))
            {
                LogError.Log($"Could not find '{filePath}' in project directory...");
            }
            System.Console.WriteLine(filePath);
            var c = await File.ReadAllTextAsync(filePath);
            // System.Console.WriteLine(c);
            // System.Console.WriteLine(FileSystemUtils.GetSha1FromByteArray(await File.ReadAllBytesAsync(filePath)));
            System.Console.WriteLine(FileSystemUtils.GetSha1FromString(c));


            // await FileSystemUtils.CloneFileFromPath(filePath, InitRepository.FilesDirectoryPath);
        }
    }
}