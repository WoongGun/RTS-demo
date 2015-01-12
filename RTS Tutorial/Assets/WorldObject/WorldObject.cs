using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class WorldObject : MonoBehaviour {

	public string objectName;
	public Texture2D buildImage;
	public int cost, sellValue, hitPoints, maxHitPoints;

	// protected so they can't be accesed by external but accessible by subclasses
	protected Player player;
	protected string[] actions = {};
	protected bool currentlySelected = false;

	protected Bounds selectionBounds;

	protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

	protected GUIStyle healthStyle = new GUIStyle();
	protected float healthPercentage = 1.0f;

	protected WorldObject target = null;
	protected bool attacking = false;
	public float weaponRange = 10.0f;
	protected bool aiming = false;
	public float weaponRechargeTime = 1.0f;
	private float currentWeaponChargeTime;
	protected bool movingIntoPosition = false;

	private List< Material > oldMaterials = new List< Material >();

	/*** Game engine methods ***/

	// protected virtual so subclasses can have their own implementation but can also use default
	protected virtual void Awake() {
		selectionBounds = ResourceManager.InvalidBounds;
		CalculateBounds();
	}
	
	protected virtual void Start () {
		SetPlayer();
		if(player) SetTeamColor();
	}
	
	protected virtual void Update () {
		currentWeaponChargeTime += Time.deltaTime;
		if(attacking && !movingIntoPosition && !aiming) PerformAttack();
	}
	
	protected virtual void OnGUI() {
		if (currentlySelected) DrawSelection();
	}
	
	/*** protected methods ***/

	/// <summary>
	/// Sets the color of the team.
	/// </summary>
	protected void SetTeamColor() {
		TeamColor[] teamColors = GetComponentsInChildren< TeamColor >();
		foreach(TeamColor teamColor in teamColors) teamColor.renderer.material.color = player.teamColor;
	}

	/// <summary>
	/// Begins the attack.
	/// </summary>
	/// <param name="target">Target.</param>
	protected virtual void BeginAttack(WorldObject target) {
		this.target = target;
		if(TargetInRange()) {
			attacking = true;
			PerformAttack();
		} else AdjustPosition();
	}

	/// <summary>
	/// Aims at target.
	/// </summary>
	protected virtual void AimAtTarget() {
		aiming = true;
		//this behaviour needs to be specified by a specific object
	}

	/// <summary>
	/// Uses the weapon.
	/// </summary>
	protected virtual void UseWeapon() {
		currentWeaponChargeTime = 0.0f;
		//this behaviour needs to be specified by a specific object
	}

	/// <summary>
	/// Draws the selection box.
	/// </summary>
	/// <param name="selectBox">Select box.</param>
	protected virtual void DrawSelectionBox(Rect selectBox) {
		GUI.Box(selectBox, "");
		CalculateCurrentHealth(0.35f, 0.65f);
		DrawHealthBar(selectBox, "");
	}

	/// <summary>
	/// Calculates the current health.
	/// </summary>
	/// <param name="lowSplit">Low split.</param>
	/// <param name="highSplit">High split.</param>
	protected virtual void CalculateCurrentHealth(float lowSplit, float highSplit) {
		healthPercentage = (float)hitPoints / (float)maxHitPoints;
		if(healthPercentage > highSplit) healthStyle.normal.background = ResourceManager.HealthyTexture;
		else if(healthPercentage > lowSplit) healthStyle.normal.background = ResourceManager.DamagedTexture;
		else healthStyle.normal.background = ResourceManager.CriticalTexture;
	}

	/// <summary>
	/// Draws the health bar.
	/// </summary>
	/// <param name="selectBox">Select box.</param>
	/// <param name="label">Label.</param>
	protected void DrawHealthBar(Rect selectBox, string label) {
		healthStyle.padding.top = -20;
		healthStyle.fontStyle = FontStyle.Bold;
		GUI.Label(new Rect(selectBox.x, selectBox.y - 7, selectBox.width * healthPercentage, 5), label, healthStyle);
	}

	/*** public methods ***/

	/// <summary>
	/// Sets the selection.
	/// </summary>
	/// <param name="selected">If set to <c>true</c> selected.</param>
	/// <param name="playingArea">Playing area.</param>
	public virtual void SetSelection(bool selected, Rect playingArea){
		currentlySelected = selected;
		if(selected) this.playingArea = playingArea;
	}

	/// <summary>
	/// Performs the action.
	/// </summary>
	/// <param name="actionToPerform">Action to perform.</param>
	public virtual void PerformAction(string actionToPerform){
		//it is up to children with specific actions to determine what to do with each of those actions
	}

	/// <summary>
	/// Performs mouse click, selecting objects and displaying info..
	/// </summary>
	/// <param name="hitObject">Hit object.</param>
	/// <param name="hitPoint">Hit point.</param>
	/// <param name="controller">Controller.</param>
	public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
		//only handle input if currently selected
		if(currentlySelected && hitObject && hitObject.name != "Ground") {
			WorldObject worldObject = hitObject.transform.parent.GetComponent< WorldObject >();
			//clicked on another selectable object
			if(worldObject) {
				Resource resource = hitObject.transform.parent.GetComponent< Resource >();
				if(resource && resource.isEmpty()) return;
				Player owner = hitObject.transform.root.GetComponent< Player >();
				if(owner) { //the object is controlled by a player
					if(player && player.human) { //this object is controlled by a human player
						//start attack if object is not owned by the same player and this object can attack, else select
						if(player.username != owner.username && CanAttack()) BeginAttack(worldObject);
						else ChangeSelection(worldObject, controller);
					} else ChangeSelection(worldObject, controller);
				} else ChangeSelection(worldObject, controller);
			}
		}
	}

	/// <summary>
	/// Sets the state of the hover based on object under the cursor.
	/// </summary>
	/// <param name="hoverObject">Hover object.</param>
	public virtual void SetHoverState(GameObject hoverObject) {
		//only handle input if owned by a human player and currently selected
		if(player && player.human && currentlySelected) {
			//something other than the ground is being hovered over
			if(hoverObject.name != "Ground") {
				Player owner = hoverObject.transform.root.GetComponent< Player >();
				Unit unit = hoverObject.transform.parent.GetComponent< Unit >();
				Building building = hoverObject.transform.parent.GetComponent< Building >();
				if(owner) { //the object is owned by a player
					if(owner.username == player.username) player.hud.SetCursorState(CursorState.Select);
					else if(CanAttack()) player.hud.SetCursorState(CursorState.Attack);
					else player.hud.SetCursorState(CursorState.Select);
				} else if(unit || building && CanAttack()) player.hud.SetCursorState(CursorState.Attack);
				else player.hud.SetCursorState(CursorState.Select);
			}
		}
	}

	/// <summary>
	/// Determines whether this instance can attack.
	/// </summary>
	/// <returns><c>true</c> if this instance can attack; otherwise, <c>false</c>.</returns>
	public virtual bool CanAttack() {
		//default behaviour needs to be overidden by children
		return false;
	}

	/// <summary>
	/// Sets the player.
	/// </summary>
	public void SetPlayer() {
		player = transform.root.GetComponentInChildren< Player >();
	}

	/// <summary>
	/// Gets the actions.
	/// </summary>
	/// <returns>The actions.</returns>
	public string[] GetActions(){
		return actions;
	}

	/// <summary>
	/// Gets the selection bounds.
	/// </summary>
	/// <returns>The selection bounds.</returns>
	public Bounds GetSelectionBounds() {
		return selectionBounds;
	}

	/// <summary>
	/// Takes the damage.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void TakeDamage(int damage) {
		hitPoints -= damage;
		if(hitPoints<=0) Destroy(gameObject);
	}

	/// <summary>
	/// Calculates the bounds for selectionBounds.
	/// </summary>
	public void CalculateBounds(){
		selectionBounds = new Bounds(transform.position, Vector3.zero);
		foreach (Renderer r in GetComponentsInChildren< Renderer >())
			selectionBounds.Encapsulate(r.bounds);
	}

	/// <summary>
	/// Determines whether this instance is owned by the specified owner.
	/// </summary>
	/// <returns><c>true</c> if this instance is owned by the specified owner; otherwise, <c>false</c>.</returns>
	/// <param name="owner">Owner.</param>
	public bool IsOwnedBy(Player owner) {
		if(player && player.Equals(owner)) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Sets the colliders.
	/// </summary>
	/// <param name="enabled">If set to <c>true</c> enabled.</param>
	public void SetColliders(bool enabled) {
		Collider[] colliders = GetComponentsInChildren< Collider >();
		foreach(Collider collider in colliders) collider.enabled = enabled;
	}

	/// <summary>
	/// Sets the transparent material.
	/// </summary>
	/// <param name="material">Material.</param>
	/// <param name="storeExistingMaterial">If set to <c>true</c> store existing material.</param>
	public void SetTransparentMaterial(Material material, bool storeExistingMaterial) {
		if(storeExistingMaterial) oldMaterials.Clear();
		Renderer[] renderers = GetComponentsInChildren< Renderer >();
		foreach(Renderer renderer in renderers) {
			if(storeExistingMaterial) oldMaterials.Add(renderer.material);
			renderer.material = material;
		}
	}

	/// <summary>
	/// Restores the materials using old materials.
	/// </summary>
	public void RestoreMaterials() {
		Renderer[] renderers = GetComponentsInChildren< Renderer >();
		if(oldMaterials.Count == renderers.Length) {
			for(int i = 0; i < renderers.Length; i++) {
				renderers[i].material = oldMaterials[i];
			}
		}
	}

	/// <summary>
	/// Sets the playing area.
	/// </summary>
	/// <param name="playingArea">Playing area.</param>
	public void SetPlayingArea(Rect playingArea) {
		this.playingArea = playingArea;
	}

	/*** private methods ***/

	private void DrawSelection(){
		GUI.skin = ResourceManager.SelectBoxSkin;
		Rect selectBox = WorkManager.CalculateSelectionBox(selectionBounds, playingArea);
		// draw the selection box around box within playing area.
		GUI.BeginGroup (playingArea);
		DrawSelectionBox(selectBox);
		GUI.EndGroup();
	}
	private bool TargetInRange() {
		Vector3 targetLocation = target.transform.position;
		Vector3 direction = targetLocation - transform.position;
		if(direction.sqrMagnitude < weaponRange * weaponRange) {
			return true;
		}
		return false;
	}
	
	private void AdjustPosition() {
		Unit self = this as Unit;
		if(self) {
			movingIntoPosition = true;
			Vector3 attackPosition = FindNearestAttackPosition();
			self.StartMove(attackPosition);
			attacking = true;
		} else attacking = false;
	}
	
	private Vector3 FindNearestAttackPosition() {
		Vector3 targetLocation = target.transform.position;
		Vector3 direction = targetLocation - transform.position;
		float targetDistance = direction.magnitude;
		float distanceToTravel = targetDistance - (0.9f * weaponRange);
		return Vector3.Lerp(transform.position, targetLocation, distanceToTravel / targetDistance);
	}
	
	private void PerformAttack() {
		if(!target) {
			attacking = false;
			return;
		}
		if(!TargetInRange()) AdjustPosition();
		else if(!TargetInFrontOfWeapon()) AimAtTarget();
		else if(ReadyToFire()) UseWeapon();
	}
	
	private bool TargetInFrontOfWeapon() {
		Vector3 targetLocation = target.transform.position;
		Vector3 direction = targetLocation - transform.position;
		if(direction.normalized == transform.forward.normalized) return true;
		else return false;
	}

	private bool ReadyToFire() {
		if(currentWeaponChargeTime >= weaponRechargeTime) return true;
		return false;
	}
	
	private void ChangeSelection(WorldObject worldObject, Player controller) {
		//this should be called by the following line, but there is an outside chance it will not
		SetSelection(false, playingArea);
		if(controller.SelectedObject) controller.SelectedObject.SetSelection(false, playingArea);
		controller.SelectedObject = worldObject;
		worldObject.SetSelection(true, controller.hud.GetPlayingArea());
	}
}
