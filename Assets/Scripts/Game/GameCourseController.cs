using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animation;
using Assets.Scripts.Locomotion;
using Assets.Scripts.Map;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameCourseController : MonoBehaviour
    {
        public GameObject SpearmanPrefab; 
        public GameObject Elf1Prefab; 
        public GameObject Elf2Prefab; 
        public GameObject Elf3Prefab; 
        public GameObject Orc1Prefab; 
        public GameObject Orc2Prefab; 
        public GameObject Orc3Prefab; 

        private GameCourseModel _courseModel;
        private Stack<LocomotionManager> _locomotions;

        public void Start()
        {
            _locomotions = new Stack<LocomotionManager>();
            _courseModel = GetComponent<GameCourseModel>();
        }

        public GameCourseState CourseState
        {
            get
            {
                if (_courseModel.Phrase == Phrase.Placing)
                {
                    return GameCourseState.Starting;
                }
                if (_courseModel.Phrase == Phrase.Play && _courseModel.IsFinished())
                {
                    return GameCourseState.Finished;
                }
                if (_locomotions.Any())
                {
                    return GameCourseState.NonInteractive;
                }
                else
                {
                    return GameCourseState.Interactive;
                }
            }
        }

        public void MyUpdate()
        {
            if (_courseModel.Phrase == Phrase.Play && _courseModel.IsFinished()) //todo maybe Phrase.GameEnded?
            {
                return;
            }

            if (_locomotions.Any())
            {
                var currentLocomotion = _locomotions.Peek();
                if (currentLocomotion.LocomotionFinished)
                {
                    _locomotions.Pop();
                    return; //todo ??
                }
                else
                {
                    if (currentLocomotion.DuringAnimation)
                    {
                        currentLocomotion.UpdateAnimation();
                    }
                    else
                    {
                        var steps = currentLocomotion.AdvanceJourney();
                        var previousStep = steps.PreviousStep;
                        var locomotionTarget = currentLocomotion.LocomotionLocomotionTarget;
                        if (previousStep != null)
                        {
                            if (!previousStep.ShouldExecuteBattle)
                            {
                                previousStep.ApplyStepToModel(_courseModel, locomotionTarget);
                            }
                        }

                        // WE ARE FIGHTING
                        if (steps.NextStep != null)
                        {
                            if (steps.NextStep.ShouldExecuteBattle)
                            {
                                ExecuteBattle(locomotionTarget.Position);
                            }
                        }
                    }
                }
            }


            if (_courseModel.Phrase == Phrase.Placing)
            {
                if (_courseModel.Turn == GameTurn.FirstPlayerTurn)
                {
                    // todo prawdziwa faza wystawiania
                    _courseModel.AddUnit(new MyHexPosition(0, 0), MyPlayer.Player1, Orientation.N, Elf1Prefab);
                    _courseModel.AddUnit(new MyHexPosition(1, 2), MyPlayer.Player1, Orientation.N, Elf2Prefab);
                    _courseModel.AddUnit(new MyHexPosition(2, 4), MyPlayer.Player1, Orientation.N, Elf3Prefab);
                    _courseModel.NextTurn();
                }
                else
                {
                    // todo wstawianie drugiego
                    _courseModel.AddUnit(new MyHexPosition(4, 1), MyPlayer.Player2, Orientation.S, Orc1Prefab);
                    _courseModel.AddUnit(new MyHexPosition(5, 2), MyPlayer.Player2, Orientation.S, Orc2Prefab);
                    _courseModel.AddUnit(new MyHexPosition(5, 3), MyPlayer.Player2, Orientation.S, Orc3Prefab);
                    _courseModel.Phrase = Phrase.Play;
                    _courseModel.NextTurn();
                    _courseModel.NextPhrase();
                }
            }
        }

        public void MoveTo(MyHexPosition selectorPosition, UnitModel selectedUnit)
        {
            var clickedUnit = _courseModel.GetUnitAt(selectorPosition);
            // we are moving!!!
            _locomotions.Push(LocomotionManager.CreateMovementJourney(selectedUnit, selectorPosition));
            _courseModel.NextTurn();
        }


        private void ExecuteBattle(MyHexPosition battlePlace)
        {
            var battleResults = _courseModel.PerformBattleAtPlace(battlePlace);
            _locomotions = new Stack<LocomotionManager>(_locomotions.Where(c => !battleResults.UnitsIncapaciated.Contains(c.LocomotionLocomotionTarget)));

            battleResults.UnitsKilled.ForEach(c =>
            {
                _locomotions.Push(LocomotionManager.CreateDeathJourney(c));
            });

            battleResults.UnitsPushed.ForEach(c =>
            {
                _locomotions.Push(LocomotionManager.CreatePushJourney(c.UnitPushed, c.EndPosition));
            });
        }


        public bool TileIsClickable(MyHexPosition hexPosition)
        {
            return _courseModel.HasTileAt(hexPosition);
        }

        public UnitModel GetUnitAtPlace(MyHexPosition position)
        {
            return _courseModel.GetUnitAt(position);
        }

        public MyPlayer CurrentPlayer => _courseModel.Turn.Player;

        public List<MyHexPosition> GetPossibleMoveTargets(UnitModel unit)
        {
            return unit.PossibleMoveTargets.Where(c => _courseModel.CanMoveTo(unit, c)).ToList();
        }
    }

    public enum GameCourseState
    {
        Starting, Finished, Interactive, NonInteractive
    }
}
