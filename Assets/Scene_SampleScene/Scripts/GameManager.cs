using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestProject
{
    public class GameManager : MonoBehaviour
    {
        public PlayerController player;
        public UIManager uiManager;

        private void Start()
        {
            player.onDeathAction += ShowGameOver;
        }

        public void ShowGameOver()
        {
            uiManager.SetState(UIManager.State.GameOver);
        }
    }
}
