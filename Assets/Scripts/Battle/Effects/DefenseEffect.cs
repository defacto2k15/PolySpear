using Assets.Scripts.Animation;
using Assets.Scripts.Battle;
using Assets.Scripts.Game;
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

        public void Execute(BattlefieldVision vision, MyHexPosition activatingPosition, BattleEngagementResult reciever)
        {
            Assert.IsTrue(vision.PossesedPawn is UnitModel);   // TODO !!! UGLY !!
                if (reciever.UnitWasStruck((UnitModel) vision.PossesedPawn))
                {
                    reciever.UnStrike((UnitModel) vision.PossesedPawn);
                }
        }

        public bool IsDefendableEffect => false;
        public IAnimation UsageAnimation => new EmptyAnimation();
    }
}