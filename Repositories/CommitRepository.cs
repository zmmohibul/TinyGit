using MiniatureGit.Core;
using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class CommitRepository
    {
        public static async Task MakeCommit(string commitMessage)
        {
            var fileModifiedButNotTracked = false;

            var filesStagedForAddition = await InitRepository.GetFilesStagedForAddition();
            foreach (var (file, fileSha) in filesStagedForAddition)
            {
                if (File.Exists(file))
                {
                    var fileInDirSha = await FileSystemUtils.GetShaOfFileContent(file);
                    if (!fileSha.Equals(fileInDirSha))
                    {
                        System.Console.WriteLine($"The file {file.Remove(0, 2)} has untracked changes since it was last staged.");
                        fileModifiedButNotTracked = true;
                        LogError.Log();
                    }
                }
            }

            var headCommit = await InitRepository.GetHeadCommit();
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
                LogError.Log("\nPlease add modified files and untracked changes to staging area before making commit.");
            }

            if (filesStagedForAddition.Count() == 0)
            {
                var noChangeSinceLastCommit = true;
                foreach (var (file, fileSha) in headCommit.Files)
                {
                    var fileInDirSha = await FileSystemUtils.GetShaOfFileContent(file);
                    if (!fileSha.Equals(fileInDirSha))
                    {
                        noChangeSinceLastCommit = false;
                    }
                }

                if (noChangeSinceLastCommit)
                {
                    LogError.Log("No new changes to commit");
                }
            }

            var newCommit = CloneCommit(headCommit, commitMessage);
            foreach (var (file, fileSha) in filesStagedForAddition)
            {
                newCommit.Files[file] = fileSha;

                var fileContent = await File.ReadAllBytesAsync(file);
                if (!File.Exists(Path.Join(InitRepository.FilesDirectoryPath, fileSha)))
                {
                    await File.WriteAllBytesAsync(Path.Join(InitRepository.FilesDirectoryPath, fileSha), fileContent);
                }
            }

            var newCommitSha = FileSystemUtils.GetSha1FromObject<Commit>(newCommit);
            await FileSystemUtils.WriteObjectAsync<Commit>(newCommit, newCommitSha, InitRepository.CommitsDirectoryPath);

            await InitRepository.ChangeHeadAndCurrentBranch(newCommitSha);
            await InitRepository.ClearStagingArea();
        }

        public static async Task Checkout(string commitId)
        {
            var commitIdExist = File.Exists(Path.Join(InitRepository.CommitsDirectoryPath, commitId));
            if (!commitIdExist)
            {
                LogError.Log("Invalid Commit Id...");
            }

            await CheckoutCommit(commitId);
        }

        public static async Task CheckoutBranch(string branchName)
        {
            var branchExists = File.Exists(Path.Join(InitRepository.BranchesDirectoryPath, branchName));
            if (!branchExists)
            {
                LogError.Log("Invalid branch name...");
            }

            var commitId = await File.ReadAllTextAsync(Path.Join(InitRepository.BranchesDirectoryPath, branchName));

            await CheckoutCommit(commitId);
        }

        public static async Task LogCommits()
        {
            var commitsSha = Directory.GetFiles(InitRepository.CommitsDirectoryPath);
            System.Console.WriteLine();
            foreach (var sha in commitsSha)
            {
                var commit = await FileSystemUtils.ReadObjectAsync<Commit>(sha);
                System.Console.WriteLine($"Commit Message: {commit.CommitMessage}");
                System.Console.WriteLine($"Commited At: {commit.CreatedAt}");
                System.Console.WriteLine($"Commit Id: {Path.GetRelativePath(InitRepository.CommitsDirectoryPath, sha)}");
                System.Console.WriteLine("=============================================================================\n");
            }
        }

        private static async Task CheckoutCommit(string commitId)
        {
            var commitToCheckout = await FileSystemUtils.ReadObjectAsync<Commit>(Path.Join(InitRepository.CommitsDirectoryPath, commitId));

            var files = Directory.GetFiles(".", "*.*", SearchOption.AllDirectories)
                .Where(d => !d.StartsWith("./."))
                .Where(d => !d.StartsWith("./MiniatureGit"));
            
            foreach (var file in files)
            {
                File.Delete(file);
            }

            foreach (var (file, fileSha) in commitToCheckout.Files)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                var fileContent = await File.ReadAllBytesAsync(Path.Join(InitRepository.FilesDirectoryPath, fileSha));
                await File.WriteAllBytesAsync(file, fileContent);
            }
        }

        private static Commit CloneCommit(Commit commitToClone, string newCommitMessage)
        {
            var commitToCloneSha = FileSystemUtils.GetSha1FromObject<Commit>(commitToClone);

            var commitToReturn = new Commit(newCommitMessage, commitToCloneSha);

            foreach (var (file, fileSha) in commitToClone.Files)
            {
                commitToReturn.Files[file] = fileSha;
            }

            return commitToReturn;
        }
    }
}