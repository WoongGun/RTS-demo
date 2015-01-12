using UnityEngine;
using RTS;

public class Resource : WorldObject {
	
	//Public variables
	public float capacity;
	
	//Variables accessible by subclass
	protected float amountLeft;
	protected ResourceType resourceType;
	
	/*** Game Engine methods, all can be overridden by subclass ***/
	
	protected override void Start () {
		base.Start();
		amountLeft = capacity;
		resourceType = ResourceType.Unknown;
	}

	protected override void CalculateCurrentHealth (float lowSplit, float highSplit) {
		healthPercentage = amountLeft / capacity;
		healthStyle.normal.background = ResourceManager.GetResourceHealthBar(resourceType);
	}
	
	/*** Public methods ***/

	/// <summary>
	/// Remove the specified amount.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void Remove(float amount) {
		amountLeft -= amount;
		if(amountLeft < 0) amountLeft = 0;
	}

	/// <summary>
	/// Ises the empty.
	/// </summary>
	/// <returns><c>true</c>, if empty was ised, <c>false</c> otherwise.</returns>
	public bool isEmpty() {
		return amountLeft <= 0;
	}

	/// <summary>
	/// Gets the type of the resource.
	/// </summary>
	/// <returns>The resource type.</returns>
	public ResourceType GetResourceType() {
		return resourceType;
	}
}
