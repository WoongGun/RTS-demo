using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class Player : MonoBehaviour {

	public string username;
	public bool human;
	public HUD hud;
	public Color teamColor;
	
	public int startMoney, startMoneyLimit, startPower, startPowerLimit;
	public WorldObject SelectedObject { get; set; }
	public Material notAllowedMaterial, allowedMaterial;

	private bool findingPlacement = false;
	private Dictionary< ResourceType, int > resources, resourceLimits;
	private Building tempBuilding;
	private Unit tempCreator;

	/*** Game Engine Methods ***/
	void Awake() {
		resources = InitResourceList();
		resourceLimits = InitResourceList();
	}

	// Use this for initialization
	void Start () {
		hud = GetComponentInChildren< HUD >();

		AddStartResourceLimits();
		AddStartResources();
	}
	
	// Update is called once per frame
	void Update () {
		if(human) {
			hud.SetResourceValues(resources, resourceLimits);
		}
		if(findingPlacement) {
			tempBuilding.CalculateBounds();
			if(CanPlaceBuilding()) tempBuilding.SetTransparentMaterial(allowedMaterial, false);
			else tempBuilding.SetTransparentMaterial(notAllowedMaterial, false);
		}
	}
	
	/*** public methods ***/

	/// <summary>
	/// Adds the resource.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="amount">Amount.</param>
	public void AddResource(ResourceType type, int amount) {
		resources[type] += amount;
	}

	/// <summary>
	/// Increments the resource limit.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="amount">Amount.</param>
	public void IncrementResourceLimit(ResourceType type, int amount) {
		resourceLimits[type] += amount;
	}

	/// <summary>
	/// Adds the unit.
	/// </summary>
	/// <param name="unitName">Unit name.</param>
	/// <param name="spawnPoint">Spawn point.</param>
	/// <param name="rallyPoint">Rally point.</param>
	/// <param name="rotation">Rotation.</param>
	/// <param name="creator">Creator.</param>
	public void AddUnit(string unitName, Vector3 spawnPoint, Vector3 rallyPoint, Quaternion rotation, Building creator) {
		Units units = GetComponentInChildren< Units >();
		GameObject newUnit = (GameObject)Instantiate(ResourceManager.GetUnit(unitName),spawnPoint, rotation);
		newUnit.transform.parent = units.transform;
		Unit unitObject = newUnit.GetComponent< Unit >();
		if(unitObject) {
			unitObject.SetBuilding(creator);
			if(spawnPoint != rallyPoint) unitObject.StartMove(rallyPoint);
		}
	}

	/// <summary>
	/// Creates the building.
	/// </summary>
	/// <param name="buildingName">Building name.</param>
	/// <param name="buildPoint">Build point.</param>
	/// <param name="creator">Creator.</param>
	/// <param name="playingArea">Playing area.</param>
	public void CreateBuilding(string buildingName, Vector3 buildPoint, Unit creator, Rect playingArea) {
		GameObject newBuilding = (GameObject)Instantiate(ResourceManager.GetBuilding(buildingName), buildPoint, new Quaternion());
		tempBuilding = newBuilding.GetComponent< Building >();
		if (tempBuilding) {
			tempCreator = creator;
			findingPlacement = true;
			tempBuilding.SetTransparentMaterial(notAllowedMaterial, true);
			tempBuilding.SetColliders(false);
			tempBuilding.SetPlayingArea(playingArea);
		} else Destroy(newBuilding);
	}

	/// <summary>
	/// Determines whether this instance is finding building location.
	/// </summary>
	/// <returns><c>true</c> if this instance is finding building location; otherwise, <c>false</c>.</returns>
	public bool IsFindingBuildingLocation() {
		return findingPlacement;
	}

	/// <summary>
	/// Finds the building location. Sets tempBuilding position as the new location.
	/// </summary>
	public void FindBuildingLocation() {
		Vector3 newLocation = WorkManager.FindHitPoint(Input.mousePosition);
		newLocation.y = 0;
		tempBuilding.transform.position = newLocation;
	}

	/// <summary>
	/// Determines whether this instance can place building.
	/// </summary>
	/// <returns><c>true</c> if this instance can place building; otherwise, <c>false</c>.</returns>
	public bool CanPlaceBuilding() {
		bool canPlace = true;
		
		Bounds placeBounds = tempBuilding.GetSelectionBounds();
		//shorthand for the coordinates of the center of the selection bounds
		float cx = placeBounds.center.x;
		float cy = placeBounds.center.y;
		float cz = placeBounds.center.z;
		//shorthand for the coordinates of the extents of the selection box
		float ex = placeBounds.extents.x;
		float ey = placeBounds.extents.y;
		float ez = placeBounds.extents.z;
		
		//Determine the screen coordinates for the corners of the selection bounds
		List< Vector3 > corners = new List< Vector3 >();
		corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx+ex,cy+ey,cz+ez)));
		corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx+ex,cy+ey,cz-ez)));
		corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx+ex,cy-ey,cz+ez)));
		corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx-ex,cy+ey,cz+ez)));
		corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx+ex,cy-ey,cz-ez)));
		corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx-ex,cy-ey,cz+ez)));
		corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx-ex,cy+ey,cz-ez)));
		corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx-ex,cy-ey,cz-ez)));
		
		foreach(Vector3 corner in corners) {
			GameObject hitObject = WorkManager.FindHitObject(corner);
			if(hitObject && hitObject.name != "Ground") {
				WorldObject worldObject = hitObject.transform.parent.GetComponent< WorldObject >();
				if(worldObject && placeBounds.Intersects(worldObject.GetSelectionBounds())) canPlace = false;
			}
		}
		return canPlace;
	}

	/// <summary>
	/// Starts the construction.
	/// </summary>
	public void StartConstruction() {
		findingPlacement = false;
		Buildings buildings = GetComponentInChildren< Buildings >();
		if(buildings) tempBuilding.transform.parent = buildings.transform;
		tempBuilding.SetPlayer();
		tempBuilding.SetColliders(true);
		tempCreator.SetBuilding(tempBuilding);
		tempBuilding.StartConstruction();
	}

	/// <summary>
	/// Determines whether this instance cancel building placement.
	/// </summary>
	/// <returns><c>true</c> if this instance cancel building placement; otherwise, <c>false</c>.</returns>
	public void CancelBuildingPlacement() {
		findingPlacement = false;
		Destroy(tempBuilding.gameObject);
		tempBuilding = null;
		tempCreator = null;
	}

	/*** private methods ***/

	private Dictionary< ResourceType, int > InitResourceList() {
		Dictionary< ResourceType, int > list = new Dictionary< ResourceType, int >();
		list.Add(ResourceType.Money, 0);
		list.Add(ResourceType.Power, 0);
		return list;
	}
	
	private void AddStartResourceLimits() {
		IncrementResourceLimit(ResourceType.Money, startMoneyLimit);
		IncrementResourceLimit(ResourceType.Power, startPowerLimit);
	}
	
	private void AddStartResources() {
		AddResource(ResourceType.Money, startMoney);
		AddResource(ResourceType.Power, startPower);
	}
	

}
