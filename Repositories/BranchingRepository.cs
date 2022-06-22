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
            System.Console.WriteLine(lcaCommit.CommitMessage);

            var mergedCommitParents = new List<string>();
            mergedCommitParents.Add(currentBranchCommitId);
            mergedCommitParents.Add(givenBranchCommitId);
            var mergedCommit = new Commit($"Merged {currentBranchName} and {givenBranchName}", mergedCommitParents);

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
                        mergedCommit.Files[file] = fileSha;
                        await StagingRepository.AddFileToStagingAreaWithSha(file, fileSha);
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
                        mergedCommit.Files[file] = fileSha;
                        await StagingRepository.AddFileToStagingAreaWithSha(file, fileSha);
                    }
                }
            }

            // 3. Any files that have been modified in both the current and given branch in the same way are left unchanged by the merge
            foreach (var (file, fileSha) in givenBranchHeadCommit.Files)
            {
                if (currentBranchHeadCommit.Files.ContainsKey(file))
                {
                    var fileShaInCurrentBranch = currentBranchHeadCommit.Files[file];
                    if (fileSha.Equals(fileShaInCurrentBranch))
                    {
                        mergedCommit.Files[file] = fileSha;
                        await StagingRepository.AddFileToStagingAreaWithSha(file, fileSha);
                    }
                }

            }

            // 4. Any files that were not present at the split point and are present only in the current branch should remain as they are.
            foreach (var (file, fileSha) in currentBranchHeadCommit.Files)
            {
                if (!lcaCommit.Files.ContainsKey(file) && !givenBranchHeadCommit.Files.ContainsKey(file))
                {
                    mergedCommit.Files[file] = fileSha;
                    await StagingRepository.AddFileToStagingAreaWithSha(file, fileSha);
                }
            }

            // 5. Any files that were not present at the split point and are present only in the given branch should be checked out and staged
            foreach (var (file, fileSha) in givenBranchHeadCommit.Files)
            {
                if (!lcaCommit.Files.ContainsKey(file) && !currentBranchHeadCommit.Files.ContainsKey(file))
                {
                    mergedCommit.Files[file] = fileSha;
                    await StagingRepository.AddFileToStagingAreaWithSha(file, fileSha);
                }
            }

            // 6. Any files present at the split point, unmodified in the current branch, and absent in the given branch should be removed 
            foreach (var (file, fileSha) in currentBranchHeadCommit.Files)
            {
                if (lcaCommit.Files.ContainsKey(file) && !givenBranchHeadCommit.Files.ContainsKey(file))
                {
                    if (lcaCommit.Files[file].Equals(fileSha))
                    {
                        await StagingRepository.StageFileForRemoval(file);
                    }
                }
            }

            // 7. Any files present at the split point, unmodified in the given branch, and absent in the current branch should remain absent
            foreach (var (file, fileSha) in givenBranchHeadCommit.Files)
            {
                if (lcaCommit.Files.ContainsKey(file) && !currentBranchHeadCommit.Files.ContainsKey(file))
                {
                    if (lcaCommit.Files[file].Equals(fileSha))
                    {
                        await StagingRepository.StageFileForRemoval(file);
                    }
                }
            }

            // 8. Merge Coinflict
            var mergeConfilct = false;
            var mergeConfilctData = new MergeConfilctData();
            foreach (var (file, fileSha) in currentBranchHeadCommit.Files)
            {
                if (givenBranchHeadCommit.Files.ContainsKey(file) && !givenBranchHeadCommit.Files[file].Equals(fileSha))
                {
                    System.Console.WriteLine($"Merge confilct encounterd in file {file}");

                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }

                    await File.WriteAllTextAsync(file, "<<<<<<<<<<<HEAD\n");

                    var contentInCurrentBranch = await File.ReadAllTextAsync(Path.Join(FilesDirectoryPath, fileSha));
                    await File.AppendAllTextAsync(file, contentInCurrentBranch);

                    await File.AppendAllTextAsync(file, "===============================================================\n");

                    var contentInGivenBranch = await File.ReadAllTextAsync(Path.Join(FilesDirectoryPath, givenBranchHeadCommit.Files[file]));
                    await File.AppendAllTextAsync(file, contentInGivenBranch);

                    mergeConfilctData.FilesInConflict.Add(file);
                    mergeConfilct = true;
                }
            }

            if (mergeConfilct)
            {
                await FileSystemUtils.WriteObjectAsync<Commit>(mergedCommit, "UnmergedCommit", MiniatureGitDirName);
                await FileSystemUtils.WriteObjectAsync<MergeConfilctData>(mergeConfilctData, "MergeConflictData", MiniatureGitDirName);
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

                commitId1ToRootPath[currCommitId] = currCommit.Parents[0];
                if (string.IsNullOrEmpty(currCommit.Parents[0]))
                {
                    break;
                }
                
                currCommitId = currCommit.Parents[0];
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
                
                currCommitId = currCommit.Parents[0];
                currCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{currCommitId}");
            }

            var lcaCommit = await FileSystemUtils.ReadObjectAsync<Commit>($"{CommitsDirectoryPath}/{lcaId}");
            return lcaCommit;
        }

    }
}