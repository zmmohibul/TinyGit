using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class BranchingRepository : MainRepository
    {
        public static async Task CreateNewBranch(string branchName)
        {
            var newbranchPath = Path.Join(BranchesDirectoryPath, branchName);
            if (File.Exists(newbranchPath))
            {
                LogError.Log($"A branch with the name {branchName} already exists.");
            }

            var headCommitId = await File.ReadAllTextAsync(Head);
            await File.WriteAllTextAsync(newbranchPath, headCommitId);
        }

    }
}