using UnityEngine;
using UnityEditor;

namespace Siudowski.MatrixEditor
{
	[CustomEditor(typeof(MatrixData))]
	public class MatrixDataEditor : Editor
	{
		private string ButtonTitle => "Open editor window";
		
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			var script = (MatrixData)target;
			
			EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
			
			if(GUILayout.Button(ButtonTitle))
			{
				// setting a reference to this very scriptable object
				MatrixWindow.matrix = script;
				
				EditorWindow.GetWindow(typeof(MatrixWindow), false, MatrixWindow.WindowTitle);
			}
			
			EditorGUI.EndDisabledGroup();
		}
	}
}