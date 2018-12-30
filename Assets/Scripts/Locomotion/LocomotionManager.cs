using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine.Assertions;

namespace Assets.Scripts.Locomotion
{
    public class LocomotionManager
    {
        private UnitModel _locomotionTarget;
        private Queue<IJourneyStep> _journeySteps;
        private IJourneyStep _currentStep;
        private MyAnimation _currentAnimation;

        private LocomotionManager(UnitModel locomotionTarget, Queue<IJourneyStep> journeySteps)
        {
            _locomotionTarget = locomotionTarget;
            _journeySteps = journeySteps;
        }

        public static LocomotionManager CreateMovementJourney(UnitModel unit, MyHexPosition target) 
        {
            Assert.IsFalse(unit.Position.Equals(target), "Unit is arleady at target");

            // todo, now it supports movement by only one hex, should be more
            Orientation targetOrientation = Orientation.N;
            for (int i = 0; i < 6; i++)
            {
                if (target.Equals(unit.Position.Neighbors[i]))
                {
                    targetOrientation = unit.Position.NeighborDirections[i];
                }
            }

            List<Orientation> transitionalOrientations = OrientationUtils.GetOrientationsToTarget(unit.Orientation, targetOrientation);
            var journeySteps = new Queue<IJourneyStep>();
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
            return new LocomotionManager(unit, journeySteps);
        }

        public static LocomotionManager CreatePushJourney(UnitModel unit, MyHexPosition target)
        {
            var journeySteps = new Queue<IJourneyStep>();
            journeySteps.Enqueue(new JourneyMotion()
            {
                To = target
            });
            journeySteps.Enqueue(new JourneyBattle());
            return new LocomotionManager(unit, journeySteps);
        }

        public static LocomotionManager CreateDeathJourney(UnitModel unit)
        {
            var journeySteps = new Queue<IJourneyStep>();
            journeySteps.Enqueue( new JourneyDeath());
            return new LocomotionManager(unit, journeySteps);
        }

        public JourneyStepPairs AdvanceJourney()
        {
            Assert.IsTrue(_journeySteps.Any() || _currentStep != null, "There are no more steps to do");
            IJourneyStep nextStep = null;
            if (_journeySteps.Any())
            {
                nextStep = _journeySteps.Dequeue();
            }
            var steps = new JourneyStepPairs()
            {
                PreviousStep = _currentStep,
                NextStep = nextStep
            };
            _currentStep = nextStep;
            if (_currentStep != null)
            {
                _currentAnimation = _currentStep.CreateAnimation( _locomotionTarget);
                _currentAnimation.StartAnimation();
            }
            return steps;
        }

        public bool LocomotionFinished => !_journeySteps.Any() &&  _currentStep == null ;

        public UnitModel LocomotionLocomotionTarget => _locomotionTarget;

        public bool DuringAnimation => _currentAnimation != null && _currentAnimation.WeAreDuringAnimation();

        public void UpdateAnimation()
        {
            Assert.IsTrue(_currentAnimation.WeAreDuringAnimation(), "We should be during animation, but it is finished");
            _currentAnimation.UpdateAnimation();
        }
    }
}
