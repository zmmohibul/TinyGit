using System.Security.Cryptography;
using System.Text;
using MiniatureGit.Core;
using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class StagingRepository
    {
        public static async Task StageFile(string filePath)
        {
            if (!FileSystemUtils.FileExists(filePath))
            {
                LogError.Log($"Could not find '{filePath}' in project directory...");
            }

            if (filePath.StartsWith("./"))
            {
                await InitRepository.AddFileToStagingArea(filePath);
            }
            else
            {
                await InitRepository.AddFileToStagingArea($"./{filePath}");
            }
        }

        public static async Task StageAllFiles()
        {
            var files = Directory.GetFiles(".", "*.*", SearchOption.AllDirectories)
                .Where(d => !d.StartsWith("./."))
                .Where(d => !d.StartsWith("./MiniatureGit"));

            var headCommit = await InitRepository.GetHeadCommit();

            foreach (var file in files)
            {
                System.Console.WriteLine(file);
                if (headCommit.Files.ContainsKey(file))
                {
                    var fileInDirSha = await FileSystemUtils.GetShaOfFileContent(file);
                    if (!fileInDirSha.Equals(headCommit.Files[file]))
                    {
                        await InitRepository.AddFileToStagingArea(file);
                    }
                }
                else
                {
                    await InitRepository.AddFileToStagingArea(file);
                }
            }
        }
    }
}