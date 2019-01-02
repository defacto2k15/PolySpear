using Assets.Scripts.Battle;
using Assets.Scripts.Units;
using UnityEngine.Assertions;

namespace Assets.Scripts.Symbols
{
    public class DefenseEffect : IEffect
    {
        public UnitModel RetriveTarget(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            Assert.IsTrue(false, "Defense os only reactive event, no target here");
            return null;
        }

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

        public bool IsDefendableEffect => false;
    }
}