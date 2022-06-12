using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class InitRepository
    {
        public static readonly DirectoryInfo PWD = new DirectoryInfo(".");
        public static readonly DirectoryInfo MiniatureGit = new DirectoryInfo(Path.Join(PWD.FullName, ".minigit"));
        public static readonly DirectoryInfo Files = new DirectoryInfo(Path.Join(MiniatureGit.FullName, "files"));
        public static readonly DirectoryInfo Commits = new DirectoryInfo(Path.Join(MiniatureGit.FullName, "commits"));
        public static readonly DirectoryInfo Branches = new DirectoryInfo(Path.Join(MiniatureGit.FullName, "branches"));
        
        public static readonly string Head = Path.Join(MiniatureGit.FullName, "HEAD");
        public static readonly string CurrentBranch = Path.Join(MiniatureGit.FullName, "CurrentBranch");
        public static readonly string Master = Path.Join(Branches.FullName, "master");

        public static void Init()
        {
            if (Directory.Exists(MiniatureGit.FullName))
            {
                LogError.Log("This is an already initialized Git repository...");
            }

            MiniatureGit.Create();
            Files.Create();
            Commits.Create();
            Branches.Create();
        }

        public static bool IsGitRepo()
        {
            return Directory.Exists(MiniatureGit.FullName);
        }
    }
}