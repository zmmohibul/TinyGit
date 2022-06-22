using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class CommonRepository
    {
        protected static string MiniatureGitDirName { get; } = ".minigit";
        protected static string FilesDirectoryPath { get; } = $"./{MiniatureGitDirName}/files";
        protected static string CommitsDirectoryPath { get; } = $"./{MiniatureGitDirName}/commits";
        protected static string BranchesDirectoryPath { get; } = $"./{MiniatureGitDirName}/branches";
        
        protected static readonly string Head = Path.Join(MiniatureGitDirName, "HEAD");
        protected static readonly string RootCommit = Path.Join(MiniatureGitDirName, "RootCommit");

        protected static readonly string UnmergedCommit = Path.Join(MiniatureGitDirName, "UnmergedCommit");
        protected static readonly string MergeConfilctData = Path.Join(MiniatureGitDirName, "MergeConflictData");

        protected static readonly string CurrentHeadStatePath = $"./{MiniatureGitDirName}";
        protected static readonly string CurrentHeadStateFileName = "CurrHeadState";

        protected static readonly string CurrentBranch = Path.Join(MiniatureGitDirName, "CurrentBranch");
        protected static readonly string Master = Path.Join(BranchesDirectoryPath, "master");

        protected static string StagingAreaPath { get; } = $"./{MiniatureGitDirName}/StagingArea";

        public static async Task ExitIfThereAreUntrackedChanges()
        {
            var filesStagedForAddition = await StagingRepository.GetFilesStagedForAddition();
            var headCommit = await HeadPointerRepository.GetHeadCommit();

            if (filesStagedForAddition.Count() > 0)
            {
                foreach (var file in filesStagedForAddition.Keys)
                {
                    System.Console.WriteLine($"The file {file} has been modified");
                }

                LogError.Log("Please stage your modified files and make commit before you checkout other commits");
            }

            var fileModifiedButNotTracked = false;
            foreach (var (file, fileSha) in headCommit.Files)
            {
                if (File.Exists(file))
                {
                    var fileInDirSha = await FileSystemUtils.GetShaOfFileContent(file);

                    if (filesStagedForAddition.ContainsKey(file))
                    {
                        if (filesStagedForAddition[file].Equals(fileInDirSha))
                        {
                            continue;
                        }
                    }
                    if (!fileSha.Equals(fileInDirSha))
                    {
                        System.Console.WriteLine($"The file {file.Remove(0, 2)} has been modified since last commit.");
                        fileModifiedButNotTracked = true;
                    }
                }
            }
            if (fileModifiedButNotTracked)
            {
                LogError.Log("Please stage them and make commit before you can checkout to other commits");
            }
        }
    }
}