using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestProject
{
    public class UIManager : MonoBehaviour
    {
        public enum State 
        {
            Start,
            Pause,
            Game,
            GameOver
        };

        public GameObject startPanel;
        public GameObject pausePanel;
        public GameObject gamePanel;
        public GameObject gameOverPanel;

        private State m_state = State.GameOver;
        // Update is called once per frame

        private void Start()
        {

            SetState(State.Start);
        }

        private void OnGUI()
        {
            //set timescale
            Time.timeScale = (m_state == State.Game) ? 1 : 0;
        }

        public void SetState(State state)
        {
            if (state != m_state)
            {
                m_state = state;
                startPanel.SetActive(m_state == State.Start);
                pausePanel.SetActive(m_state == State.Pause);
                gamePanel.SetActive(m_state == State.Game);
                gameOverPanel.SetActive(m_state == State.GameOver);
            }
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                SetState(State.Pause);
            }
        }

        public void StartGame()
        {
            SetState(State.Game);
        }

        public void PauseGame()
        {
            SetState(State.Pause);
        }
    }
}
