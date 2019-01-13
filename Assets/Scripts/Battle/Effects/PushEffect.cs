using Assets.Scripts.Animation;
using Assets.Scripts.Battle;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine.Assertions;

namespace Assets.Scripts.Symbols
{
    public class PushEffect : IEffect
    {
        public UnitModel RetriveTarget(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            var target = vision.GetUnitAt(new MyHexPosition(1, 0));
            Assert.IsNotNull(target,"There is no target");
            return target;
        }

        public bool IsActivated(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            var unitInFront = vision.GetUnitAt(new MyHexPosition(1, 0));
            if (unitInFront != null && unitInFront.Owner != vision.PossesedPawn.Owner)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Execute(BattlefieldVision vision, MyHexPosition activatingPosition, BattleEngagementResult reciever)
        {
            var unitInFront = vision.GetUnitAt(new MyHexPosition(1, 0));
            Assert.IsTrue(unitInFront != null && unitInFront.Owner != vision.PossesedPawn.Owner,"There is no enemy unit in front of me");
            reciever.DisplaceUnit(unitInFront, vision.ToGlobalPosition(new MyHexPosition(2,0)));
        }

        public bool IsDefendableEffect => true;
        public IAnimation UsageAnimation => new EmptyAnimation();
    }
}