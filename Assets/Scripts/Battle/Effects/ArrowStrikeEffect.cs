using System;
using Assets.Scripts.Animation;
using Assets.Scripts.Battle;
using Assets.Scripts.Game;
using Assets.Scripts.Sound;
using Assets.Scripts.Units;
using UnityEngine.Assertions;

namespace Assets.Scripts.Symbols
{
    public class ArrowStrikeEffect : IEffect
    {
        public UnitModel RetriveTarget(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            Assert.IsFalse(true, "Not implemented, should not be called"); //todo, ugly
            return null;
        }

        public bool IsActivated(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            Assert.IsFalse(true, "Not implemented, should not be called"); //todo, ugly
            return false;
        }

        public void Execute(BattlefieldVision vision, MyHexPosition activatingPosition, BattleEngagementResult reciever)
        {
            var unitInFront = vision.GetUnitAt(new MyHexPosition(0, 0));
            Assert.IsTrue(unitInFront != null,"There is no enemy unit in front of me");
            reciever.AddStruckUnit(unitInFront);
        }

        public bool IsDefendableEffect => true;
        public Func<GameCourseModel, MasterSound, PawnModelComponent, PawnModelComponent, IAnimation> UsageAnimationGenerator => (a,a1,b,c) => new EmptyAnimation();
    }
}