using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public static AudioClip strikeSound,
		failedSound,
		gameOverSound,
		fishingReelSound,
        buttonClickSound;

	static AudioSource audioSrc;

	// Use this for initialization
	void Start () {
		
		strikeSound = Resources.Load<AudioClip> ("strike");
		failedSound = Resources.Load<AudioClip> ("failed");
		gameOverSound = Resources.Load<AudioClip> ("game_win");
        fishingReelSound = Resources.Load<AudioClip>("fishing_reel");
        buttonClickSound = Resources.Load<AudioClip>("button_click");

        audioSrc = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static void PlaySound(string clip)
	{
		switch(clip)
		{
			case "strike":
				audioSrc.PlayOneShot(strikeSound);
				Debug.Log("Strike Sound Play!");
				break;

			case "failed":
				audioSrc.PlayOneShot(failedSound);
				Debug.Log("Failed Sound Play!");
				break;

			case "game_win":
				audioSrc.PlayOneShot(gameOverSound);
				Debug.Log("Game Over Sound Play!");
				break;

			case "fishing_reel":
				audioSrc.PlayOneShot(fishingReelSound);
				Debug.Log("Fishing Reel Sound Play!");
				break;

            case "button_click":
                audioSrc.PlayOneShot(buttonClickSound);
                Debug.Log("Button Click Sound Play!");
                break;


        }
    }
}
