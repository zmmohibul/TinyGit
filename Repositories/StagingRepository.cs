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

            await InitRepository.AddFileToStagingArea(filePath);

            // StagingArea.FilesStagedForAddition[filePath] = fileContentSha;

            // await FileSystemUtils.WriteObjectAsync<StagingArea>(StagingArea, "StagingArea", )
        }
    }
}