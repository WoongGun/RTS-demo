using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public float velocity = 1;
	public int damage = 1;
	
	private float range = 1;
	private WorldObject target;

	/*** game engine methods ***/

	void Update () {
		if(HitSomething()) {
			InflictDamage();
			Destroy(gameObject);
		}
		if(range>0) {
			float positionChange = Time.deltaTime * velocity;
			range -= positionChange;
			transform.position += (positionChange * transform.forward);
		} else {
			Destroy(gameObject);
		}
	}

	/*** public methods ***/

	/// <summary>
	/// Sets the range.
	/// </summary>
	/// <param name="range">Range.</param>
	public void SetRange(float range) {
		this.range = range;
	}

	/// <summary>
	/// Sets the target.
	/// </summary>
	/// <param name="target">Target.</param>
	public void SetTarget(WorldObject target) {
		this.target = target;
	}

	/*** private methods ***/

	private bool HitSomething() {
		if(target && target.GetSelectionBounds().Contains(transform.position)) return true;
		return false;
	}
	
	private void InflictDamage() {
		if(target) target.TakeDamage(damage);
	}
}