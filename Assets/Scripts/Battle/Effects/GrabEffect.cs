﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Sound;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Battle.Effects
{
    public class GrabEffect : IEffect
    {
        private const int MinDragDistance = 3;
        private const int MaxDragDistance = 4;
        private const int DistanceAfterGrab = 2;

        public UnitModel RetriveTarget(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            var grabbedUnit = RetriveGrabbedUnit(vision);
            Assert.IsNotNull(grabbedUnit,"There is no target");
            return grabbedUnit;
        }

        public bool IsActivated(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            return RetriveGrabbedUnit(vision) != null;
        }

        public void Execute(BattlefieldVision vision, MyHexPosition activatingPosition, BattleEngagementResult reciever)
        {
            var grabbedUnit = RetriveGrabbedUnit(vision);
            Assert.IsNotNull(grabbedUnit,"There is no target");
            reciever.DisplaceUnit(grabbedUnit, vision.ToGlobalPosition(new MyHexPosition(DistanceAfterGrab,0)));
        }

        public bool IsDefendableEffect => true;

        private UnitModel RetriveGrabbedUnit(BattlefieldVision vision)
        {
            for (int i = MinDragDistance; i <= MaxDragDistance; i++)
            {
                var pos = new MyHexPosition(i, 0);
                if (!vision.HasTileAt(pos))
                {
                    return null;
                }
                var otherUnitAtFront = vision.GetUnitAt(pos);
                if (otherUnitAtFront != null)
                {
                    if (otherUnitAtFront.Owner != vision.PossesedPawn.Owner)
                    {
                        return otherUnitAtFront;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
            
        }

        public Func<GameCourseModel, MasterSound, PawnModelComponent, PawnModelComponent, IAnimation> UsageAnimationGenerator => (a,a1,b,c) => new EmptyAnimation();
    }
}
