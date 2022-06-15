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
                    newCommit.Files[file] = fileSha;
                }
            }

            foreach (var (file, flieSha) in newCommit.Files)
            {
                var fileContent = await File.ReadAllBytesAsync(file);
                await File.WriteAllBytesAsync(Path.Join(InitRepository.FilesDirectoryPath, flieSha), fileContent);
            }

            var newCommitSha = FileSystemUtils.GetSha1FromObject<Commit>(newCommit);
            await FileSystemUtils.WriteObjectAsync<Commit>(newCommit, newCommitSha, InitRepository.CommitsDirectoryPath);
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