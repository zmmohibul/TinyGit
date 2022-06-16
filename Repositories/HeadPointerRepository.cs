using MiniatureGit.Core;
using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class HeadPointerRepository
    {
        public static string CommitsDirectoryPath { get; } = $"./{InitRepository.MiniatureGitDirName}/commits";
        private static readonly DirectoryInfo Commits = new DirectoryInfo(CommitsDirectoryPath);

        public static string BranchesDirectoryPath { get; } = $"./{InitRepository.MiniatureGitDirName}/branches";
        private static readonly DirectoryInfo Branches = new DirectoryInfo(BranchesDirectoryPath);

        public static bool DetachedHeadState { get; set; }

        private static readonly string CurrentHeadStatePath = $"./{InitRepository.MiniatureGitDirName}";
        private static readonly string CurrentHeadStateFileName = "CurrHeadState";

        private static readonly string Head = $"./{InitRepository.MiniatureGitDirName}/HEAD";
        private static readonly string CurrentBranch = $"./{InitRepository.MiniatureGitDirName}/CurrentBranch";
        private static readonly string Master = Path.Join(Branches.FullName, "master");

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
    }
}