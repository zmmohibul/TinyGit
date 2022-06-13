using MiniatureGit.Repositories;
using MiniatureGit.Utils;

namespace MiniatureGit 
{
    public class Program 
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                LogError.Log("Please enter an argument...");
            }

            var firstArguemnt = args[0].ToLower();
            
            if (firstArguemnt.Equals("init"))
            {
                InitRepository.Init();
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
                StagingRepository.StageFile(fileToStage);
            }
            else
            {
                LogError.Log("Invalid command argument...");
            }
        }
    }
}