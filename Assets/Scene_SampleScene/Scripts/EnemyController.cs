using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestProject
{
    public class EnemyController : MonoBehaviour
    {
        public PlayerController player;
        public LabyrinthGenerator labyrinth;
        public List<Enemy> enemyPrefabs;
        

        [SerializeField]
        private int m_maxEnemyCount;

        private List<Enemy> enemies = new List<Enemy>();

        public void Start()
        {
            for (int j = 0; j < enemyPrefabs.Count; ++j)
            { 
                for (int i = 0; i < m_maxEnemyCount; ++i)
                {
                    Enemy enemy = Instantiate(enemyPrefabs[j], gameObject.transform);
                    enemy.transform.position = new Vector3
                    (
                        Random.Range(-labyrinth.size.x * 0.5f, labyrinth.size.x * 0.5f) * labyrinth.scale,
                        1,
                        Random.Range(-labyrinth.size.y * 0.5f, labyrinth.size.y * 0.5f) * labyrinth.scale
                    );
                    enemies.Add(enemy);
                }
            }
        }

        public void Update()
        {
            for (int i = 0; i < enemies.Count; ++i)
            {
                if (enemies[i])
                {
                    enemies[i].UpdateEnemy(player.gameObject.transform.position, (player) ? player.canAttack : false);
                }
            }
        }
    }
}
