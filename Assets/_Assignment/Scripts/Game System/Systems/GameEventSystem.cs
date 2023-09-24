using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game
{
    public static class GameEventSystem
    {
        public delegate void Delegate_DetectBlankSystem();

        public delegate void Delegate_MatchSystem();

        public delegate void Delegate_MovementSystem(int id, IHexagon hexagon);

        public static event Delegate_MovementSystem EVT_MovementSystem;
        public static event Delegate_MatchSystem EVT_MatchSystem;
        public static event Delegate_DetectBlankSystem EVT_DetectBlankSystem;
        
        public static Vector2 CalculateLocalPosition(this Vector3 a, Vector2 b)
        {
            return a + new Vector3(b.x, b.y, 0);
        }

        public static void EVT_MovementPublish(this IHexagon hexagon, int id)
        {
            EVT_MovementSystem?.Invoke(id, hexagon);
        }

        public static void EVT_MatchPublish()
        {
            EVT_MatchSystem?.Invoke();
        }

        public static void EVT_DetectBlankSystemPublish()
        {
            EVT_DetectBlankSystem?.Invoke();
        }
    }
}