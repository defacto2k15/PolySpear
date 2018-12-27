using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.ScenarioTesting
{
    public class Scenario : ScriptableObject
    {
        public string TestDescription;
        public bool TestEnabled = true;
        public List<UnitStartDefinition> StartStates;
        public List<ScenarioMovement> Movements;
        public List<ScenarioPrediction> Predictions;
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

        public string Description
        {
            get
            {
                if (Type == ScenarioPredictionType.UnitIsDestroyed)
                {
                    return $"Unit of index {UnitIndex} was to be destroyed";
                }
                else
                {
                    return $"Unit of index {UnitIndex} was to be at state {PredictedState}";
                }    
            }
        }
    }

    public enum ScenarioPredictionType
    {
        UnitIsInState, UnitIsDestroyed
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
