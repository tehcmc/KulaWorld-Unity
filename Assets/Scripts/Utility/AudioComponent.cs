using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound
{
	[SerializeField] string name;
	[SerializeField] AudioClip clip;

	public string Name { get => name; set => name = value; }
	public AudioClip Clip { get => clip; set => clip = value; }

}

public class AudioComponent : MonoBehaviour
{
	[SerializeField] List<Sound> sounds;

	IDictionary<string, Sound> soundsDict = new Dictionary<string, Sound>();

	float defaultVolume;
	float defaultPitch;
	AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		defaultVolume = audioSource.volume;
		foreach (var sound in sounds)
		{

			soundsDict.Add(sound.Name.ToLower(), sound);
		}

	}



	public void PlaySound(string name)
	{
		string lowerName = name.ToLower();
		if (!soundsDict.ContainsKey(lowerName))
		{
			Debug.Log("no sound");
			return;
		}


		AudioClip sound = soundsDict[lowerName].Clip;
		audioSource.PlayOneShot(sound);
	}



}