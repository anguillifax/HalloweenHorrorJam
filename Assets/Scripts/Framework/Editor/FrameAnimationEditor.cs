using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HHGame
{
	[CustomEditor(typeof(FrameAnimation))]
	[CanEditMultipleObjects]
	internal class FrameAnimationEditor : Editor
	{
		private bool editing = false;
		private int currentPicker = default;
		private readonly List<Sprite> items = new List<Sprite>();

		private FrameAnimation TargetObject => (FrameAnimation)target;

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GUILayout.Space(20);
			GUILayout.Label("Editor Utilities");

			if (GUILayout.Button("Clear Frames"))
			{
				Undo.RecordObject(target, "Clear frames");
				TargetObject.frames = new Sprite[0];
				EditorUtility.SetDirty(target);
			}

			if (GUILayout.Button("Create by Sequentially Selecting Sprites"))
			{
				Undo.RegisterCompleteObjectUndo(target, "Change frames");

				currentPicker = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
				EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, string.Empty, currentPicker);

				items.Clear();
				TargetObject.frames = new Sprite[0];
				EditorUtility.SetDirty(target);
				editing = true;
			}

			switch (Event.current.commandName)
			{
				case "ObjectSelectorUpdated":
					if (editing
						&& EditorGUIUtility.GetObjectPickerControlID() == currentPicker)
					{
						var obj = EditorGUIUtility.GetObjectPickerObject();
						if (items.Count == 0
							|| (items.Count > 0 && items[items.Count - 1] != obj))
						{
							items.Add((Sprite)obj);
							TargetObject.frames = items.ToArray();
							EditorUtility.SetDirty(target);
						}
					}
					break;

				case "ObjectSelectorClosed":
					if (editing)
					{
						currentPicker = -1;
						editing = false;
					}
					break;
			}
		}
	}
}