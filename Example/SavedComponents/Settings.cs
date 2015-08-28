using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Example of a component you can save in the SaveObject, 
 * you can save any number of components in a save object, 
 * each containing any number of various properties.
 * */


public class Settings : MonoBehaviour {
	public string charName;
	public float BGMVol;
	public float SFXVol;
	public Color windowColor;
	public List<ExtraSetting> extra;
	public Dictionary<int, string> lut = new Dictionary<int,string>();

	
/**
 * As dictionaries are saved but not displayed, a quick 
 * workaround is to have a list of objects which you 
 * can access with a property that acts like a key.
 * */
	public ExtraSetting GetExtra(string name) {
		int i = extra.Count;
		while (i-- > 0) {
			if (extra[i].name == name) {
				return extra[i];
			}
		}
		return null;
	}
}

/**
 * Custom classes are also saved. They can also be seen in
 * the inspector if they have the [System.Serializable] tag.
 * */
[System.Serializable]
public class ExtraSetting {
	public string name;
	public float value;
}