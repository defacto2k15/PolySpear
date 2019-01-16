using System;
using System.Collections.Generic;
using Assets.Scripts.Animation;
using Assets.Scripts.Battle;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Symbols
{
    public class StrikeEffect : IEffect
    {
        public UnitModel RetriveTarget(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            var target = vision.GetUnitAt(new MyHexPosition(1, 0));
            Assert.IsNotNull(target, "There is no target");
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
            Assert.IsTrue(unitInFront != null && unitInFront.Owner != vision.PossesedPawn.Owner, "There is no enemy unit in front of me");
            reciever.AddStruckUnit(unitInFront);
        }

        public bool IsDefendableEffect => true;

        public Func<GameCourseModel, PawnModelComponent, PawnModelComponent, IAnimation> UsageAnimationGenerator => (model, pawn1, pawn2)
            =>
        {
            //TODO VERY BIG UGLY

            var unit = pawn1 as UnitModelComponent;
            Assert.IsNotNull(unit);
            var startPos = pawn1.transform.localPosition;
            return new SequenceAnimation(new List<IAnimation>()
            {
                new UnitStrikeAnimation(unit,pawn1.PawnModel.Position.GetPosition(),
                    Vector3.Lerp(pawn1.PawnModel.Position.GetPosition(), pawn2.PawnModel.Position.GetPosition(), 0.2f), true),
                new UnitStrikeAnimation(unit, Vector3.Lerp(pawn2.PawnModel.Position.GetPosition(),
                    pawn1.PawnModel.Position.GetPosition(), 0.8f), startPos, false)
            });
        };
    }
}