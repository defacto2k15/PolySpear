using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts
{
    public class SubclassesInList : PropertyAttribute
    {
        public SubclassesInList(Type type)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] allTypes = assembly.GetTypes();
            List = allTypes.Where(type.IsAssignableFrom)
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
                .Select(t => t.FullName).ToArray();
        }

        public string[] List { get; private set; }
    }
}