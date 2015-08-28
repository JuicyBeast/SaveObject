﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(SaveObject))]
public class SaveObjectEditor : Editor {
	SaveObject myTarget;
	string nextLoad = "";
	private bool unsavedChanges;
	private int changeCD;
	private string previousSerialize;
	void Awake() {
		myTarget = (SaveObject)target;
		nextLoad = SaveObject.saveName;
		resetChange();
	}

	private void resetChange() {
		unsavedChanges = false;
		FieldInfo info = typeof(SaveObject).GetField("lastSave", BindingFlags.NonPublic | BindingFlags.Static);
		previousSerialize = (string)info.GetValue(null);
		changeCD = 1100;
	}
	public override void OnInspectorGUI() {
		Object obj = PrefabUtility.GetPrefabParent(myTarget.gameObject);
		string path =AssetDatabase.GetAssetPath(myTarget.gameObject);
		if (path == "") {
			if (obj != null || Application.isPlaying) {
				
				if (!Application.isPlaying) {
					Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(obj));
					DestroyImmediate(myTarget.gameObject);
					Debug.LogError("IMPORTANT: Cannot put SaveObject in scene");
				} else {
					EditorGUILayout.BeginHorizontal();
					string buttonText = "Loaded:";
					if(nextLoad != SaveObject.saveName){
						buttonText = "Load:";
					}
					if (GUILayout.Button(buttonText)) {
						if (nextLoad != "None") {
							Load();
							resetChange();
						}
					} 
					
					nextLoad = EditorGUILayout.TextField(nextLoad);
					EditorGUILayout.EndHorizontal();
					if (SaveObject.saveName == "None") {
						if (GUILayout.Button("Load Default")) {
							nextLoad = "";
							Load();
							resetChange();
						}
						return;
					}
					EditorGUILayout.BeginHorizontal();

					if (GUILayout.Button("Save"+ (unsavedChanges? "*":""))) {
						SaveObject.Save();
						resetChange();
					}

					if (GUILayout.Button("Call Refresh")) {
						MethodInfo dynMethod = myTarget.GetType().GetMethod("Refresh", BindingFlags.NonPublic | BindingFlags.Instance);
						dynMethod.Invoke(myTarget, new object[0]);
					}

					if (GUILayout.Button("New Game")) {
						SaveObject.NewGame();
						FieldInfo info = typeof(SaveObject).GetField("instance", BindingFlags.NonPublic | BindingFlags.Static);
						Selection.activeObject = ((SaveObject)info.GetValue(null)).gameObject;
						resetChange();
					}
					EditorGUILayout.EndHorizontal();
					if (!unsavedChanges) {
						if (changeCD++ > 2 && SaveObject.saveName != "None") {
							changeCD = 0;
							if (JSONLevelSerializer.Serialize(myTarget.gameObject) != previousSerialize) {
								unsavedChanges = true;
								changeCD = -10000000;
							}
						}
					}
				}
			} else {
				if (GUILayout.Button("Place In Project")) {
					string folder = "Resources";
					if (!System.IO.Directory.Exists(Application.dataPath + "/" + folder)) {
						AssetDatabase.CreateFolder("Assets", folder);
					}
					string fullPath = "Assets/" + folder + "/" + myTarget.gameObject.name + ".prefab";
					obj = PrefabUtility.CreatePrefab(fullPath, myTarget.gameObject);
					Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(fullPath);
					DestroyImmediate(myTarget.gameObject);
				}
			}
		} else {
			GUIStyle style = new GUIStyle();
			style.font = EditorStyles.boldFont;
			style.normal = EditorStyles.boldLabel.normal;
			style.alignment = TextAnchor.MiddleCenter;
			EditorGUILayout.LabelField("New Game Stats", style);
			if (GUILayout.Button("Get Initialization Script")) {
				string resFol = "/Resources/";
				if (path.Contains(resFol)) {
					int begin = path.IndexOf(resFol) + resFol.Length;
					path = path.Substring(begin, path.Length - begin - 7);
					EditorGUIUtility.systemCopyBuffer = "SaveObject.Initialize(\""+path+"\");";
				} else {
					EditorGUIUtility.systemCopyBuffer = "SaveObject.Initialize([LINK TO SAVER PREFAB]);";
				}
				Debug.LogWarning(EditorGUIUtility.systemCopyBuffer + " << In the clipboard, copy paste in your script to initialize");
			}
		}
	}

	private void Load() {
		SaveObject.Load(nextLoad);
		GUIUtility.keyboardControl = 0;
		Selection.activeObject = ((SaveObject)typeof(SaveObject).GetField("instance", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).gameObject;
	}
}
