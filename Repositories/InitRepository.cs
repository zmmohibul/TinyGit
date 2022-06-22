using System.Security.Cryptography;
using System.Text;
using MiniatureGit.Core;
using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class InitRepository : CommonRepository
    {
        protected static readonly DirectoryInfo MiniatureGit = new DirectoryInfo(MiniatureGitDirName);
        protected static readonly DirectoryInfo Files = new DirectoryInfo(Path.Join(MiniatureGit.FullName, "files"));
        protected static readonly DirectoryInfo Commits = new DirectoryInfo(CommitsDirectoryPath);
        protected static readonly DirectoryInfo Branches = new DirectoryInfo(BranchesDirectoryPath);

        public static async Task Init()
        {
            if (IsGitRepo())
            {
                LogError.Log("This is an already initialized Git repository...");
            }

            MiniatureGit.Create();
            Files.Create();
            Commits.Create();
            Branches.Create();

            var initialCommit = new Commit();

            var initialCommitSha = FileSystemUtils.GetSha1FromObject<Commit>(initialCommit);
            await FileSystemUtils.WriteObjectAsync<Commit>(initialCommit, initialCommitSha, CommitsDirectoryPath);

            await File.WriteAllTextAsync(Master, initialCommitSha);
            await File.WriteAllTextAsync(Head, initialCommitSha);
            await File.WriteAllTextAsync(CurrentBranch, "master");
            await File.WriteAllTextAsync(RootCommit, initialCommitSha);
            
            await StagingRepository.Setup();
            await HeadPointerRepository.Setup();
        }

        public static bool IsGitRepo()
        {
            return Directory.Exists($"./{MiniatureGitDirName}");
        }
    }
}