using MiniatureGit.Repositories;
using MiniatureGit.Utils;

namespace MiniatureGit 
{
    public class Program 
    {
        public static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                LogError.Log("Please enter an argument...");
            }

            var firstArguemnt = args[0].ToLower();
            
            if (firstArguemnt.Equals("init"))
            {
                InitRepository.Init();
            }
            else
            {
                LogError.Log("Invalid command argument...");
            }
        }
    }
}