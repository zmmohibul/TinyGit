using MiniatureGit.Repositories;
using MiniatureGit.Utils;

namespace MiniatureGit 
{
    public class Program 
    {
        public static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                LogError.Log("Please enter an argument...");
            }

            var firstArguemnt = args[0].ToLower();
            
            if (firstArguemnt.Equals("init"))
            {
                await InitRepository.Init();
            }
            else if (!InitRepository.IsGitRepo())
            {
                LogError.Log("This is not an initialized git repository");
            }
            else if (firstArguemnt.Equals("merge"))
            {
                if (args.Length < 2)
                {
                    LogError.Log("Please enter a branch name");
                }
                
                await BranchingRepository.MergeBranch(args[1]);
            }
            else if (File.Exists("./.minigit/UnmergedCommit"))
            {
                LogError.Log("Please resolve merge conflict and merge again");
            }
            else if (firstArguemnt.Equals("commit"))
            {
                if (args.Length < 2)
                {
                    LogError.Log("Please enter a commit message");
                }

                await CommitRepository.MakeCommit(args[1]);
            }
            else if (firstArguemnt.Equals("add"))
            {
                if (args.Length < 2)
                {
                    LogError.Log("Please enter file path to stage file");
                }
                
                var fileToStage = args[1];

                if (fileToStage.Equals("."))
                {
                    await StagingRepository.StageAllFiles();
                }
                else
                {
                    await StagingRepository.StageFile(fileToStage);
                }

            }
            else if (firstArguemnt.Equals("log"))
            {
                await CommitRepository.LogCommits();
            }
            else if (firstArguemnt.Equals("checkout"))
            {
                if (args.Length < 2)
                {
                    LogError.Log("Please enter a commit id");
                }

                if (args.Length > 2)
                {
                    if (!args[1].Equals("branch"))
                    {
                        LogError.Log("Invalid Command", "You can checkout a commit id or branch name");
                    }

                    await CommitRepository.CheckoutBranch(args[2]);
                    return;
                }

                await CommitRepository.Checkout(args[1]);
            }
            else if (firstArguemnt.Equals("branch"))
            {
                if (args.Length < 2)
                {
                    LogError.Log("Please enter a branch name");
                }

                await BranchingRepository.CreateNewBranch(args[1]);
            }
            else
            {
                LogError.Log("Invalid command argument...");
            }
        }
    }
}