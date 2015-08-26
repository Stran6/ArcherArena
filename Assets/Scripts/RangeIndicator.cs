using UnityEngine;
using System.Collections;

public class RangeIndicator : MonoBehaviour {
	protected GameObject owner;
	
	private GameObject lineIndicator;
	private float rangeModifier = 1.0f;
	
	private GameObject aoeIndicator;
	private float aoeMin = 1.0f;
	private float aoeModifier = 1.0f;
	
	private GameObject treeIndicator;
	private int widthMin = 1;
	private float widthModifier = 1.0f;
	private float chargeModifier = 1.0f;
	
	private GameObject leftSplitIndicator, rightSplitIndicator;
	private bool usingSplitIndicators;
	private float angle = 120.0f;
	private float angleModifier = 1.0f;
	
	private float speed = 60;
	private float speedModifier = 1.0f;
	
	public void Initialize(GameObject owner) {
		this.owner = owner;
		usingSplitIndicators = false;
	}
	
	public void changeRangeLimitMultiplier(float max) {
		rangeModifier = max;
	}
	public void changeAOEModifiers(float baseSize, float growthRate) {
		aoeMin = baseSize;
		aoeModifier = growthRate;
	}
	public void changeTreeModifiers(int baseSize, float growthRate, float chargeRate) {
		widthMin = baseSize;
		widthModifier = growthRate;
		chargeModifier = chargeRate;
	}
	public void changeSplitModifiers(float baseAngle, float baseSplits) {
		angle = baseAngle;
		angleModifier = baseSplits;
	}
	public void changeSpeedModifier(float max) {
		speedModifier = max;
	}
	public float getSpeedModifier() {
		return speedModifier;
	}
	
	public void useLineIndicator(Quaternion direction) {
		lineIndicator = (GameObject)Instantiate (Resources.Load ("Range"), owner.transform.position, direction);
	}
	public void useAOEIndicator(Quaternion direction) {
		aoeIndicator = (GameObject)Instantiate (Resources.Load ("aoeIndicator"), owner.transform.position, Quaternion.identity);
	}
	public void useTreeIndicator(Quaternion direction) {
		treeIndicator = (GameObject)Instantiate (Resources.Load ("CubeRange"), owner.transform.position, direction);
	}
	public void useSplitIndicator(Quaternion direction) {
		leftSplitIndicator = ((GameObject)Instantiate (Resources.Load ("Range"), owner.transform.position, direction));
		rightSplitIndicator =  ((GameObject)Instantiate (Resources.Load ("Range"), owner.transform.position, direction));
		usingSplitIndicators = true;
	}
	
	public void resetModifiers() {
		rangeModifier = 1.0f;
		
		aoeMin = 1.0f;
		aoeModifier = 1.0f;
		
		widthMin = 1;
		widthModifier = 1.0f;
		
		angle = 120.0f;
		angleModifier = 1.0f;
		
		speedModifier = 1.0f;
	}
	
	public void deleteIndicators() {
		if (lineIndicator) {
			Destroy (lineIndicator);
		}
		if (aoeIndicator) {
			Destroy (aoeIndicator);
		}
		if (treeIndicator) {
			Destroy (treeIndicator);
		}
		if (usingSplitIndicators) {
			Destroy (leftSplitIndicator);
			Destroy (rightSplitIndicator);
			usingSplitIndicators = false;
		}
	}
	
	private void changeColor(GameObject indicator, float baseColor, float currentSpeed) {
		float actualSpeed = speed;
		actualSpeed *= (speedModifier <= 1.0f) ? speedModifier : 1.0f;
		float colorChange = 1.3f - (currentSpeed / actualSpeed);
		if (currentSpeed >= actualSpeed) {
			colorChange = 0.0f;
		}
		indicator.renderer.material.color = new Color (baseColor * colorChange, baseColor, baseColor * colorChange);
	}
	
	private void lineIndicatorUpdate (Quaternion direction, float currentRange, float currentSpeed, float currentCharge) {
		if (lineIndicator) {
			lineIndicator.transform.rotation = direction;
			lineIndicator.transform.position = owner.transform.position + (direction * new Vector3 (0, 0, 1.1f + (currentRange * rangeModifier)/2));
			lineIndicator.transform.localScale = new Vector3 (0.2f, 0.05f, currentRange * rangeModifier);
			
			changeColor(lineIndicator, 0.9f, currentSpeed);
		}
	}
	
	private void AOEIndicatorUpdate (Quaternion direction, float currentRange, float currentSpeed, float currentCharge) {
		if (aoeIndicator) {
			aoeIndicator.transform.position = owner.transform.position + (direction * new Vector3 (0, 0, 1.1f + (currentRange * rangeModifier)));
			float diameter = aoeMin + (1 + (currentCharge * aoeModifier));
			diameter *= 0.5f;
			aoeIndicator.transform.localScale = new Vector3 (diameter, 0.04f, diameter);
			
			changeColor(aoeIndicator, 0.8f, currentSpeed);
		}
	}
	
	private void TreeIndicatorUpdate(Quaternion direction, float currentRange, float currentSpeed, float currentCharge) {
		if (treeIndicator) {
			treeIndicator.transform.rotation = direction;
			treeIndicator.transform.position = owner.transform.position + (direction * new Vector3 (0, 0, 1.1f + (currentRange * rangeModifier)));
			
			int numTrees = (int)(chargeModifier * currentCharge);
			if(numTrees > 3) { numTrees = 3; }
			int width = (int) (widthMin + ((int)numTrees * widthModifier));
			treeIndicator.transform.localScale = new Vector3 (width, 0.03f, 0.5f);
			
			changeColor(treeIndicator, 0.8f, currentSpeed);
		}
	}
	
	private void SplitIndicatorUpdate(Quaternion direction, float currentRange, float currentSpeed, float currentCharge) {
		if (usingSplitIndicators) {
			float modifiedAngle = angle / ((currentCharge + 1) * angleModifier);
			Vector3 normalDirection = (direction * new Vector3(0, 0, 1));
			float modifiedRange = currentRange * rangeModifier;
			Vector3 initialPosition = direction * new Vector3(0, 0, modifiedRange * 0.05f);
			Vector3 positionOffset = new Vector3(0, 0, modifiedRange * 0.45f);
			Vector3 commonScale = new Vector3 (0.2f, 0.05f, modifiedRange * 0.80f);
			
			Vector3 splitDirection = Quaternion.Euler(0, modifiedAngle, 0) * normalDirection;
			rightSplitIndicator.transform.rotation = Quaternion.LookRotation(splitDirection);
			rightSplitIndicator.transform.RotateAround(lineIndicator.transform.position, Vector3.up, modifiedAngle);
			rightSplitIndicator.transform.position = owner.transform.position + initialPosition + (rightSplitIndicator.transform.rotation * positionOffset);
			rightSplitIndicator.transform.localScale = commonScale;
			
			Vector3 leftDirection = Quaternion.Euler(0, -modifiedAngle, 0) * normalDirection;
			leftSplitIndicator.transform.rotation = Quaternion.LookRotation(leftDirection);
			leftSplitIndicator.transform.RotateAround(lineIndicator.transform.position, Vector3.up, -modifiedAngle);
			leftSplitIndicator.transform.position = owner.transform.position + initialPosition + (leftSplitIndicator.transform.rotation * positionOffset);
			leftSplitIndicator.transform.localScale = commonScale;
			
			changeColor (leftSplitIndicator, 0.9f, currentSpeed);
			changeColor (rightSplitIndicator, 0.9f, currentSpeed);
		}
	}
	
	public void updateBehavior (Quaternion direction, float currentRange, float currentSpeed, float currentCharge) {
		lineIndicatorUpdate (direction, currentRange, currentSpeed, currentCharge);
		AOEIndicatorUpdate (direction, currentRange, currentSpeed, currentCharge);
		TreeIndicatorUpdate (direction, currentRange, currentSpeed, currentCharge);
		SplitIndicatorUpdate (direction, currentRange, currentSpeed, currentCharge);
	}
}