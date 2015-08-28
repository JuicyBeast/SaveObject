using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 * Example of a component you can save in the SaveObject, 
 * you can save any number of components in a save object, 
 * each containing any number of various properties.
 * */
public class Progress : MonoBehaviour{
	public int golds;
	public int numRefreshes;
	public List<string> unlocks;
	public List<ShopItem> shop;
	

/**
 * As dictionaries are saved but not displayed, a quick 
 * workaround is to have a list of objects which you 
 * can access with a property that acts like a key.
 * */
	public ShopItem GetShopItem(string name) {
		int i = shop.Count;
		while (i-- > 0) {
			if (shop[i].name == name) {
				return shop[i];
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
public class ShopItem{
	public string name;
	public int level;
}
