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
            { MyPlayer.Player1, new Color(16.2f/255f, 99f/255f, 2.5f/255f) * 1.3f},
            { MyPlayer.Player2, new Color(67/255f, 0.5f/255f, 71.9f/255f) * 1.3f},
        };

        public static Color SelectorColor => Color.yellow;
        public static Color SelectedUnitMarkerColor => Color.cyan;
        public static Color MoveTargetMarker => Color.blue;
        public static float RotationEpsilon => 0.01f;
        public static float RotationSpeed => 0.1f;
        public static float MotionEpsilon => 0.001f;
        public static float MotionSpeed => 5f;
        public static double DeathAnimationLength => 2f;
        public static float DeathAnimationLoopLength => 0.5f;
    }
}
