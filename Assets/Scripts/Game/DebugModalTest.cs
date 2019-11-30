using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class DebugModalTest : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                FindObjectOfType<EndgameModalScript>().SetWinningPlayer(MyPlayer.Player1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                FindObjectOfType<EndgameModalScript>().SetWinningPlayer(MyPlayer.Player2);
            }
        }
    }
}
