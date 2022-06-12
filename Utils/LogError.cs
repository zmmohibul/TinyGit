namespace MiniatureGit.Utils
{
    public class LogError
    {
        public static void Log(params string[] messages)
        {
            foreach (var message in messages)
            {
                System.Console.WriteLine(message);
            }

            Environment.Exit(1);
        }
    }
}