using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestProject
{
    [CreateAssetMenu(fileName = "Player Data", menuName = "New Player Data", order = 64)]
    public class PlayerData : ScriptableObject
    {
        [Min(1)]
        public int hp;
        [Min(0)]
        public int damage;
        [Min(0.1f)]
        public float speed;
    }
}
