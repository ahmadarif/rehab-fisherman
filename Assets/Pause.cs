using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {

	[SerializeField]
	private GameObject pausePanel;
	bool isPaused = false;

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Escape)) 
		{
			if (!pausePanel.activeInHierarchy) 
			{
				PauseGame();
			}
			if (pausePanel.activeInHierarchy) 
			{
				ContinueGame();   
			}
		} 
	}

	public void PauseGame()
	{
		isPaused = !isPaused;
		if (isPaused) {
			Time.timeScale = 0;
			pausePanel.SetActive(true);
			//Disable scripts that still work while timescale is set to 0	
		} else {
			ContinueGame ();
		}

	} 
	public void ContinueGame()
	{
		Time.timeScale = 1;
		pausePanel.SetActive(false);
		//enable the scripts again
	}
}
