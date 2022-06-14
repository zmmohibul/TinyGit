namespace MiniatureGit.Core
{
    public class Commit
    {
        public string CommitMessage { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Parent { get; set; }

        /*
        * A Dictionary with key as file name and value as sha of file content 
        */
        public Dictionary<string, string> Files { get; set; }
        
        public Commit()
        {
            CommitMessage = "Initial Commit";
            Parent = string.Empty;
            Files = new Dictionary<string, string>();
        }

        public Commit(string commitMessage, string parent)
        {
            CommitMessage = commitMessage;
            Parent = parent;
            Files = new Dictionary<string, string>();
        }
    }
}