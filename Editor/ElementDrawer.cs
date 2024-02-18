using UnityEngine;
using UnityEditor;

namespace Siudowski.MatrixEditor
{
	[CustomPropertyDrawer(typeof(Element))]
	public class ElementDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.PropertyField(position, property.FindPropertyRelative("name"), GUIContent.none);
		}
	}
}