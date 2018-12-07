using System;

namespace Assets.Scripts.Game
{
    public class GameTurn
    {
        public static GameTurn FirstPlayerTurn = new GameTurn(() => SecondPlayerTurn, MyPlayer.Player1);
        public static GameTurn SecondPlayerTurn = new GameTurn(() => FirstPlayerTurn, MyPlayer.Player2);

        private readonly Func<GameTurn> _nextPlayerTurnSupplier;

        private GameTurn(Func<GameTurn> nextPlayerTurnSupplier, MyPlayer player)
        {
            _nextPlayerTurnSupplier = nextPlayerTurnSupplier;
            Player = player;
        }

        public GameTurn NextPlayerTurn => _nextPlayerTurnSupplier();
        public MyPlayer Player { get; }
    }
}