﻿using System;
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
        private ScenarioTestingGameCourseController _controller;
        private Scenario _scenario;
        private readonly LateStart _lateStart = new LateStart();
        private List<UnitModel> _createdUnits = new List<UnitModel>();
        private int _movementIndex = 0;
        private bool _executionFinished = false;

        public ScenarioOneTestExecutor(ScenarioTestingGameCourseController controller, Scenario scenario)
        {
            _controller = controller;
            _scenario = scenario;
        }

        public void UpdateScenario()
        {
            if (_lateStart.ShouldRunStart)
            {
                _controller.CustomStart();
                foreach (var startState in _scenario.StartStates)
                {
                    var newUnit = _controller.CreateUnit(startState.UnitPrefab, startState.State.Position,
                        startState.State.Orientation, startState.OwningPlayer);
                    _createdUnits.Add(newUnit);
                }
            }
            else
            {
                _controller.CustomUpdate();
                if (_controller.IsDurningLocomotion || _controller.IsDurningAnimation)
                {
                    return;
                }
                else if (_movementIndex != _scenario.Movements.Count)
                {
                    var movement = _scenario.Movements[_movementIndex];

                    _controller.Move(_createdUnits[movement.UnitIndex], movement.NewPosition);
                    _movementIndex++;
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

                            if (!currentState.Equals(prediction.PredictedState))
                            {
                                throw new FailedScenarioPredictionException(i, prediction.Type, prediction.Description, $"unit was at state {currentState}");
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