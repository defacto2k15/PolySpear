using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerGameController
{
	private int player;
	private const int PLAYERS = 2;
	//private List<Player> players = new List<Player> (2);
	private Player[] players;
	private int playersSoFar;
	private static ServerGameController singleton;

	public Player[] getPlayers ()
	{
		return players;
	}

	public static ServerGameController getSingleton ()
	{
		if (singleton == null)
			singleton = new ServerGameController ();
		return singleton;
	}

	private ServerGameController ()
	{
		player = 0;
		playersSoFar = 0;
		players = new Player[PLAYERS];
	}

	public void addPlayer (Player player)
	{
		//Player player = NetworkServer.FindLocalObject (playerID).GetComponent<Player> ();
		player.RpcSetPlayerNumber (playersSoFar);
		players [playersSoFar] = player;
		++playersSoFar;
		if (playersSoFar == PLAYERS) {
			players [0].RpcBeginTurn ();
		}
	}

	public void endTurn ()
	{
		player = (player + 1) % PLAYERS;
		//MonoBehaviour.print (player);
		players [player].RpcBeginTurn ();
	}
}
