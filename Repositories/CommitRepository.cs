using MiniatureGit.Core;
using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class CommitRepository
    {
        public static async Task MakeCommit(string commitMessage)
        {
            var headCommit = await InitRepository.GetHeadCommit();
            System.Console.WriteLine(headCommit.CommitMessage);
            System.Console.WriteLine(headCommit.CreatedAt);

            var newCommit = CloneCommit(headCommit, commitMessage);

            System.Console.WriteLine(newCommit.CommitMessage);
            System.Console.WriteLine(newCommit.CreatedAt);
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