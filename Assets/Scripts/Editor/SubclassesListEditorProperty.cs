using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Symbols;
using Assets.Scripts.Units;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    public class SubclassesListEditorProperty
    {
        [CustomPropertyDrawer(typeof(SubclassesInList))]
        public class StringInListDrawer : PropertyDrawer
        {
            // Draw the property inside the given rect
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var stringInList = attribute as SubclassesInList;
                var list = stringInList.List;
                if (property.propertyType == SerializedPropertyType.String)
                {
                    int index = Mathf.Max(0, Array.IndexOf(list, property.stringValue));
                    index = EditorGUI.Popup(position, property.displayName, index, list);

                    property.stringValue = list[index];
                }
                else
                {
                    base.OnGUI(position, property, label);
                }
            }
        }

    }
}
