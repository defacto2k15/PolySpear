using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class InitialGameStateCreator : MonoBehaviour
    {
        public List<InitialUnitStateSpecification> Player1Units;
        public List<InitialUnitStateSpecification> Player2Units;

        public void InitializePlayer1Units( GameCourseController controller)
        { 
            foreach (var specification in Player1Units)
            {
                controller.AddUnit(specification.Position, MyPlayer.Player1, specification.Orientation, specification.UnitPrefab);
            }
        }

        public void InitializePlayer2Units( GameCourseController controller)
        {
            foreach (var specification in Player2Units)
            {
                controller.AddUnit(specification.Position, MyPlayer.Player2, specification.Orientation, specification.UnitPrefab);
            }
        }

    }
}