using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine.Assertions;

namespace Assets.Scripts.Locomotion
{
    public class LocomotionManager<T> where T : PawnModelComponent
    {
        private T _locomotionTarget;
        private Queue<IJourneyStep<T>> _journeySteps;
        private IJourneyStep<T> _currentStep;
        private MyAnimation _currentAnimation;

        public LocomotionManager(T locomotionTarget, Queue<IJourneyStep<T>> journeySteps)
        {
            _locomotionTarget = locomotionTarget;
            _journeySteps = journeySteps;
        }

        public JourneyStepPairs<T> AdvanceJourney(GameCourseModel model)
        {
            Assert.IsTrue(_journeySteps.Any() || _currentStep != null, "There are no more steps to do");
            IJourneyStep<T> nextStep = null;
            if (_journeySteps.Any())
            {
                nextStep = _journeySteps.Dequeue();
            }
            var steps = new JourneyStepPairs<T>()
            {
                PreviousStep = _currentStep,
                NextStep = nextStep
            };
            _currentStep = nextStep;
            if (_currentStep != null)
            {
                _currentAnimation = _currentStep.CreateAnimation(model, _locomotionTarget);
                _currentAnimation.StartAnimation();
            }
            return steps;
        }

        public bool LocomotionFinished => !_journeySteps.Any() &&  _currentStep == null ;

        public T LocomotionTarget => _locomotionTarget;

        public bool DuringAnimation => _currentAnimation != null && _currentAnimation.WeAreDuringAnimation();

        public void UpdateAnimation()
        {
            Assert.IsTrue(_currentAnimation.WeAreDuringAnimation(), "We should be during animation, but it is finished");
            _currentAnimation.UpdateAnimation();
        }
    }

    public static class LocomotionUtils
    {
        public static LocomotionManager<UnitModelComponent> CreateMovementJourney(UnitModelComponent unit, MyHexPosition target) 
        {
            Assert.IsFalse(unit.Model.Position.Equals(target), "Unit is arleady at target");

            // todo, now it supports movement by only one hex, should be more
            Orientation targetOrientation = Orientation.N;
            for (int i = 0; i < 6; i++)
            {
                if (target.Equals(unit.Model.Position.Neighbors[i]))
                {
                    targetOrientation = unit.Model.Position.NeighborDirections[i];
                }
            }

            List<Orientation> transitionalOrientations = OrientationUtils.GetOrientationsToTarget(unit.Model.Orientation, targetOrientation);
            var journeySteps = new Queue<IJourneyStep<UnitModelComponent>>();
            foreach (var orientation in transitionalOrientations)
            {
                journeySteps.Enqueue(new JourneyDirector()
                {
                    To = orientation
                });
            }
            journeySteps.Enqueue(new JourneyBattle());
            journeySteps.Enqueue(new JourneyMotion()
            {
                To = target
            });
            journeySteps.Enqueue(new JourneyBattle());
            return new LocomotionManager<UnitModelComponent>(unit, journeySteps);
        }

        public static LocomotionManager<UnitModelComponent> CreatePushJourney(UnitModelComponent unit, MyHexPosition target)
        {
            var journeySteps = new Queue<IJourneyStep<UnitModelComponent>>();
            journeySteps.Enqueue(new JourneyDisplacement()
            {
                To = target
            });
            journeySteps.Enqueue(new JourneyPassiveOnlyBattle());
            return new LocomotionManager<UnitModelComponent>(unit, journeySteps);
        }


        public static LocomotionManager<UnitModelComponent> CreateDeathJourney(UnitModelComponent unit)
        {
            var journeySteps = new Queue<IJourneyStep<UnitModelComponent>>();
            journeySteps.Enqueue( new JourneyDeath());
            return new LocomotionManager<UnitModelComponent>(unit, journeySteps);
        }

        public static LocomotionManager<ProjectileModelComponent> CreateProjectileJourney(ProjectileModelComponent projectile, MyHexPosition endPosition)
        {
            var startPosition = projectile.Model.Position;
            int offsetU = endPosition.U - startPosition.U;
            int offsetV = endPosition.V - startPosition.V;

            var journeySteps = new Queue<IJourneyStep<ProjectileModelComponent>>();

            Assert.IsTrue((offsetU != 0 && offsetV == 0) || (offsetU == 0 && offsetV != 0) || (offsetU == offsetV),
                $"Move is not horizontal nor diagonal: startPosition {startPosition} endPosition {endPosition}");
            var stepsCount = Math.Max(Math.Abs(offsetU), Math.Abs(offsetV));

            // horizontal movement
            for (int i = 1; i <= stepsCount; i++)
            {
                journeySteps.Enqueue(new ProjectileJourneyMotion()
                {
                    To = startPosition + new MyHexPosition( i * Math.Sign(offsetU), i * Math.Sign(offsetV))
                });
            }
            journeySteps.Enqueue(new ProjectileJourneyHit());
            return new LocomotionManager<ProjectileModelComponent>(projectile, journeySteps);
        }
    }
}
