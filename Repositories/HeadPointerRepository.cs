using MiniatureGit.Core;
using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class HeadPointerRepository : CommonRepository
    {
        public static async Task<Commit> GetHeadCommit()
        {
            var headCommitSha = await File.ReadAllTextAsync(Head);
            var headCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{headCommitSha}");
            return headCommit;
        }

        public static async Task ChangeHead(string newCommitSha)
        {
            await File.WriteAllTextAsync(Head, newCommitSha);
        }

        public static async Task ChangeCurrentBranch(string branchName)
        {
            var branchPath = Path.Join(BranchesDirectoryPath, branchName);
            if (!File.Exists(branchPath))
            {
                throw new FileNotFoundException($"Branch {branchName} not found ");
            }

            await File.WriteAllTextAsync(CurrentBranch, branchName);
        }

        public static async Task ChangeHeadAndCurrentBranch(string newCommitSha)
        {
            await File.WriteAllTextAsync(Head, newCommitSha);
            var currentBranch = await File.ReadAllTextAsync(CurrentBranch);
            await File.WriteAllTextAsync(Path.Join(BranchesDirectoryPath, currentBranch), newCommitSha);
        }

        public static async Task ChangeHeadDetachedState(bool detached)
        {
            var currentHeadState = new CurrentHeadState(detached);
            await FileSystemUtils.WriteObjectAsync<CurrentHeadState>(currentHeadState, CurrentHeadStateFileName, CurrentHeadStatePath);
        }

        public static async Task<bool> IsHeadDetached()
        {
            var currentHeadState = await FileSystemUtils.ReadObjectAsync<CurrentHeadState>($"{CurrentHeadStatePath}/{CurrentHeadStateFileName}");
            return currentHeadState.Detached;
        }

        public static async Task Setup()
        {
            var currentHeadState = new CurrentHeadState(false);
            await FileSystemUtils.WriteObjectAsync<CurrentHeadState>(currentHeadState, CurrentHeadStateFileName, CurrentHeadStatePath);
        }
    }
}