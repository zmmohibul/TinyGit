using MiniatureGit.Utils;

namespace MiniatureGit.Core
{
    public static class StagingArea
    {
        private static Dictionary<string, string> FilesStagedForAddition { get; set; } = new Dictionary<string, string>();
        
    }
}