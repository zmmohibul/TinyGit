using MiniatureGit.Core;
using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class CommitRepository
    {
        public static async Task MakeCommit(string commitMessage)
        {
            var headCommit = await InitRepository.GetHeadCommit();

            var newCommit = CloneCommit(headCommit, commitMessage);

            var filesStagedForAddition = await InitRepository.GetFilesStagedForAddition();
            foreach (var (file, fileSha) in filesStagedForAddition)
            {
                if (File.Exists(file))
                {
                    var fileInDirSha = await FileSystemUtils.GetShaOfFileContent(file);
                    if (!fileSha.Equals(fileInDirSha))
                    {
                        LogError.Log($"The file {file.Remove(0, 2)} has been modified since last staging.");
                    }
                }
            }

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