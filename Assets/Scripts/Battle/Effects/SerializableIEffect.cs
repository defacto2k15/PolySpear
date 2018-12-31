using System;
using System.Reflection;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Symbols
{
    [Serializable]
    public class SerializableIEffect
    {
        [SerializeField]
        [SubclassesInList(typeof(IEffect))]public string Select;

        public IEffect Value
        {
            get
            {
                var type = Assembly.GetExecutingAssembly().GetType(Select,true);
                return Activator.CreateInstance(type) as IEffect;
            }
        }
        
    }
}