namespace Infrastructure.EventsBus.Signals
{
    public struct FinishedBuild
    {
        private int _prize;
        private int _reward;

        public int Prize => _prize;
        public int Reward => _reward;

        public FinishedBuild(int prize, int reward)
        {
            _prize = prize;
            _reward = reward;
        }
    }
}