namespace MiniatureGit.Core
{
    public class CurrentHeadState
    {
        public bool Detached { get; set; }

        public CurrentHeadState(bool detached)
        {
            Detached = detached;
        }
    }
}