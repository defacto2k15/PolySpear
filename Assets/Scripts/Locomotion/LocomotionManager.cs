using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Units;
using UnityEngine.Assertions;

namespace Assets.Scripts.Animation
{
    public class LocomotionManager
    {
        private UnitModel _modelBeingMoved;
        private Queue<JourneyStep> _journeySteps;

        public bool WeAreDuringLocomotion()
        {
            return _modelBeingMoved != null;
        }

        public void StartJourney(UnitModel unit, MyHexPosition target)
        {
            Assert.IsFalse(WeAreDuringLocomotion(), "There is arleady one unit during travel");
            Assert.IsFalse(unit.Position.Equals(target), "Unit is arleady at target");
            _modelBeingMoved = unit;

            // todo, now it supports movement by only one hex, should be more
            Orientation targetOrientation = Orientation.N;
            for (int i = 0; i < 6; i++)
            {
                if (target.Equals(unit.Position.Neighbors[i]))
                {
                    targetOrientation = unit.Position.NeighborDirections[i];
                }
            }

            List<Orientation> transitionalOrientations = Orientation.GetOrientationsToTarget(unit.Orientation, targetOrientation);
            _journeySteps = new Queue<JourneyStep>();
            foreach (var orientation in transitionalOrientations)
            {
                _journeySteps.Enqueue(new JourneyStep(new JourneyDirector()
                {
                    To = orientation
                }));
            }
            _journeySteps.Enqueue(new JourneyStep());
            _journeySteps.Enqueue(new JourneyStep(new JourneyMotion()
            {
                To = target
            }));
            _journeySteps.Enqueue(new JourneyStep());
        }


        public JourneyStep NextStep()
        {
            Assert.IsTrue(_journeySteps.Any(), "There are no more steps to do");
            var nextStep = _journeySteps.Dequeue();
            return nextStep;
        }

        public bool AnyMoreSteps => _journeySteps.Any();

        public UnitModel LocomotionTarget
        {
            get
            {
                Assert.IsTrue(WeAreDuringLocomotion(), "We are not during journey");
                return _modelBeingMoved;
            }
        }
    }

    public enum JourneyStepType
    {
        Motion, Director, Action
    }
        public class JourneyStep // todo maybe polymorphism??
        {
            private JourneyMotion _motion;
            private JourneyDirector _director;
            private JourneyStepType _stepType;

            public JourneyStep()
            {
                _stepType = JourneyStepType.Action;
            }

            public JourneyStep(JourneyMotion motion)
            {
                _stepType = JourneyStepType.Motion;
                _motion = motion;
            }

            public JourneyStep(JourneyDirector director)
            {
                _stepType = JourneyStepType.Director;
                _director = director;
            }

            public JourneyMotion Motion 
            {
                get
                {
                    Assert.IsNotNull(_motion);
                    return _motion;
                }   
            }

            public JourneyDirector Director
            {
                get
                {
                    Assert.IsNotNull(_director);
                    return _director;
                }
            }

            public JourneyStepType StepType => _stepType;
        }

        public class JourneyMotion
        {
            public MyHexPosition To;
        }

        public class JourneyDirector // obrot
        {
            public Orientation To;
        }
}
