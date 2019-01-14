using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Locomotion;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.ScenarioTesting
{
    public class ScenarioTestingUIController : MonoBehaviour
    {
        public GameCourseController GameCourseController;

        public void MyStart()
        {
        }

        public void MyUpdate()
        {
            GameCourseController.MyUpdate();
        }

        public UnitModel CreateUnit(GameObject prefab, MyHexPosition startPosition, Orientation startOrientation, MyPlayer player)
        {
            return GameCourseController.AddUnit(startPosition, player, startOrientation, prefab).Model;
        }

        //after units setting
        public void FinalizeStart()
        {
            GameCourseController.NextPhrase();
        }

        public void Reset() // such custom reseting is not optimal, but good for now
        {
            GameCourseController.Reset();
        }

        public bool IsDurningLocomotion => GameCourseController.CourseState != GameCourseState.Interactive;

        public void Move(UnitModel unit, MyHexPosition targetPosition)
        {
            var possibleTargets = GameCourseController.GetPossibleMoveTargets(unit);
            if (!possibleTargets.Contains(targetPosition))
            {
                throw new ImpossibleMovePredictionException(unit, targetPosition, possibleTargets);
            }
            GameCourseController.MoveTo(targetPosition,unit, null);
        }

        public bool IsPositionMovable(UnitModel unit, MyHexPosition position)
        {
            var possibleTargets = GameCourseController.GetPossibleMoveTargets(unit);
            return possibleTargets.Any(c => c.Equals(position));
        }
    }

    public class ImpossibleMovePredictionException : Exception
    {
        public ImpossibleMovePredictionException(UnitModel unit, MyHexPosition targetPosition, List<MyHexPosition> possibleTargets)
            :base($"Cannot move unit {unit} to position {targetPosition}. Only possible moves are {possibleTargets.Select(c => c.ToString()).Aggregate((acc, c) => acc+","+c)}")
        {
        }
    }
}
