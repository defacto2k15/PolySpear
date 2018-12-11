using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    public class OrientationEditorProperty
    {
        //[CustomPropertyDrawer(typeof(Orientation))]
        public class StringInListDrawer : PropertyDrawer
        {
            // Draw the property inside the given rect
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);
;

                var stringInList = attribute as StringInList;
                var list = stringInList.List.Select(c => c.Name).ToList();
                if (property.propertyType == SerializedPropertyType.Generic)
                {
                    int index = list.Select((c, i) => new {c, i}).Where(c => c.c.Equals(property.stringValue)).Select(c => c.i).FirstOrDefault();
                    index = EditorGUI.Popup(position, property.displayName, index, list.ToArray());

                    //property.objectReferenceValue = stringInList.List[index];
                }
                else
                {
                    base.OnGUI(position, property, label);
                }
            }
        }
    }
}
