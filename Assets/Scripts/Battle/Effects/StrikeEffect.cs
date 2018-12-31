using Assets.Scripts.Battle;
using Assets.Scripts.Units;
using UnityEngine.Assertions;

namespace Assets.Scripts.Symbols
{
    public class StrikeEffect : IEffect
    {
        public bool IsActivated(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            var unitInFront = vision.GetUnitAt(new MyHexPosition(1, 0));
            if (unitInFront != null && unitInFront.Owner != vision.PossesedUnit.Owner)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Execute(BattlefieldVision vision, MyHexPosition activatingPosition, BattleResults reciever)
        {
            var unitInFront = vision.GetUnitAt(new MyHexPosition(1, 0));
            Assert.IsTrue(unitInFront != null && unitInFront.Owner != vision.PossesedUnit.Owner,"There is no enemy unit in front of me");
            reciever.AddStruckUnit(unitInFront);
        }
    }
}