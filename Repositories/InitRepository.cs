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

        private static readonly DirectoryInfo Branches = new DirectoryInfo(Path.Join(MiniatureGit.FullName, "branches"));
        
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

            var c = await File.ReadAllTextAsync(filePath);
            using var sha1 = SHA1.Create();
            var fileContentSha = Convert.ToHexString(sha1.ComputeHash(UnicodeEncoding.UTF8.GetBytes(c)));

            System.Console.WriteLine(filePath);
            System.Console.WriteLine(fileContentSha);
            
            SA = await FileSystemUtils.ReadObjectAsync<StagingArea>(StagingAreaPath);
            SA.FilesStagedForAddition[filePath] = fileContentSha;

            await FileSystemUtils.WriteObjectAsync<StagingArea>(SA, "StagingArea", $"./{MiniatureGitDirName}");
        }

        public static bool IsGitRepo()
        {
            return Directory.Exists($"./{MiniatureGitDirName}");
        }
    }
}