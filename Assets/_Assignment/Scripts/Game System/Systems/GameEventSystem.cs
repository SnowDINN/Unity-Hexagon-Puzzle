using Anonymous.Game.Hexagon;

namespace Anonymous.Game
{
    public static class GameEventSystem
    {
        public delegate void Delegate_DetectBlankSystem();

        public delegate void Delegate_MatchSystem(IHexagon hexagon);

        public delegate void Delegate_MovementSystem(int id, IHexagon hexagon);

        public static event Delegate_MovementSystem EVT_MovementSystem;
        public static event Delegate_MatchSystem EVT_MatchSystem;
        public static event Delegate_DetectBlankSystem EVT_DetectBlankSystem;

        public static void EVT_MovementPublish(this IHexagon hexagon, int id)
        {
            EVT_MovementSystem?.Invoke(id, hexagon);
        }

        public static void EVT_MatchPublish(this IHexagon hexagon)
        {
            EVT_MatchSystem?.Invoke(hexagon);
        }

        public static void EVT_DetectBlankSystemPublish(this IHexagon hexagon)
        {
            EVT_DetectBlankSystem?.Invoke();
        }
    }
}