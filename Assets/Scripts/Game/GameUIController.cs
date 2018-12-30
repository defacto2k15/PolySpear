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


        public void Start()
        {
            _view = GetComponent<GameCourseView>();
        }

        public void Update()
        {
            CourseController.MyUpdate();
            GameCourseState state = CourseController.CourseState;

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
                        }
                        else if (_selectedUnit != null && CourseController.GetPossibleMoveTargets(_selectedUnit).Contains(position))
                        {
                            CourseController.MoveTo(position, _selectedUnit);    
                        }
                        else if (unitAtClickedPlace == null)
                        {
                            _selectedUnit = null;
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
                    var possibleMoveTargets = CourseController.GetPossibleMoveTargets(_selectedUnit);
                    _view.SetMoveTargets(possibleMoveTargets);
                }
                else
                {
                    _view.RemoveSelectedMarker();
                    _view.RemoveMoveTargets();
                }
            }
            else
            {
                _view.RemoveSelectedMarker();
                _view.RemoveMoveTargets();
                _view.MakeSelectorInvisible();
                _selectedUnit = null;
            }
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
