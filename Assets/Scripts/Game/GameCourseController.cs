using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animation;
using Assets.Scripts.Map;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameCourseController : MonoBehaviour
    {
        public EndGameScreenView EndGameScreenView;

        public GameObject SpearmanPrefab; 
        public GameObject Elf1Prefab; 
        public GameObject Elf2Prefab; 
        public GameObject Elf3Prefab; 
        public GameObject Orc1Prefab; 
        public GameObject Orc2Prefab; 
        public GameObject Orc3Prefab; 

        private GameCourseView _view;
        private LocomotionManager _locomotionManager;
        private GameCourseModel _courseModel;
        private Stack<MyAnimator> _animations = new Stack<MyAnimator>();
        
        // UI State
        private UnitModel _selectedUnit;

        public void Start()
        {
            _view = GetComponent<GameCourseView>();
            _locomotionManager = new LocomotionManager();
            _courseModel = GetComponent<GameCourseModel>();
        }

        public void Update()
        {
            if (_animations.Any() && _animations.Peek().WeAreDuringAnimation())
            {
                _view.MakeSelectorInvisible(); //todo: these lines vvv are repeated many times. This needs to change
                _view.RemoveSelectedMarker();
                _view.RemoveMoveTargets();
                _selectedUnit = null;
                _animations.Peek().UpdateAnimation();
                return;
            }
            if (_courseModel.Phrase == Phrase.Play && _courseModel.IsFinished()) //todo maybe Phrase.GameEnded?
            {
                EndGameScreenView.ShowScreen(_courseModel.GetWinner());
                return;
            }

            if (_locomotionManager.WeAreDuringLocomotion())
            {
                if (!_locomotionManager.AnyMoreSteps)
                {
                    _locomotionManager = new LocomotionManager();
                    _view.MakeSelectorVisible();
                    return;
                }
                _view.MakeSelectorInvisible();
                _view.RemoveSelectedMarker();
                _view.RemoveMoveTargets();
                _selectedUnit = null;
                HandleLocomotion(_locomotionManager.NextStep(), _locomotionManager.LocomotionTarget);
                return;
            }

            var selectorPosition = UpdateSelector();
            if (selectorPosition == null)
            {
                return;
            }

            _view.MakeSelectorVisible();
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
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var clickedUnit = _courseModel.GetUnitAt(selectorPosition);

                    if (clickedUnit != null && clickedUnit.Owner == _courseModel.Turn.Player) // we clicked our own unit
                    {
                        _selectedUnit = clickedUnit;
                        _view.SetSelectedMarker(_selectedUnit.Position);
                        var possibleMoveTargets = clickedUnit.PossibleMoveTargets.Where(c => _courseModel.CanMoveTo(clickedUnit, c)).ToList();
                        _view.SetMoveTargets(possibleMoveTargets);
                    }
                    else if (_selectedUnit != null && _courseModel.CanMoveTo(_selectedUnit, selectorPosition)) // we have arleady selected unit and we can go when we clicked
                    {
                            // we are moving!!!
                            _locomotionManager.StartJourney(_selectedUnit, selectorPosition);
                            _courseModel.NextTurn();
                    }
                    else
                    {
                        _view.RemoveSelectedMarker();
                        _view.RemoveMoveTargets();
                        _selectedUnit = null;
                    }

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
                var newAnimator = new MyAnimator();
                if (step.StepType == JourneyStepType.Director)
                {
                    newAnimator.StartRotationAnimation(locomotionTarget, step.Director.To, () =>
                    {
                        _courseModel.OrientUnit(locomotionTarget, step.Director.To);
                        _animations.Pop();
                    });
                }else if (step.StepType == JourneyStepType.Motion)
                {
                    newAnimator.StartMotionAnimation(locomotionTarget, step.Motion.To, () =>
                    {
                        _courseModel.MoveUnit(locomotionTarget, step.Motion.To);
                        _animations.Pop();
                    });
                }
                _animations.Push(newAnimator);
            }
        }

        private void ExecuteBattle(MyHexPosition battlePlace)
        {
            var battleResults = _courseModel.PerformBattleAtPlace(battlePlace);
            _animations = new Stack<MyAnimator>(_animations.Where(c => !battleResults.UnitsIncapaciated.Contains(c.AnimationTarget)));

            if (battleResults.UnitsIncapaciated.Contains(_locomotionManager.LocomotionTarget))
            {
                _locomotionManager = new LocomotionManager();
            }
            battleResults.UnitsKilled.ForEach(c =>
            {
                var newAnimator = new MyAnimator();
                newAnimator.StartDeathAnimation(c, () =>
                {
                    _courseModel.FinalizeKillUnit(c);
                });
                _animations.Push(newAnimator);
            });

            battleResults.UnitsPushed.ForEach(c =>
            {
                var newAnimator = new MyAnimator();
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
                    _view.MakeSelectorVisible();
                    _view.MoveSelectorTo(hexPosition);
                    return hexPosition;
                }
                else
                {
                    _view.MakeSelectorInvisible();
                }
            }
            else
            {
                _view.MakeSelectorInvisible();
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
    }
}
