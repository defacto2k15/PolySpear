using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScreenView : MonoBehaviour {

    public void ShowScreen(MyPlayer winningPlayer)
    {
        gameObject.SetActive(true);
        string textToShow;
        if (winningPlayer == MyPlayer.Player1)
        {
            textToShow = "Wygrały elfy";
        }
        else
        {
            textToShow = "Wygrały orki";
        }
        GetComponentInChildren<Text>().text = $"Koniec gry \n{textToShow}";
    }
}
