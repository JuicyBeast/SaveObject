
using UnityEngine;
using System;

[ExecuteInEditMode]
public class SaveObject : MonoBehaviour {
	#region hidden
	private static SaveObject instance;
	private static Action refreshCallback;

	private static GameObject resetOriginal;
#if UNITY_EDITOR
	private static string lastSave;
#endif
	private static string _saveName = "None";
	public static string saveName {
		get {
			return _saveName;
		}
	}

	void Awake() {
		if (resetOriginal != null) {
			name = resetOriginal.name;
		}
		UniqueIdentifier.identifier = new UniqueIdentifier();
		UniqueIdentifier.identifier.Id = name;
		UniqueIdentifier.identifier.gameObject = gameObject;
		instance = this;
		DontDestroyOnLoad(gameObject);
		
#if UNITY_EDITOR
		if (saveName == "None") { 
			foreach(Component comp in GetComponents<Component>()){
				if(comp!=this && comp!=transform){
					DestroyImmediate(comp);
				}
			}
		}
#endif
	}
	#endregion

	/// <summary>
	///	Initialize Unity Saver
	/// </summary>
	/// <param name="originalPath"> The path to the original save object prefab in the Resources folder</param>
	/// <param name="refreshCallback">The action that will be called when you load a game or press refresh on the SaveObject Component</param>
	public static void Initialize(string originalPath, Action refreshCallback = null) {
		Initialize(Resources.Load(originalPath) as GameObject);
	}

	/// <summary>
	/// Initialize Unity Saver
	/// </summary>
	/// <param name="original"> The original save object prefab</param>
	/// <param name="refreshCallback">The action that will be called when you load a game or press refresh on the SaveObject Component</param>
	public static void Initialize(GameObject original, Action refreshCallback = null) {
		resetOriginal = original;
		Instantiate(original).name = original.name;
		SetRefreshCallback(refreshCallback);
		Save();
	}

	/// <summary>
	/// Set the action that will be called when you load a game or press refresh on the SaveObject Component
	/// </summary>
	/// <param name="refreshCallback">The action that will be called</param>
	public static void SetRefreshCallback(Action refreshCallback) {
		SaveObject.refreshCallback = refreshCallback;
	}

	/// <summary>
	/// Load a game
	/// </summary>
	/// <param name="saveName"> The name of the "file" you want to load, will load the default or currently loaded "file" if left blank</param>
	public static void Load(string saveName = "") {
		
		bool fromNone = false;
		if (_saveName == "None") {
			_saveName = "Default";
			fromNone = true;
		}
		if (!string.IsNullOrEmpty(saveName)) {
			_saveName = saveName;
		}
		
		if (UnityEngine.PlayerPrefs.HasKey(_saveName)) {
			
#if UNITY_EDITOR
			if (fromNone) {
				NewGame();
			}
#endif
			string save = UnityEngine.PlayerPrefs.GetString(_saveName);
			JSONLevelSerializer.Deserialize(save);
#if UNITY_EDITOR
			lastSave = save;
			if (lastSave == "") { 
				//Prevents "value not used" prompt. The value lastSave is used in the Editor Script.
			}
#endif
		} else {
			NewGame();
		}
		
		instance.Refresh();
	}

	/// <summary>
	/// Load a game from a provided string
	/// </summary>
	/// <param name="save"> Serialization string of the saved gameObject</param>
	public static void LoadFromString(string save) {
		bool fromNone = false;
		if (_saveName == "None") {
			_saveName = "Default";
			fromNone = true;
		}
		if (!string.IsNullOrEmpty(saveName)) {
			_saveName = saveName;
		}
#if UNITY_EDITOR
		if (fromNone) {
			NewGame();
		}
#endif
		JSONLevelSerializer.Deserialize(save);
#if UNITY_EDITOR
		lastSave = save;
#endif


		instance.Refresh();
	}

	/// <summary>
	/// Save a game
	/// </summary>
	public static void Save() {
		if (_saveName == "None") return;
		string save = instance.currentSaveString;
		PlayerPrefs.SetString(_saveName, save);
		PlayerPrefs.Save();
#if UNITY_EDITOR
		lastSave = save;
#endif
	}

	/// <summary>
	/// Gets the string used to save the game
	/// </summary>
	/// <returns>the string used to save the game</returns>
	public static string GetSaveString() {
		return instance.currentSaveString;
	}

	/// <summary>
	/// Revert the load state to prefab original state
	/// </summary>
	public static void NewGame() {
#if UNITY_EDITOR
		lastSave = instance.currentSaveString;
#endif
		Destroy(instance.gameObject);
		Instantiate(resetOriginal);
		
	}

	/// <summary>
	/// Get a Component of the save object
	/// </summary>
	public static T Get<T>() where T : Component {
		Component comp = null;
		if (instance == null) {
			Debug.LogError("SaveObject not initialized");
		} else {
			comp = instance.GetComponent<T>();

			if (_saveName == "None") {
				Debug.LogError("No Save Loaded");
			} else if (comp == null) {
				Debug.LogError(typeof(T) + " not present in SaveObject");
			}
		}
		return (T)comp;
	}
	#region hidden
	public string currentSaveString {
		get { return JSONLevelSerializer.Serialize(gameObject); } 
	}

	private void Refresh() {
		if(refreshCallback != null) {
			refreshCallback();
		}
	}
	#endregion
}




public class UniqueIdentifier {
	public string Id;
	public GameObject gameObject;
	public static UniqueIdentifier identifier;
}