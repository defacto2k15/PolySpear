using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts
{
    // UGLY, Temporary solution
    public class Constants
    {
        public static Dictionary<MyPlayer, Color> PlayersFlagColors = new Dictionary<MyPlayer, Color>()
        {
            { MyPlayer.Player1, Color.yellow },
            { MyPlayer.Player2, Color.green},
        };

        public static Color SelectorColor => Color.yellow;
        public static Color SelectedUnitMarkerColor => Color.cyan;
        public static Color MoveTargetMarker => Color.blue;
        public static float RotationEpsilon => 0.01f;
        public static float RotationSpeed => 0.02f;
        public static float MotionEpsilon => 0.001f;
        public static float MotionSpeed => 5f;
    }
}
