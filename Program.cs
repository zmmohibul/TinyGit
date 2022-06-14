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
            else
            {
                LogError.Log("Invalid command argument...");
            }
        }
    }
}