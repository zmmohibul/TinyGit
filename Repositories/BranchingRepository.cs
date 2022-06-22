using MiniatureGit.Core;
using MiniatureGit.Utils;

namespace MiniatureGit.Repositories
{
    public class BranchingRepository : CommonRepository
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
            await CommonRepository.ExitIfThereAreUntrackedChanges();

            var givenBranchPath = Path.Join(BranchesDirectoryPath, givenBranchName);
            if (!File.Exists(givenBranchPath))
            {
                LogError.Log($"No branch with the name {givenBranchName} exists.");
            }

            var currentBranchName = await File.ReadAllTextAsync(CurrentBranch);
            var currentBranchCommitId = await File.ReadAllTextAsync(Path.Join(BranchesDirectoryPath, currentBranchName));
            var currentBranchHeadCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{currentBranchCommitId}");

            var givenBranchCommitId = await File.ReadAllTextAsync(givenBranchPath);
            var givenBranchHeadCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{givenBranchCommitId}");

            var lcaCommit = await LeastCommonAncestor(currentBranchCommitId, givenBranchCommitId);

            // var mergedCommit = new Commit

            // MERGE CONDITION CHECKS
            // 1. Any files that have been modified in the given branch since the split point, but not modified in the current branch since the split point should be changed to their versions in the given branch
            foreach (var (file, fileSha) in givenBranchHeadCommit.Files)
            {
                if (lcaCommit.Files.ContainsKey(file) && currentBranchHeadCommit.Files.ContainsKey(file))
                {
                    var fileShaInLca = lcaCommit.Files[file];
                    var fileShaInCurrentBranch = currentBranchHeadCommit.Files[file];
                    if (!fileSha.Equals(fileShaInLca) && fileShaInCurrentBranch.Equals(fileShaInLca))
                    {
                        await StagingRepository.AddFileToStagingArea(file);
                    }
                }
            }

            // 2. Any files that have been modified in the current branch but not in the given branch since the split point should stay as they are
            foreach (var (file, fileSha) in currentBranchHeadCommit.Files)
            {
                if (lcaCommit.Files.ContainsKey(file) && givenBranchHeadCommit.Files.ContainsKey(file))
                {
                    var fileShaInLca = lcaCommit.Files[file];
                    var fileShaInGivenBranch = givenBranchHeadCommit.Files[file];
                    if (!fileSha.Equals(fileShaInLca) && fileShaInGivenBranch.Equals(fileShaInLca))
                    {
                        await StagingRepository.AddFileToStagingArea(file);
                    }
                }
            }

            // 3. Any files that have been modified in both the current and given branch in the same way are left unchanged by the merge
            foreach (var (file, fileSha) in givenBranchHeadCommit.Files)
            {

            }

        }

        private static async Task<Commit> LeastCommonAncestor(string commitId1, string commitId2)
        {
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


            currCommitId = commitId2;
            currCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{currCommitId}");
            var commitId2ToRootPath = new Dictionary<string, string>();
            var lcaId = string.Empty;
            while (true)
            {
                if (commitId1ToRootPath.ContainsKey(currCommitId))
                {
                    lcaId = currCommitId;
                    break;
                }
                
                currCommitId = currCommit.Parent;;
                currCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{currCommitId}");
            }

            var lcaCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{lcaId}");
            return lcaCommit;
        }

    }
}