namespace MiniatureGit.Repositories
{
    public class MainRepository
    {
        protected static string MiniatureGitDirName { get; } = ".minigit";
        protected static string FilesDirectoryPath { get; } = $"./{MiniatureGitDirName}/files";
        protected static string CommitsDirectoryPath { get; } = $"./{MiniatureGitDirName}/commits";
        protected static string BranchesDirectoryPath { get; } = $"./{MiniatureGitDirName}/branches";
        
        protected static readonly string Head = Path.Join(MiniatureGitDirName, "HEAD");
        protected static readonly string RootCommit = Path.Join(MiniatureGitDirName, "RootCommit");

        protected static readonly string CurrentHeadStatePath = $"./{MiniatureGitDirName}";
        protected static readonly string CurrentHeadStateFileName = "CurrHeadState";

        protected static readonly string CurrentBranch = Path.Join(MiniatureGitDirName, "CurrentBranch");
        protected static readonly string Master = Path.Join(BranchesDirectoryPath, "master");

        protected static string StagingAreaPath { get; } = $"./{MiniatureGitDirName}/StagingArea";
    }
}