using UnityEngine;
using System.Collections;

public class Bow : MonoBehaviour {
	private float maxRange;
	private float minRange;
	private float currentRange;
	
	public float maxSpeed;
	private float speedAtMaxRange;
	private float minSpeed;
	public float currentSpeed;
	
	public float reloadTime;
	public bool fullRange;
	private bool fullCharge;
	private float currentCharge;
	
	private RangeIndicator rangeIndicator;
	private bool indicatorActive;
	
	public void Initialize() {
		//units (distance of 1 on the map = distance of 1 in range)
		maxRange = 25;
		minRange = 7;
		
		//units/second
		maxSpeed = 60;
		speedAtMaxRange = 30;
		minSpeed = 20;
		
		currentRange = minRange;
		currentSpeed = minSpeed;
		
		reloadTime = 1; //seconds
		fullRange = false;
		fullCharge = false;
		currentCharge = 0;
	}
	
	public void instantiateRangeIndicator(GameObject player) {
		rangeIndicator = new RangeIndicator ();
		rangeIndicator.Initialize (player);
		indicatorActive = false;
	}
	
	public float getCharge() {
		return currentCharge;
	}
	
	public bool fullyCharged() {
		return fullCharge;
	}
	
	public void initializeRangeIndicator(Quaternion direction, ArrowType arrow, bool arrowAvailable) {
		rangeIndicator.useLineIndicator (direction);
		if (arrowAvailable) {
			switch (arrow) {
			case ArrowType.FireArrow:
				rangeIndicator.changeSpeedModifier (0.7f);
				rangeIndicator.changeRangeLimitMultiplier (0.6f);
				rangeIndicator.useAOEIndicator (direction);
				rangeIndicator.changeAOEModifiers (4, 1.0f);
				break;
			case ArrowType.IceArrow:
				rangeIndicator.changeSpeedModifier (0.9f);
				rangeIndicator.changeRangeLimitMultiplier (0.8f);
				rangeIndicator.useAOEIndicator (direction);
				rangeIndicator.changeAOEModifiers (7, 5.0f);
				break;
			case ArrowType.ForceArrow:
				rangeIndicator.changeSpeedModifier (1.2f);
				rangeIndicator.changeRangeLimitMultiplier (1.0f);
				break;
			case ArrowType.PiercingArrow:
				rangeIndicator.changeSpeedModifier (1.0f);
				rangeIndicator.changeRangeLimitMultiplier (1.2f);
				break;
			case ArrowType.SplitArrow:
				rangeIndicator.changeSpeedModifier (0.8f);
				rangeIndicator.changeRangeLimitMultiplier (1.0f);
				rangeIndicator.useSplitIndicator (direction);
				rangeIndicator.changeSplitModifiers (120.0f, 4);
				break;
			case ArrowType.TreeArrow:
				rangeIndicator.changeSpeedModifier (1.5f);
				rangeIndicator.changeRangeLimitMultiplier (0.1f);
				rangeIndicator.useTreeIndicator (direction);
				rangeIndicator.changeTreeModifiers (3, 2.0f, 3.0f);
				break;
			default:
				break;
			}
		}
		indicatorActive = true;
	}
	
	public void deleteRangeIndicator() {
		if (indicatorActive) {
			rangeIndicator.deleteIndicators ();
			rangeIndicator.resetModifiers ();
			indicatorActive = false;
		}
	}
	
	public void updateIndicator(Quaternion direction) {
		rangeIndicator.updateBehavior (direction, currentRange, currentSpeed, currentCharge);
	}
	
	private float timeToMaxRange = 1.5f; //seconds
	private float timeToMaxSpeed = 3.0f; //seconds
	public void Charge() {
		if (currentRange < maxRange) {
			currentRange += ( (maxRange-minRange) / timeToMaxRange )  * Time.deltaTime;
		} else {
			fullRange = true;
		}
		
		if (currentSpeed < maxSpeed) {
			if(fullRange) {
				currentSpeed += ( (maxSpeed-speedAtMaxRange) / (timeToMaxSpeed-timeToMaxRange) ) * Time.deltaTime;
			} else {
				currentSpeed += ( (speedAtMaxRange-minSpeed) / timeToMaxRange ) * Time.deltaTime;
			}
		} else {
			fullCharge = true;
		}
		
		if (currentCharge < 3.0f) {
			currentCharge += Time.deltaTime;
		}
	}
	
	public void Release(Quaternion direction, GameObject arrow) {
		Vector3 velocity = new Vector3(0, 0, currentSpeed);
		if (indicatorActive) {
			velocity *= rangeIndicator.getSpeedModifier();
		}
		arrow.rigidbody.velocity = (direction * velocity);
		
		arrow.GetComponent<Arrow> ().getArrowType ();
		
		Arrow arrowComponent = arrow.GetComponent<Arrow>();
		arrowComponent.Initialize (currentSpeed, currentRange, currentCharge);
		
		resetCharge ();
	}
	
	public void resetCharge() {
		deleteRangeIndicator ();
		currentRange = minRange;
		currentSpeed = minSpeed;
		fullRange = false;
		fullCharge = false;
		currentCharge = 0;
	}
	
	public float getReloadDelay() {
		return reloadTime;
	}
	
	public float getRange() {
		return currentRange;
	}
	
	public float getMaxRange() {
		return maxRange;
	}
	
	public float getSpeed() {
		return currentSpeed;
	}
}
