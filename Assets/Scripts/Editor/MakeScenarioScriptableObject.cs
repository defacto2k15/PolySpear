using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ScenarioTesting;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    public class MakeScenarioScriptableObject
    {
        [MenuItem("Assets/Create/TestingScenario")]
        public static void CreateMyAsset()
        {
            Scenario asset = ScriptableObject.CreateInstance<Scenario>();

            AssetDatabase.CreateAsset(asset, "Assets/Resources/Testing/Scenarios/NewScenario.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}

