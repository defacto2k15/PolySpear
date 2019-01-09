using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ScenarioTesting;
using Assets.Scripts.Units;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.ScenarioTesting
{
    public class ScenarioOneTestExecutor 
    {
        private ScenarioTestingUIController _controller;
        private Scenario _scenario;
        private readonly LateStart _lateStart = new LateStart();
        private List<UnitModel> _createdUnits = new List<UnitModel>();
        private int _movementIndex = 0;
        private bool _executionFinished = false;
        private float _testStartTime;

        private const float MinimumTestTime = 1;

        public ScenarioOneTestExecutor(ScenarioTestingUIController controller, Scenario scenario)
        {
            _controller = controller;
            _scenario = scenario;
        }

        public void UpdateScenario()
        {
            if (_lateStart.ShouldRunStart)
            {
                _testStartTime = Time.time;
                _controller.MyStart();
                foreach (var startState in _scenario.StartStates)
                {
                    var newUnit = _controller.CreateUnit(startState.UnitPrefab, startState.State.Position,
                        startState.State.Orientation, startState.OwningPlayer);
                    _createdUnits.Add(newUnit);
                }
                _controller.FinalizeStart();
            }
            else
            {
                _controller.MyUpdate();
                if (_controller.IsDurningLocomotion)
                {
                    return;
                }
                else if (_movementIndex != _scenario.Movements.Count)
                {
                    var movement = _scenario.Movements[_movementIndex];

                    _controller.Move(_createdUnits[movement.UnitIndex], movement.NewPosition);
                    _movementIndex++;
                }
                else if (_testStartTime + MinimumTestTime > Time.time)
                {
                    return;
                }
                else
                {
                    int i = 0;
                    foreach (var prediction in _scenario.Predictions)
                    {
                        if (prediction.Type == ScenarioPredictionType.UnitIsDestroyed)
                        {
                            if (_createdUnits[prediction.UnitIndex].IsUnitAlive)
                            {
                                throw new FailedScenarioPredictionException(i, prediction.Type, prediction.Description, "unit was alive");
                            }
                        }
                        else if (prediction.Type == ScenarioPredictionType.UnitIsInState)
                        {
                            var unit = _createdUnits[prediction.UnitIndex];
                            var currentState = new ScenarioUnitState()
                            {
                                Orientation = unit.Orientation,
                                Position = unit.Position
                            };

                            if (!unit.IsUnitAlive)
                            {
                                throw new FailedScenarioPredictionException(i, prediction.Type, prediction.Description, $"unit was dead");
                            }


                            if (!currentState.Equals(prediction.PredictedState))
                            {
                                throw new FailedScenarioPredictionException(i, prediction.Type, prediction.Description, $"unit was at state {currentState}");
                            }
                        }else if (prediction.Type == ScenarioPredictionType.PositionIsMovable)
                        {
                            if (!_controller.IsPositionMovable(_createdUnits[prediction.UnitIndex], prediction.SelectorPosition))
                            {
                                throw new FailedScenarioPredictionException(i, prediction.Type, prediction.Description, $"position was not movable");
                            }
                        }else if (prediction.Type == ScenarioPredictionType.PositionIsNotMovable)
                        {
                            if (_controller.IsPositionMovable(_createdUnits[prediction.UnitIndex], prediction.SelectorPosition))
                            {
                                throw new FailedScenarioPredictionException(i, prediction.Type, prediction.Description, $"position was movable");
                            }
                        }
                        i++;
                    }
                    _executionFinished = true;
                }

            }
        }

        public void CleanUpState()
        {
            _controller.Reset();
            foreach (var unit in _createdUnits)
            {
                if (unit.IsUnitAlive)
                {
                    unit.SetUnitKilled();
                }
            }
        }

        public bool ExecutionFinished => _executionFinished;
    }

    public class FailedScenarioPredictionException : Exception
    {
        public FailedScenarioPredictionException(int predictionIndex, ScenarioPredictionType predictionType, string predictionDescription, string failureMessage)
            :base($"FailedScenario{predictionIndex} Type:{predictionType}| "+predictionDescription+" but "+failureMessage)
        {
        }
    }
}
