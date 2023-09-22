namespace Anonymous.Game
{
    public class GameSystem
    {
        public delegate void Delegate_BlockResolve();

        public delegate void Delegate_BlockSpawn();

        public static GameSystem _default;
        public static GameSystem Default => _default ??= new GameSystem();
        public event Delegate_BlockSpawn EVT_BlockSpawn;
        public event Delegate_BlockSpawn EVT_BlockResolve;
    }
}