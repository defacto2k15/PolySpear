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
        private GameCourseView _view;
        private LocomotionManager _locomotionManager;
        private GameCourseModel _courseModel;
        private MyAnimator _animator;
        
        // UI State
        private UnitModel _selectedUnit;

        public void Start()
        {
            _view = GetComponent<GameCourseView>();
            _locomotionManager = new LocomotionManager();
            _courseModel = GetComponent<GameCourseModel>();
            _animator = new MyAnimator();
        }

        public void Update()
        {
            if (_animator.WeAreDuringAnimation())
            {
                _view.MakeSelectorInvisible(); //todo: these lines vvv are repeated many times. This needs to change
                _view.RemoveSelectedMarker();
                _view.RemoveMoveTargets();
                _selectedUnit = null;
                _animator.UpdateAnimation();
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
                    _courseModel.AddUnit(new MyHexPosition(1, 1), MyPlayer.Player1, Orientation.N);
                    _courseModel.NextTurn(); 
                }
                else
                {
                    // todo wstawianie drugiego
                    _courseModel.AddUnit(new MyHexPosition(3, 3), MyPlayer.Player2, Orientation.S);
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

                    if (clickedUnit != null && clickedUnit.Owner == _courseModel.Turn.Player)
                    {
                        _selectedUnit = clickedUnit;
                        _view.SetSelectedMarker(_selectedUnit.Position);
                        var possibleMoveTargets = clickedUnit.PossibleMoveTargets.Where(c => _courseModel.IsTileMovable(c)).ToList();
                        _view.SetMoveTargets(possibleMoveTargets);
                    }
                    else if (clickedUnit == null &&
                             _selectedUnit != null &&
                             _selectedUnit.PossibleMoveTargets.Where(c => _courseModel.IsTileMovable(c)).Contains(selectorPosition))
                    {
                        // we are moving!!!
                        _locomotionManager.StartJourney(_selectedUnit, selectorPosition);
                    }
                    else { 
                        _view.RemoveSelectedMarker();
                        _view.RemoveMoveTargets();
                        _selectedUnit = null;
                    }

                }
            }
        }

        private void HandleLocomotion(JourneyStep step, UnitModel locomotionTarget)
        {
            if (step.StepType == JourneyStepType.Action)
            {
                _courseModel.PerformBattle(locomotionTarget.Position);
                if (!_courseModel.HasUnitAt(locomotionTarget.Position))
                {
                    //we died in battle
                    _animator = new MyAnimator();
                    _locomotionManager = new LocomotionManager();
                }
            }
            else
            {
                if (step.StepType == JourneyStepType.Director)
                {
                    _animator.StartRotationAnimation(locomotionTarget, step.Director.To, () =>
                    {
                        _courseModel.OrientUnit(locomotionTarget, step.Director.To);
                    });
                }else if (step.StepType == JourneyStepType.Motion)
                {
                    _animator.StartMotionAnimation(locomotionTarget, step.Motion.To, () =>
                    {
                        _courseModel.MoveUnit(locomotionTarget, step.Motion.To);
                    });
                }
            }
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
