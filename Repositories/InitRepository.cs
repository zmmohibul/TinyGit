using System.Security.Cryptography;
using System.Text;
using MiniatureGit.Core;
using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class InitRepository
    {
        public static string MiniatureGitDirName { get; } = ".minigit";
        private static readonly DirectoryInfo MiniatureGit = new DirectoryInfo(MiniatureGitDirName);

        public static string FilesDirectoryPath { get; } = $"./{MiniatureGitDirName}/files";
        private static readonly DirectoryInfo Files = new DirectoryInfo(Path.Join(MiniatureGit.FullName, "files"));
        
        public static string CommitsDirectoryPath { get; } = $"./{MiniatureGitDirName}/commits";
        private static readonly DirectoryInfo Commits = new DirectoryInfo(CommitsDirectoryPath);

        public static string BranchesDirectoryPath { get; } = $"./{MiniatureGitDirName}/branches";
        private static readonly DirectoryInfo Branches = new DirectoryInfo(BranchesDirectoryPath);

        public static bool DetachedHeadState { get; set; }
        
        

        private static readonly string Head = Path.Join(MiniatureGit.FullName, "HEAD");
        private static readonly string CurrentBranch = Path.Join(MiniatureGit.FullName, "CurrentBranch");
        private static readonly string Master = Path.Join(Branches.FullName, "master");

        public static string StagingAreaPath { get; } = $"./{MiniatureGitDirName}/StagingArea";
        private static StagingArea SA;

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


            SA = new StagingArea();
            await FileSystemUtils.WriteObjectAsync<StagingArea>(SA, "StagingArea", $"./{MiniatureGitDirName}");
        }

        public static async Task Setup()
        {
            SA = await FileSystemUtils.ReadObjectAsync<StagingArea>(StagingAreaPath);
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

        public static bool IsGitRepo()
        {
            return Directory.Exists($"./{MiniatureGitDirName}");
        }
    }
}