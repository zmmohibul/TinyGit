using MiniatureGit.Utils;

namespace MiniatureGit.Core
{
    public class StagingArea
    {
        public Dictionary<string, string> FilesStagedForAddition { get; set; }

        public StagingArea()
        {
            FilesStagedForAddition = new Dictionary<string, string>();
        }
        
    }
}