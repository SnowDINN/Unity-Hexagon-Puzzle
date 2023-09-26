using UnityEngine;

namespace Anonymous.Game.Installer
{
    [CreateAssetMenu(fileName = "Installer", menuName = "Anonymous/Installer")]
    public class Installer : ScriptableObject
    {
        public int MoveCount;
        public int MaxScore;
    }
}