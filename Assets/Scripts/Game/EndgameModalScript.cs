using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game
{
    public class EndgameModalScript : MonoBehaviour
    {
        public GameObject Player1Label;
        public GameObject Player2Label;

        public void SetWinningPlayer(MyPlayer player)
        {
            if (player == MyPlayer.Player1)
            {
                Player2Label.SetActive(false);
            }
            else
            {
                Player1Label.SetActive(false);
            }
        }

        public void ResetScene()
        {
            var thisSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(thisSceneName);
        }
    }
}
