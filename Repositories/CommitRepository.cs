namespace MiniatureGit.Repositories
{
    public class CommitRepository
    {
        public static async Task MakeCommit(string commitMessage)
        {
            var headCommit = await InitRepository.GetHeadCommit();
            System.Console.WriteLine(headCommit.CommitMessage);
            System.Console.WriteLine(headCommit.CreatedAt);
        }
    }
}