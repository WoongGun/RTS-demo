using UnityEngine;
using System.Collections;
using RTS;

public class GameObjectList : MonoBehaviour {

	public GameObject[] buildings;
	public GameObject[] units;
	public GameObject[] worldObjects;
	public GameObject player;

	private static bool created = false;
	
	void Awake() {
		if(!created) {
			DontDestroyOnLoad(transform.gameObject);
			ResourceManager.SetGameObjectList(this);
			created = true;
		} else {
			Destroy(this.gameObject);
		}
	}

	/*** public methods ***/

	/// <summary>
	/// Gets the building game object.
	/// </summary>
	/// <returns>The building.</returns>
	/// <param name="name">Name.</param>
	public GameObject GetBuilding(string name) {
		for(int i = 0; i < buildings.Length; i++) {
			Building building = buildings[i].GetComponent< Building >();
			if(building && building.name == name) return buildings[i];
		}
		return null;
	}

	/// <summary>
	/// Gets the unit.
	/// </summary>
	/// <returns>The unit.</returns>
	/// <param name="name">Name.</param>
	public GameObject GetUnit(string name) {
		for(int i = 0; i < units.Length; i++) {
			Unit unit = units[i].GetComponent< Unit >();
			if(unit && unit.name == name) return units[i];
		}
		return null;
	}

	/// <summary>
	/// Gets the world object.
	/// </summary>
	/// <returns>The world object.</returns>
	/// <param name="name">Name.</param>
	public GameObject GetWorldObject(string name) {
		foreach(GameObject worldObject in worldObjects) {
			if(worldObject.name == name) return worldObject;
		}
		return null;
	}

	/// <summary>
	/// Gets the player object.
	/// </summary>
	/// <returns>The player object.</returns>
	public GameObject GetPlayerObject() {
		return player;
	}

	/// <summary>
	/// Gets the building image.
	/// </summary>
	/// <returns>The building image.</returns>
	/// <param name="name">Name.</param>
	public Texture2D GetBuildImage(string name) {
		for(int i = 0; i < buildings.Length; i++) {
			Building building = buildings[i].GetComponent< Building >();
			if(building && building.name == name) return building.buildImage;
		}
		for(int i = 0; i < units.Length; i++) {
			Unit unit = units[i].GetComponent< Unit >();
			if(unit && unit.name == name) return unit.buildImage;
		}
		return null;
	}
}
