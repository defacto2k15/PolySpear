using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameUIController : MonoBehaviour
    {
        public GameCourseController CourseController;
        public EndGameScreenView EndGameScreenView;

        private GameCourseView _view;
        private UnitModel _selectedUnit;
        private MyHexPosition _magicMarkerPosition;

        public void Start()
        {
            _view = GetComponent<GameCourseView>();
        }

        public void Update()
        {
            CourseController.MyUpdate();
            GameCourseState state = CourseController.CourseState;
            if (state == GameCourseState.Starting)
            {
                CourseController.PlaceUnits();
            }

            var position = UpdateSelector();
            if (Input.GetMouseButtonDown(0))
            {
                if (position == null)
                {
                    _selectedUnit = null;
                }
                else
                {
                    if (state == GameCourseState.Interactive)
                    {
                        var unitAtClickedPlace = CourseController.GetUnitAtPlace(position);
                        if (unitAtClickedPlace != null && unitAtClickedPlace.Owner == CourseController.CurrentPlayer)
                        {
                            _selectedUnit = unitAtClickedPlace;
                            _magicMarkerPosition = null;
                        }
                        else if (_selectedUnit != null && GetPossibleMovePositions(_selectedUnit).Contains(position))
                        {
                            CourseController.MoveTo(position, _selectedUnit, _magicMarkerPosition);    
                            _magicMarkerPosition = null;
                        }
                        else if (unitAtClickedPlace == null)
                        {
                            _selectedUnit = null;
                            _magicMarkerPosition = null;
                        }
                    }
                }
            }
            else
            if (Input.GetMouseButtonDown(1))
            {
                if (!CourseController.PlayerCanUseMagic())
                {
                    _magicMarkerPosition = null;
                }
                else
                {
                    if (position == null)
                    {
                        _magicMarkerPosition = null;
                    }
                    else
                    {
                        if (state == GameCourseState.Interactive)
                        {
                            if (_magicMarkerPosition != null && _magicMarkerPosition.Equals(position))
                            {
                                _magicMarkerPosition = null;
                            }
                            else
                            {
                                var unitAtClickedPlace = CourseController.GetUnitAtPlace(position);
                                if (unitAtClickedPlace == null && GetPossibleMagicPositions(_selectedUnit).Contains(position))
                                {
                                    _magicMarkerPosition = position;
                                }
                                else
                                {
                                    _magicMarkerPosition = null;
                                }
                            }
                        }
                    }
                }
            }

            if (state == GameCourseState.Interactive)
            {
                if (position != null)
                {
                    _view.MakeSelectorVisible();
                    _view.MoveSelectorTo(position);
                }
                else
                {
                    _view.MakeSelectorInvisible();
                }

                if (_selectedUnit != null)
                {
                    _view.SetSelectedMarker(_selectedUnit.Position);
                    var possibleMoveTargets = GetPossibleMovePositions(_selectedUnit);
                    _view.SetMoveTargets(possibleMoveTargets);
                }
                else
                {
                    _view.RemoveSelectedMarker();
                    _view.RemoveMoveTargets();
                }

                if (_magicMarkerPosition != null)
                {
                    _view.SetMagicMarker(_magicMarkerPosition);
                }
                else
                {
                    _view.MakeMagicMarkerInvisible();
                }
            }
            else
            {
                _view.MakeMagicMarkerInvisible();
                _view.RemoveSelectedMarker();
                _view.RemoveMoveTargets();
                _view.MakeSelectorInvisible();
                _selectedUnit = null;
            }
        }

        private List<MyHexPosition> GetPossibleMovePositions(UnitModel model)
        {
            return CourseController.GetPossibleMoveTargets(model).Where(c => _magicMarkerPosition == null || !c.Equals(_magicMarkerPosition)).ToList();
        }

        private List<MyHexPosition> GetPossibleMagicPositions(UnitModel model)
        {
            return CourseController.GetPossibleMagicTargets(model).Where(c => _magicMarkerPosition == null || !c.Equals(_magicMarkerPosition)).ToList();
        }

        private MyHexPosition UpdateSelector()
        {
            var hexPosition = getMouseHex();
            if (hexPosition != null)
            {
                if (CourseController.TileIsClickable(hexPosition))
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
            return null;
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
