using System;
using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Sound;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Battle.Effects
{
    public class ProjectileShootingEffect : IEffect
    {
        private int _maxShootingDistance;
        private ProjectileType _projectileType;
        private EffectActivationCircumstances _activationCircumstances;

        protected ProjectileShootingEffect(int maxShootingDistance, ProjectileType projectileType, EffectActivationCircumstances activationCircumstances)
        {
            _maxShootingDistance = maxShootingDistance;
            _projectileType = projectileType;
            _activationCircumstances = activationCircumstances;
        }

        public UnitModel RetriveTarget(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            var grabbedUnit = ShootingTarget(vision);
            Assert.IsNotNull(grabbedUnit,"There is no target");
            return grabbedUnit;
        }

        public bool IsActivated(BattlefieldVision vision, MyHexPosition activatingPosition)
        {
            if (!_activationCircumstances.IsActivated(vision.BattleCircumstances))
            {
                return false;
            }
            var toReturn = ShootingTarget(vision) != null;
            return toReturn;
        }

        public void Execute(BattlefieldVision vision, MyHexPosition activatingPosition, BattleEngagementResult reciever)
        {
            var targetUnit = ShootingTarget(vision);
            Assert.IsNotNull(targetUnit,"There is no target");
            reciever.AddProjectile(vision.PossesedPawn.Position, vision.PossesedPawn.Orientation, targetUnit.Position, _projectileType);
        }

        public bool IsDefendableEffect => false;

        private UnitModel ShootingTarget(BattlefieldVision vision)
        {
            for (int i = 1; i <= _maxShootingDistance; i++)
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

        public Func<GameCourseModel, MasterSound, PawnModelComponent, PawnModelComponent, IAnimation> UsageAnimationGenerator => (model,sound, pawn1, pawn2)  => new EmptyAnimation();
    }
}