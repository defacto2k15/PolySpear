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
        public EndGameScreenView EndGameScreenView;

        public GameObject SpearmanPrefab; 
        public GameObject Elf1Prefab; 
        public GameObject Elf2Prefab; 
        public GameObject Elf3Prefab; 
        public GameObject Orc1Prefab; 
        public GameObject Orc2Prefab; 
        public GameObject Orc3Prefab; 

        private GameCourseView _view;
        private GameCourseModel _courseModel;
        private Stack<LocomotionManager> _locomotions;
        
        // UI State
        private UnitModel _selectedUnit;

        public void Start()
        {
            _view = GetComponent<GameCourseView>();
            _locomotions = new Stack<LocomotionManager>();
            _courseModel = GetComponent<GameCourseModel>();
        }

        public void Update()
        {
            if (_courseModel.Phrase == Phrase.Play && _courseModel.IsFinished()) //todo maybe Phrase.GameEnded?
            {
                EndGameScreenView.ShowScreen(_courseModel.GetWinner());
                return;
            }

            if (_locomotions.Any())
            {
                var currentLocomotion = _locomotions.Peek();
                if (currentLocomotion.LocomotionFinished)
                {
                    _locomotions.Pop();
                    _view.MakeSelectorVisible();
                    return; //todo ??
                }
                else
                {
                    _view.MakeSelectorInvisible();
                    _view.RemoveSelectedMarker();
                    _view.RemoveMoveTargets();
                    _selectedUnit = null;

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
                            if (steps.NextStep.ShouldExecuteBattle )
                            {
                                ExecuteBattle(locomotionTarget.Position);
                            }
                        }
                    }
                }
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
                    else if (_selectedUnit != null && _courseModel.CanMoveTo(_selectedUnit, selectorPosition) ) // we have arleady selected unit and we can go when we clicked
                    {
                        // we are moving!!!
                        _locomotions.Push(LocomotionManager.CreateMovementJourney(_selectedUnit, selectorPosition));
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
