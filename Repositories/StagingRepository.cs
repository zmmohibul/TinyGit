using System.Security.Cryptography;
using System.Text;
using MiniatureGit.Core;
using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class StagingRepository : CommonRepository
    {
        private static StagingArea SA;

        public static async Task StageFile(string filePath)
        {
            if (await HeadPointerRepository.IsHeadDetached())
            {
                LogError.Log($"You are in a detached head state!", "Checkout to tip of a branch before staging files.");
            }

            if (!FileSystemUtils.FileExists(filePath))
            {
                LogError.Log($"Could not find '{filePath}' in project directory...");
            }

            if (filePath.StartsWith("./"))
            {
                await AddFileToStagingArea(filePath);
            }
            else
            {
                await AddFileToStagingArea($"./{filePath}");
            }
        }

        public static async Task StageAllFiles()
        {
            if (await HeadPointerRepository.IsHeadDetached())
            {
                LogError.Log($"You are in a detached head state!", "Checkout to tip of a branch before staging files.");
            }
            
            var files = Directory.GetFiles(".", "*.*", SearchOption.AllDirectories)
                .Where(d => !d.StartsWith("./."))
                .Where(d => !d.StartsWith("./MiniatureGit"));

            var headCommit = await HeadPointerRepository.GetHeadCommit();

            foreach (var file in files)
            {
                if (headCommit.Files.ContainsKey(file))
                {
                    var fileInDirSha = await FileSystemUtils.GetShaOfFileContent(file);
                    if (!fileInDirSha.Equals(headCommit.Files[file]))
                    {
                        await AddFileToStagingArea(file);
                    }
                }
                else
                {
                    await AddFileToStagingArea(file);
                }
            }
        }

        public static async Task AddFileToStagingArea(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            var fileContentSha = await FileSystemUtils.GetShaOfFileContent(filePath);

            SA = await FileSystemUtils.ReadObjectAsync<StagingArea>(StagingAreaPath);
            SA.FilesStagedForAddition[filePath] = fileContentSha;

            await FileSystemUtils.WriteObjectAsync<StagingArea>(SA, "StagingArea", $"./{MiniatureGitDirName}");
        }

        public static async Task AddFileToStagingAreaWithSha(string filePath, string fileSha)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            SA.FilesStagedForAddition[filePath] = fileSha;

            await FileSystemUtils.WriteObjectAsync<StagingArea>(SA, "StagingArea", $"./{MiniatureGitDirName}");
        }

        public static async Task StageFileForRemoval(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            SA = await FileSystemUtils.ReadObjectAsync<StagingArea>(StagingAreaPath);
            SA.FilesStagedForRemoval[filePath] = string.Empty;

            await FileSystemUtils.WriteObjectAsync<StagingArea>(SA, "StagingArea", $"./{MiniatureGitDirName}");
        }

        public static async Task<Dictionary<string, string>> GetFilesStagedForAddition()
        {
            SA = await FileSystemUtils.ReadObjectAsync<StagingArea>(StagingAreaPath);
            return SA.FilesStagedForAddition;
        }

        public static async Task ClearStagingArea()
        {
            SA = new StagingArea();
            await FileSystemUtils.WriteObjectAsync<StagingArea>(SA, "StagingArea", $"./{MiniatureGitDirName}");
        }

        public static async Task Setup()
        {
            SA = new StagingArea();
            await FileSystemUtils.WriteObjectAsync<StagingArea>(SA, "StagingArea", $"./{MiniatureGitDirName}");
        }
    }
}