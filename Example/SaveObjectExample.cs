using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveObjectExample : MonoBehaviour {
	public GameObject saveObject;
	/**
	 * In order to use the SaveObject, we need to initialize it. 
	 * It will create a SaveObject instance, make sure you link 
	 * correctly to the save object either with a string with the 
	 * Resources folder as root or a direct link to the prefab.
	 * */
	void Start () {
		/**
		 * You need to call SaveObject.Initialize before doing anything with SaveObject.
		 * You can also set the refreshCallback whenever you want with.
		 * */

		SaveObject.Initialize(saveObject, OnRefresh);
		//You can also set a refresh callback like this:
		SaveObject.SetRefreshCallback(OnRefresh);
		
		/**
		 * It is recommended that you Load right after initialization, it makes sure that
		 * you always have a game ready to be used. 
		 * The next line is commented out to show what a "None" save state looks like.
		 * */
		//SaveObject.Load("Default");
	}

	/**
	 * Use the RefreshCallback to execute the extra logic it takes
	 * to properly reflect the updated values of a loaded game or
	 * when you manually update values in the inspector then press
	 * "Call Refresh"
	 * */
	private void OnRefresh() {
		/**
		 * Use SaveObject.Get<T>(); to get a saved component (component of the SaveObject)
		 * A game must be loaded ino order to be able to use SaveObject.Get<T>()
		 * */
		SaveObject.Get<Progress>().numRefreshes++;
		Debug.Log("Save "+SaveObject.saveName+" Refreshed, number of refreshes: " + SaveObject.Get<Progress>().numRefreshes);
	}

	/**
	 * Save your game like this
	 * */
	public void Save() {
		SaveObject.Save();
	}
	/**
	 * Load your game like this
	 * */
	public void Load() {
		SaveObject.Load();
	}
}
