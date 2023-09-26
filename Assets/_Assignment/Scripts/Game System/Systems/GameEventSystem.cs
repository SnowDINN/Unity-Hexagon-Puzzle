using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game
{
    public static class GameEventSystem
    {
        public delegate void Delegate_DetectBlankSystem();

        public delegate void Delegate_MatchSystem(int id);
        
        public delegate void Delegate_MatchSuccessSystem(int score);

        public delegate void Delegate_MovementSystem(IHexagon hexagon, int id);
        
        public delegate void Delegate_MovementSuccessSystem();

        public static event Delegate_MovementSystem EVT_MovementSystem;
        public static event Delegate_MovementSuccessSystem EVT_MovementSuccessSystem;
        public static event Delegate_MatchSystem EVT_MatchSystem;
        public static event Delegate_MatchSuccessSystem EVT_MatchSuccessSystem;
        public static event Delegate_DetectBlankSystem EVT_DetectBlankSystem;
        
        public static Vector2 CalculateLocalPosition(this Vector3 a, Vector2 b)
        {
            return a + new Vector3(b.x, b.y, 0);
        }

        public static void EVT_MovementPublish(this IHexagon hexagon, int id)
        {
            EVT_MovementSystem?.Invoke(hexagon, id);
        }
        
        public static void EVT_MovementSuccessPublish()
        {
            EVT_MovementSuccessSystem?.Invoke();
        }

        public static void EVT_MatchPublish(int id)
        {
            EVT_MatchSystem?.Invoke(id);
        }
        
        public static void EVT_MatchSuccessPublish(int score)
        {
            EVT_MatchSuccessSystem?.Invoke(score);
        }

        public static void EVT_DetectBlankSystemPublish()
        {
            EVT_DetectBlankSystem?.Invoke();
        }
    }
}