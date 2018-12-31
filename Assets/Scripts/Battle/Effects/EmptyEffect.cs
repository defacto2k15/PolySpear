using Assets.Scripts.Battle;
using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public class EmptyEffect : IEffect
    {
        public void Execute(BattleResults battleResults)
        {
            //do nothing
        }

        public bool IsActivated(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            return false;
        }

        public void Execute(BattlefieldVision vision, MyHexPosition activatingPosition, BattleResults reciever)
        {
        }
    }
}