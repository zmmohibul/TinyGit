using MiniatureGit.Core;
using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class BranchingRepository : MainRepository
    {
        public static async Task CreateNewBranch(string branchName)
        {
            var newbranchPath = Path.Join(BranchesDirectoryPath, branchName);
            if (File.Exists(newbranchPath))
            {
                LogError.Log($"A branch with the name {branchName} already exists.");
            }

            var headCommitId = await File.ReadAllTextAsync(Head);
            await File.WriteAllTextAsync(newbranchPath, headCommitId);
        }

        public static async Task MergeBranch(string givenBranchName)
        {
            var givenBranchPath = Path.Join(BranchesDirectoryPath, givenBranchName);
            if (!File.Exists(givenBranchPath))
            {
                LogError.Log($"No branch with the name {givenBranchName} exists.");
            }

            var currentBranchName = await File.ReadAllTextAsync(CurrentBranch);
            var currentBranchCommitId = await File.ReadAllTextAsync(Path.Join(BranchesDirectoryPath, currentBranchName));

            var givenBranchCommitId = await File.ReadAllTextAsync(givenBranchPath);

            await LeastCommonAncestor(currentBranchCommitId, givenBranchCommitId);
        }

        private static async Task LeastCommonAncestor(string commitId1, string commitId2)
        {
            // System.Console.WriteLine(commitId1);
            // System.Console.WriteLine(commitId2);

            var rootCommitId = await File.ReadAllTextAsync(RootCommit);

            var currCommitId = commitId1;
            var currCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{currCommitId}");
            var commitId1ToRootPath = new Dictionary<string, string>();
            while (true)
            {
                commitId1ToRootPath[currCommitId] = currCommit.Parent;
                if (string.IsNullOrEmpty(currCommit.Parent))
                {
                    break;
                }
                
                currCommitId = currCommit.Parent;;
                currCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{currCommitId}");
            }

            foreach (var (key, value) in commitId1ToRootPath)
            {
                System.Console.WriteLine($"{key} --- {value}");
            }


            currCommitId = commitId2;
            currCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{currCommitId}");
            var commitId2ToRootPath = new Dictionary<string, string>();
            while (true)
            {
                if (commitId1ToRootPath.ContainsKey(currCommitId))
                {
                    System.Console.WriteLine($"LCA is {currCommitId}");
                    break;
                }
                // commitId1ToRootPath[currCommitId] = currCommit.Parent;
                // if (string.IsNullOrEmpty(currCommit.Parent))
                // {
                //     break;
                // }
                
                currCommitId = currCommit.Parent;;
                currCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{currCommitId}");
            }
        }

    }
}