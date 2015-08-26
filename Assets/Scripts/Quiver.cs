using UnityEngine;
using System.Collections;

public class Quiver : MonoBehaviour {
	public struct ArrowCounter {
		public ArrowType type;
		public int amount;
		public string guiLabel;
	}
	
	private GameObject owner;
	public ArrowCounter[] arrows;
	public GUIStyle smallText;

	public bool tutorialMode = false;
	private string[] arrowTooltips  = new string[] {
		"Normal" +
		"\nAn ordinary arrow that will kill characters upon hit.",

		"Fire" +
		"\nCreates an area of fire where the arrow lands." +
		"\nCan destroy crates in one hit." +
		"\n\nCharge = Longer fire duration & area of effect.",

		"Ice" +
		"\nCreates an area of ice where the arrow lands." +
		"\nDrastically slows characters in the area." +
		"\n\nCharge = Longer ice duration & area of effect",

		"Force" +
		"\nLaunches anything upon impact." +
		"\nThe launched object can\nsmash through other characters." +
		"\n\nCharge = More force applied on the target",

		"Tree" +
		"\nCreates a wall of trees where the arrow lands." +
		"\n\nCharge = More trees", 

		"Piercing" +
		"\nWill pierce through everything\n and continue onwards." +
		"\n\nCharge = No additional effect", 

		"Split" +
		"\nAfter a short delay, the arrow splits\ninto multiple arrows in an arc." +
		"\n\nCharge = More splits and\nthe angle between them changes"
	};

	public void Initialize(int inventorySize, GameObject owner) {
		arrows = new ArrowCounter[inventorySize];
		
		//First Index will be normal arrows
		arrows [0].type = ArrowType.Arrow;
		arrows [0].amount = -1; //Infinite
		
		for (int i = 1; i < arrows.Length; i++) {
			arrows [i].amount = 0;
		}
		
		this.owner = owner;
		
		smallText = new GUIStyle ();
		smallText.fontSize = 14;
		smallText.alignment = TextAnchor.UpperCenter;
		smallText.normal.textColor = new Color (1, 1, 1);
	}
	
	private void generateGUILabel(int index) {
		switch (arrows [index].type) {
		case ArrowType.FireArrow:
			arrows[index].guiLabel = "Fire";
			break;
		case ArrowType.IceArrow:
			arrows[index].guiLabel = "Ice";
			break;
		case ArrowType.ForceArrow:
			arrows[index].guiLabel = "Force";
			break;
		case ArrowType.TreeArrow:
			arrows[index].guiLabel = "Tree";
			break;
		case ArrowType.PiercingArrow:
			arrows[index].guiLabel = "Piercing";
			break;
		case ArrowType.SplitArrow:
			arrows[index].guiLabel = "Split";
			break;
		}
	}
	
	public void GUIDisplay(ArrowType currentSelected) {
		int index = findInQuiver (currentSelected);
		GUIStyle style = new GUIStyle ();
		style.alignment = TextAnchor.MiddleLeft;
		style.normal.textColor = new Color (1, 0, 0);
		
		int x = 0;
		int boxWidth = 50;
		for(int i = 1; i < arrows.Length; i++) {
				GUI.Label(new Rect(x + (boxWidth * 0.40f), 2, 30, 20), i.ToString());
				GUI.Box(new Rect(x, 20, boxWidth, 35), "");
				if(i == index) {
					GUI.Label(new Rect(x + (boxWidth * 0.40f), 33, 30, 20), arrows[i].amount.ToString(), style);
					smallText.normal.textColor = new Color(1, 0, 0);
				} else {
					GUI.Label(new Rect(x + (boxWidth * 0.40f), 33, 30, 20), arrows[i].amount.ToString());
					smallText.normal.textColor = new Color(1, 1, 1);
				}
				
				GUI.Label(new Rect(x, 20, boxWidth, 40), arrows[i].guiLabel, smallText);
				
				x += boxWidth;
		}

		if (tutorialMode) {
			GUIStyle bigger = new GUIStyle(GUI.skin.box);
			bigger.fontSize = 18;
			GUI.Box (new Rect (0, 60, 410, 150), arrowTooltips [((int)currentSelected)], bigger);
		}
	}
	
	//Returns index of existing arrowType if stored, or next empty one
	private int findInQuiver(ArrowType type) {
		int index = -1;
		for(int i = 0; i < arrows.Length; i++) {
			if(arrows[i].type == type) {
				index = i;
				i = arrows.Length;
			}
		}
		return index;
	}

	private int findEmpty(ArrowType type) {
		int index = -1;
		for(int i = 1; i < arrows.Length; i++) {
			if(arrows[i].amount == 0) {
				index = i;
				i = arrows.Length;
			}
		}
		return index;
	}
	
	public void addArrow(ArrowType arrow, int amount) {
		int index = findInQuiver (arrow);

		if (index == -1) {
			index = findEmpty (arrow);
		}
		
		if (index != -1) {
			arrows [index].type = arrow;
			arrows [index].amount += amount;
			generateGUILabel (index);
		}
	}
	
	public bool pullArrow(ArrowType arrow) {
		int index = findInQuiver (arrow);
		
		bool result = true;
		if (arrows [index].amount == 0) {
			result = false;
		} else {
			result = true;
			arrows [index].amount--;
		}
		return result;
	}

	public bool arrowInQuiver(ArrowType arrow) {
		int index = findInQuiver (arrow);
		return (arrows[index].amount == 0) ? false : true;
	}
}





