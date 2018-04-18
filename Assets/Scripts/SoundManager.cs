using UnityEngine;

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

	public static void PlaySound(string clip)
	{
		switch(clip)
		{
			case "strike":
				audioSrc.PlayOneShot(strikeSound);
				break;

			case "failed":
				audioSrc.PlayOneShot(failedSound);
				break;

			case "game_win":
				audioSrc.PlayOneShot(gameOverSound);
				break;

			case "fishing_reel":
				audioSrc.PlayOneShot(fishingReelSound);
				break;

            case "button_click":
                audioSrc.PlayOneShot(buttonClickSound);
                break;
        }
    }
}
