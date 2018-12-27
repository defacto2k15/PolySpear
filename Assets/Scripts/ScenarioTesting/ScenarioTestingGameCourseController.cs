using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.ScenarioTesting
{
    public class ScenarioTestingGameCourseController : MonoBehaviour
    {
        private LocomotionManager _locomotionManager;
        private GameCourseModel _courseModel;
        private Stack<OptionalAnimator> _animations;

        private bool _shouldShowAnimations = true;
        public bool ShouldShowAnimations
        {
            set { _shouldShowAnimations = value; }
        }

        public void CustomStart()
        {
            _locomotionManager = new LocomotionManager();
            _courseModel = GetComponent<GameCourseModel>();
            _animations = new Stack<OptionalAnimator>();
        }

        public void CustomUpdate()
        {
            if (_animations.Any() ) 
            {
                if (!_animations.Peek().WeAreDuringAnimation())
                {
                    _animations.Pop();
                }
                else
                {
                    _animations.Peek().UpdateAnimation();
                }
                return;
            }
            if (_courseModel.Phrase == Phrase.Play && _courseModel.IsFinished()) //todo maybe Phrase.GameEnded?
            {
                return;
            }

            if (_locomotionManager.WeAreDuringLocomotion())
            {
                if (!_locomotionManager.AnyMoreSteps)
                {
                    _locomotionManager = new LocomotionManager();
                    return;
                }
                HandleLocomotion(_locomotionManager.NextStep(), _locomotionManager.LocomotionTarget);
                return;
            }

            var selectorPosition = UpdateSelector();
            if (selectorPosition == null)
            {
                return;
            }

            if (_courseModel.Phrase == Phrase.Placing)
            {
                if (_courseModel.Turn == GameTurn.FirstPlayerTurn)
                {
                    _courseModel.NextTurn(); 
                }
                else
                {
                    _courseModel.NextTurn();
                    _courseModel.NextPhrase();
                }
            }
        }

        private void HandleLocomotion(JourneyStep step, UnitModel locomotionTarget)
        {
            // WE ARE FIGHTING
            if (step.StepType == JourneyStepType.Action)
            {
                ExecuteBattle(locomotionTarget.Position);
            }
            else
            {
                var newAnimator = new OptionalAnimator(_shouldShowAnimations);
                if (step.StepType == JourneyStepType.Director)
                {
                    newAnimator.StartRotationAnimation(locomotionTarget, step.Director.To, () =>
                    {
                        _courseModel.OrientUnit(locomotionTarget, step.Director.To);
                    });
                }else if (step.StepType == JourneyStepType.Motion)
                {
                    newAnimator.StartMotionAnimation(locomotionTarget, step.Motion.To, () =>
                    {
                        _courseModel.MoveUnit(locomotionTarget, step.Motion.To);
                    });
                }
                _animations.Push(newAnimator);
            }
        }

        private void ExecuteBattle(MyHexPosition battlePlace)
        {
            var battleResults = _courseModel.PerformBattleAtPlace(battlePlace);
            _animations = new Stack<OptionalAnimator>(_animations.Where(c => !battleResults.UnitsIncapaciated.Contains(c.AnimationTarget)));

            if (battleResults.UnitsIncapaciated.Contains(_locomotionManager.LocomotionTarget))
            {
                _locomotionManager = new LocomotionManager();
            }
            battleResults.UnitsKilled.ForEach(c =>
            {
                var newAnimator = new OptionalAnimator(_shouldShowAnimations);
                newAnimator.StartDeathAnimation(c, () =>
                {
                    _courseModel.FinalizeKillUnit(c);
                });
                _animations.Push(newAnimator);
            });

            battleResults.UnitsPushed.ForEach(c =>
            {
                var newAnimator = new OptionalAnimator(_shouldShowAnimations);
                newAnimator.StartMotionAnimation(c.UnitPushed, c.EndPosition, () =>
                {
                    _courseModel.MoveUnit(c.UnitPushed, c.EndPosition);
                    ExecuteBattle(c.EndPosition);
                });
                _animations.Push(newAnimator);
            });
        }

        private MyHexPosition UpdateSelector()
        {
            var hexPosition = getMouseHex();
            if (hexPosition != null)
            {
                if (_courseModel.HasTileAt(hexPosition))
                {
                    return hexPosition;
                }
            }
            return null; // todo, returning null is ugly
        }

        private MyHexPosition getMouseHex()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue);
            if (hits.Length == 0)
            {
                return null;
            }
            else
            {
                float minDist = float.PositiveInfinity;
                int min = 0;
                for (int i = 0; i < hits.Length; ++i)
                {
                    if (hits[i].distance < minDist)
                    {
                        minDist = hits[i].distance;
                        min = i;
                    }
                }
                return MyHexPosition.FromWorldPosition(hits[min].point);
            }
        }

        /// Custom used to testing
        /// 
        public UnitModel CreateUnit(GameObject prefab, MyHexPosition startPosition, Orientation startOrientation, MyPlayer player)
        {
            return _courseModel.AddUnit(startPosition, player, startOrientation, prefab);
        }

        public void Reset() // such custom reseting is not optimal, but good for now
        {
            _locomotionManager = new LocomotionManager();
            _courseModel.Reset();
            _animations = new Stack<OptionalAnimator>();
        }

        public bool IsDurningLocomotion => _locomotionManager.WeAreDuringLocomotion();
        public bool IsDurningAnimation => _animations.Any();

        public void Move(UnitModel unit, MyHexPosition targetPosition)
        {
            _locomotionManager.StartJourney(unit, targetPosition);
            _courseModel.NextTurn();
        }

    }
}
