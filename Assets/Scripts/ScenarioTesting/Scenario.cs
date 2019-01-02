using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts;
using Assets.Scripts.Game;
using UnityEditor;
using UnityEngine;

namespace Assets.ScenarioTesting
{
    public class Scenario : ScriptableObject
    {
        public string FileName_DO_NOT_CHANGE_BY_HAND;
        public string TestDescription;
        public bool TestEnabled = true;
        public List<UnitStartDefinition> StartStates;
        public List<ScenarioMovement> Movements;
        public List<ScenarioPrediction> Predictions;

        public void OnEnable()
        {
            var path = AssetDatabase.GetAssetPath(this);
            if (path != null)
            {
                var filename = path.Split('/').Last().Split('.').First();
                if (string.IsNullOrEmpty(FileName_DO_NOT_CHANGE_BY_HAND))
                {
                    FileName_DO_NOT_CHANGE_BY_HAND = filename;
                }
            }
        }
    } 

    [Serializable]
    public class UnitStartDefinition
    {
        public GameObject UnitPrefab;
        public ScenarioUnitState State;
        public MyPlayer OwningPlayer;
    }


    [Serializable]
    public class ScenarioMovement
    {
        public int UnitIndex;
        public MyHexPosition NewPosition;
    }

    [Serializable]
    public class ScenarioPrediction
    {
        public int UnitIndex;
        public ScenarioPredictionType Type;
        public ScenarioUnitState PredictedState;
        public MyHexPosition SelectorPosition;

        public string Description
        {
            get
            {
                if (Type == ScenarioPredictionType.UnitIsDestroyed)
                {
                    return $"Unit of index {UnitIndex} was to be destroyed";
                }
                else if(Type == ScenarioPredictionType.UnitIsInState)
                {
                    return $"Unit of index {UnitIndex} was to be at state {PredictedState}";
                }else if (Type == ScenarioPredictionType.PositionIsMovable)
                {
                    return $"Position {SelectorPosition} should be movable for unit of index {UnitIndex}";
                }
                else
                {
                    return $"Position {SelectorPosition} should not be movable for unit of index {UnitIndex}";
                }    
            }
        }
    }

    public enum ScenarioPredictionType
    {
        UnitIsInState, UnitIsDestroyed, PositionIsMovable, PositionIsNotMovable
    }

    [Serializable]
    public class ScenarioUnitState
    {
        public MyHexPosition Position;
        public Orientation Orientation;

        public override string ToString()
        {
            return $"{nameof(Position)}: {Position}, {nameof(Orientation)}: {Orientation}";
        }

        protected bool Equals(ScenarioUnitState other)
        {
            return Equals(Position, other.Position) && Orientation == other.Orientation;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScenarioUnitState) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Position != null ? Position.GetHashCode() : 0) * 397) ^ (int) Orientation;
            }
        }
    }
}
