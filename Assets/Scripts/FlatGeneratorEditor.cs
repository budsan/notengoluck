using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FlatGenerator))]
public class FlatGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		FlatGenerator generator = (FlatGenerator)target;
		if (GUILayout.Button("Build Object"))
		{
			generator.EditorClean();
			generator.Build();
		}
	}
}
