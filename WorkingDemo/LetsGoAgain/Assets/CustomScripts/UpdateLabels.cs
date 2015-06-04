using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateLabels : MonoBehaviour {

	public Text timerValue;
	public Text openHandStr;
	public Text closeHandStr;
	public Text currentHandStr;
	public int currentPointerIndex; 
	Text[] values;

	// Assign starting values for user-changable variables to class variables
	void Start () {
		values = new Text[] {openHandStr, closeHandStr, timerValue, currentHandStr};
		for(int i=0; i<this.transform.childCount; i++){
			values[i] = this.transform.GetChild(i).gameObject.GetComponent<Text>();
			if(i == 0){
				openHandStr = values[i];
			}
			else if(i == 1){
				closeHandStr = values[i];
			}
			else if(i == 2){
				timerValue = values[i];
			}
			else if(i == 3){
				currentHandStr = values[i];
			}
		}
		currentPointerIndex = 0;
		values[currentPointerIndex].transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Italic;
	}
	
	// Allows user to change value of on-screen variables by useing "/", ",", "." keys
	void Update () {
		if (Input.GetKeyDown(KeyCode.Slash)){
			values[currentPointerIndex].transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Bold;
			currentPointerIndex = (currentPointerIndex+1)%(values.Length - 1);
			values[currentPointerIndex].transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Italic;
		}
		else if (Input.GetKeyDown(KeyCode.Comma)){
			Text currentText =  values[currentPointerIndex].GetComponent<Text>();
			float currentValue = float.Parse(currentText.text.ToString());
			if(currentText.Equals(openHandStr)){
				currentValue -= 0.01f;
				currentValue = Mathf.Round(currentValue * 100f) / 100f;
				if(currentValue > float.Parse(closeHandStr.text)) {
					values[currentPointerIndex].GetComponent<Text>().text = currentValue.ToString();
				}
			}
			else if(currentText.Equals(closeHandStr)){
				currentValue -= 0.01f;
				currentValue = Mathf.Round(currentValue * 100f) / 100f;
				if(currentValue > 0){
					values[currentPointerIndex].GetComponent<Text>().text = currentValue.ToString();
				}
			}
			else{
				currentValue -= 0.1f;
				currentValue = Mathf.Round(currentValue * 100f) / 100f;
				if(currentValue > 0){
					values[currentPointerIndex].GetComponent<Text>().text = currentValue.ToString();
				}
			}
		}
		else if (Input.GetKeyDown(KeyCode.Period)){
			Text currentText =  values[currentPointerIndex].GetComponent<Text>();
			float currentValue = float.Parse(currentText.text.ToString());
			print (currentText);
			if(currentText.Equals(closeHandStr)){
				currentValue += 0.01f;
				currentValue = Mathf.Round(currentValue * 100f) / 100f;
				if(currentValue < float.Parse(openHandStr.text)) {
					values[currentPointerIndex].GetComponent<Text>().text = currentValue.ToString();
				}
			}
			else if(currentText.Equals(openHandStr)){
				currentValue += 0.01f;
				currentValue = Mathf.Round(currentValue * 100f) / 100f;
				values[currentPointerIndex].GetComponent<Text>().text = currentValue.ToString();
			}
			else{
				currentValue += 0.1f;
				currentValue = Mathf.Round(currentValue * 100f) / 100f;
				values[currentPointerIndex].GetComponent<Text>().text = currentValue.ToString();
			}
		}
	}

}
