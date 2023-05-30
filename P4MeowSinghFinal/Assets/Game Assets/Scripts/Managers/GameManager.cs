

// Re-wrote and re-commented the script, reference used from tank tutorial.



using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Complete
{
    public class GameManager : MonoBehaviour
    {
        public int m_NumRoundsToWin = 5;            // The number of rounds required to win the game.
        public float m_StartDelay = 3f;             // The delay before the start of each round.
        public float m_EndDelay = 3f;               // The delay before the end of each round or the game.
        public CameraControl m_CameraControl;       // Reference to the camera control script.
        public Text m_MessageText;                  // Reference to the overlay text for displaying messages.
        public GameObject m_TankPrefab;             // Reference to the tank prefab.
        public TankManager[] m_Tanks;               // A collection of tank managers.

        private int m_RoundNumber;                  // The current round number.
        private WaitForSeconds m_StartWait;         // Delay for the start of each round.
        private WaitForSeconds m_EndWait;           // Delay for the end of each round or the game.
        private TankManager m_RoundWinner;          // Reference to the winner of the current round.
        private TankManager m_GameWinner;           // Reference to the winner of the game.
        private bool paused;
        public GameObject pauseMenu;

        private void Start()
        {
            // Create the delays.
            m_StartWait = new WaitForSeconds (m_StartDelay);
            m_EndWait = new WaitForSeconds (m_EndDelay);

            // Spawn tanks and set camera targets.
            SpawnAllTanks();
            SetCameraTargets();

            // Start the game loop.
            StartCoroutine (GameLoop ());
        }

        void Update()
        {
            // Pause the game if the "P" key is pressed.
            if(Input.GetKeyDown(KeyCode.P))
            {
                ChangePaused();
            }
        }

        private void SpawnAllTanks()
        {
            // Instantiate tanks and set player numbers.
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].m_Instance =
                    Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
            }
        }

        private void SetCameraTargets()
        {
            // Set camera targets to tank transforms.
            Transform[] targets = new Transform[m_Tanks.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = m_Tanks[i].m_Instance.transform;
            }
            m_CameraControl.m_Targets = targets;
        }

        private IEnumerator GameLoop ()
        {
            // Start the round and wait for it to finish.
            yield return StartCoroutine (RoundStarting ());
            yield return StartCoroutine (RoundPlaying());
            yield return StartCoroutine (RoundEnding());

            // Check for a game winner.
            if (m_GameWinner != null)
            {
                // Restart the level if there is a game winner.
                SceneManager.LoadScene (0);
            }
            else
            {
                // Restart the game loop if there is no game winner.
                StartCoroutine (GameLoop ());
            }
        }

        private IEnumerator RoundStarting ()
        {
            // Reset tanks and disable their control.
            ResetAllTanks ();
            DisableTankControl ();

            // Set camera position and size.
            m_CameraControl.SetStartPositionAndSize ();

            // Increment the round number and display it.
            m_RoundNumber++;
            m_MessageText.text = "ROUND " + m_RoundNumber;

            // Wait for the specified start delay.
            yield return m_StartWait;
        }

        private IEnumerator RoundPlaying ()
        {
            // Enable tank control and clear the text.
            EnableTankControl ();
            m_MessageText.text = string.Empty;

            // Wait until only one tank is left.
            while (!OneTankLeft())
            {
                yield return null;
            }
        }

        private IEnumerator RoundEnding ()
        {
            // Disable tank control and find the round winner.
            DisableTankControl ();
            m_RoundWinner = null;
            m_RoundWinner = GetRoundWinner ();

            // Increment the round winner's score.
            if (m_RoundWinner != null)
                m_RoundWinner.m_Wins++;

            // Find the game winner and display the message.
            m_GameWinner = GetGameWinner ();
            string message = EndMessage ();
            m_MessageText.text = message;

            // Wait for the specified end delay.
            yield return m_EndWait;
        }

        private bool OneTankLeft()
        {
            // Count the number of active tanks.
            int numTanksLeft = 0;
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (m_Tanks[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }

            // Return true if there is one or fewer tanks left.
            return numTanksLeft <= 1;
        }
        
        private TankManager GetRoundWinner()
        {
            // Return the active tank as the round winner.
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (m_Tanks[i].m_Instance.activeSelf)
                    return m_Tanks[i];
            }

            // Return null if there is no active tank.
            return null;
        }

        private TankManager GetGameWinner()
        {
            // Return the tank with enough wins as the game winner.
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                    return m_Tanks[i];
            }

            // Return null if there is no game winner.
            return null;
        }

        private string EndMessage()
        {
            // Set the default message to draw.
            string message = "DRAW!";

            // Change the message if there is a round winner.
            if (m_RoundWinner != null)
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

            // Add each tank's score to the message.
            message += "\n\n\n\n";
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
            }

            // Change the entire message if there is a game winner.
            if (m_GameWinner != null)
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

            return message;
        }

        private void ResetAllTanks()
        {
            // Reset all tanks.
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].Reset();
            }
        }

        private void EnableTankControl()
        {
            // Enable tank control.
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].EnableControl();
            }
        }

        private void DisableTankControl()
        {
            // Disable tank control.
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].DisableControl();
            }
        }

        void ChangePaused()
        {
            // Toggle the pause state and adjust the time scale.
            if (!paused)
            {
                paused = true;
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                paused = false;
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
}
