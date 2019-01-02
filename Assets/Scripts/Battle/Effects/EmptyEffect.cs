using Assets.Scripts.Battle;
using Assets.Scripts.Units;
using UnityEngine.Assertions;

namespace Assets.Scripts.Symbols
{
    public class EmptyEffect : IEffect
    {
        public UnitModel RetriveTarget(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            Assert.IsTrue(false, "Empty effect should not be called, it is never activated");
            return null;
        }

        public bool IsActivated(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            return false;
        }

        public void Execute(BattlefieldVision vision, MyHexPosition activatingPosition, BattleResults reciever)
        {
        }

        public bool IsDefendableEffect => false;
    }
}