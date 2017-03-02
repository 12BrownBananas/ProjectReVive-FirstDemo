using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AppleScript : MonoBehaviour {
	
	//Rotation information
	private Quaternion myRotation;
	public float smooth = 1f;
	
	//Interaction information
	public int minUnits; //Amount of units needed to carry, minimum
	public int maxUnits; //Amount of units that can carry at one time
	
	private int firstOpenIndex; //First slot that a unit can fit into
	private int numUnits; //The raw number of units on this fruit
	
	public delegate void CollisionWithTarget();
	public static event CollisionWithTarget OnCollision;
	
	public GameObject myController;
	
	private List<GameObject> units;
	
	private List<GameObject> myUnits;
	
	public float[] xOffset;
	public float[] zOffset;
	
	
	
	// Use this for initialization
	void Start () {
		myRotation = transform.rotation;
		numUnits = 0;
		myUnits = new List<GameObject>();
	}
	
	void OnTriggerExit(Collider target) {
		if (target.gameObject.name == "obj_target") {
			
		}
	}
    
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.name == "obj_target") {
			units = myController.GetComponent<AcceptInput>().CurrentlySelectedUnits;
			for (int i = 0; i < units.Count; i++) {
				if (numUnits < maxUnits && firstOpenIndex < maxUnits) {
					if (units[i].GetComponent<UnitAI>().getGettingObject() == false) {
						units[i].GetComponent<UnitAI>().setGettingObject(true);
						units[i].GetComponent<UnitAI>().setTargetObject(this.gameObject);	
						Vector3 dest = new Vector3(transform.position.x+xOffset[firstOpenIndex], transform.position.y, transform.position.z+zOffset[firstOpenIndex]);
						units[i].GetComponent<UnitAI>().pathToPoint(dest);
						units[i].GetComponent<UnitAI>().setCarryingNum(firstOpenIndex);
						myUnits.Add(units[i]);
						numUnits++;
						getFirstOpenIndex();
					}
				}
			}
		}
	}
	
	public void getFirstOpenIndex() {
		firstOpenIndex = 0;
		List<int> reservedValues = new List<int>();
		for (int i = 0; i < myUnits.Count; i++) {
			reservedValues.Add(myUnits[i].GetComponent<UnitAI>().getCarryingNum());
		}
		for (int i = 0; i < maxUnits; i++) {
			bool isGood = true;
			for (int j = 0; j < reservedValues.Count; j++) {
				if (i == reservedValues[j]) {
					isGood = false;
				}
			}
			if (isGood) {
				firstOpenIndex = i;
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		getFirstOpenIndex();
	}
	
	public void checkWeight() { //We're gonna call this to see if the total units carrying has dropped below the minimum threshhold
		if (numUnits < minUnits) {
			GetComponent<NavMeshAgent>().SetDestination(transform.position);
		}
	}
	
	public bool canBreakOff() {
		int selectedCount = 0;
		for (int i = 0; i < myUnits.Count; i++) {
			if (myUnits[i].GetComponent<UnitAI>().selected == true) {
				selectedCount++;
			}
		}
		if (selectedCount < minUnits) {
			return true;
		}
		return false;
	}
	
	public int getNumUnits() {
		return numUnits;
	}
	public void setNumUnits(int num) {
		numUnits = num;
	}
	
	public void removeUnit(GameObject unit) {
		if (myUnits.Contains(unit)) {
			myUnits.Remove(unit);
		}
	}
}
