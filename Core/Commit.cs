namespace MiniatureGit.Core
{
    public class Commit
    {
        public string CommitMessage { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Parent { get; set; }

        public Dictionary<string, string> Files { get; set; }
        
        public Commit()
        {
            CommitMessage = "Initial Commit";
            Parent = string.Empty;
            Files = new Dictionary<string, string>();
        }
    }
}