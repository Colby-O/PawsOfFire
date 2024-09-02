using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PawsOfFire.Player
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/Settings")]
    internal class PlayerSettings : ScriptableObject
    {
        public Vector2 sensitivityLimits;
        public Vector2 sensitivity;
    }
}
