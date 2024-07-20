using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manager class to handle playing and stopping game sounds
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // Singleton instance of the SoundManager

    public GameSound[] gameSounds; // Array of game sounds

    [SerializeField] private AudioSource soundFXObject; // Audio source prefab for sound effects

    // Initialize the SoundManager instance and set up the audio sources
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        foreach (GameSound gs in gameSounds)
        {
            gs.source = gameObject.AddComponent<AudioSource>();
            gs.source.clip = gs.clip;
            gs.source.volume = gs.volume;
            gs.source.pitch = gs.pitch;
            gs.source.loop = gs.loop;
            gs.source.spatialBlend = gs.spatialBlend;
        }
    }

    // Play a sound by name, optionally at a specific transform position
    public void PlaySound(string name, Transform spawnTransform = null)
    {
        GameSound gs = System.Array.Find(gameSounds, sound => sound.name == name);
        if (gs == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (spawnTransform != null)
        {
            AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
            audioSource.clip = gs.clip;
            audioSource.volume = gs.volume;
            audioSource.pitch = gs.pitch;
            audioSource.loop = gs.loop;
            audioSource.spatialBlend = gs.spatialBlend;
            audioSource.Play();

            if (!gs.loop)
            {
                Destroy(audioSource.gameObject, gs.clip.length);
            }
        }
        else
        {
            gs.source.Play();
        }
    }

    // Stop a sound by name
    public void StopSound(string name)
    {
        GameSound gs = System.Array.Find(gameSounds, sound => sound.name == name);
        if (gs == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        gs.source.Stop();
    }

    // Play a random sound from an array of sound names, optionally at a specific transform position
    public void PlayRandomSound(string[] names, Transform spawnTransform = null)
    {
        int rand = Random.Range(0, names.Length);
        PlaySound(names[rand], spawnTransform);
    }
}
