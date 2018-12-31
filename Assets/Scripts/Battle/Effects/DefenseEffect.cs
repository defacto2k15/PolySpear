using Assets.Scripts.Battle;
using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public class DefenseEffect : IEffect
    {
        public bool IsActivated(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            return true;
        }

        public void Execute(BattlefieldVision vision, MyHexPosition activatingPosition, BattleResults reciever)
        {
            if (reciever.UnitWasStruck(vision.PossesedUnit))
            {
                reciever.UnStrike(vision.PossesedUnit);
            }
        }
    }
}