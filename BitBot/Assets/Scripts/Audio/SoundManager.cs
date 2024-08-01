using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manager class to handle playing and stopping game sounds
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // Singleton instance of the SoundManager

    public GameSound[] gameSounds; // Array of game sounds

    [SerializeField] private AudioSource soundFXObject; // Audio source prefab for sound effects

    private Dictionary<string, List<AudioSource>> activeSounds; // Dictionary to track active sounds

    // Initialize the SoundManager instance and set up the audio sources
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            activeSounds = new Dictionary<string, List<AudioSource>>();
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (GameSound gs in gameSounds)
        {
            gs.source = gameObject.AddComponent<AudioSource>();
            gs.source.clip = gs.clip;
            gs.source.volume = gs.volume;
            gs.source.pitch = gs.pitch;
            gs.source.loop = gs.loop;
            gs.source.spatialBlend = gs.spatialBlend;

            // Initialize the activeSounds dictionary
            if (!activeSounds.ContainsKey(gs.name))
            {
                activeSounds[gs.name] = new List<AudioSource>();
            }
        }
    }

    // Play a sound by name
    public void PlaySound(string name, Transform spawnTransform = null)
    {
        GameSound gs = System.Array.Find(gameSounds, sound => sound.name == name);
        if (gs == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        Debug.Log("Playing Sound: " + name);

        // Stop any existing instance of the sound before playing it again
        if (gs.source.isPlaying)
        {
            gs.source.Stop();
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

            activeSounds[name].Add(audioSource); // Track this instance
        }
        else
        {
            gs.source.Play();
        }
    }

    // Stop a sound by name
    public void StopSound(string name)
    {
        if (!activeSounds.ContainsKey(name))
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        Debug.Log("Stopping Sound: " + name);

        // Stop all instances of the sound
        bool soundWasPlaying = false;
        foreach (AudioSource source in activeSounds[name])
        {
            if (source.isPlaying)
            {
                source.Stop();
                soundWasPlaying = true;
                Debug.Log("Stopped AudioSource playing: " + name);
            }
        }

        // Clear the list of stopped sounds
        activeSounds[name].RemoveAll(source => !source.isPlaying);

        if (!soundWasPlaying)
        {
            Debug.LogWarning("Sound was not playing: " + name);
        }
    }

    // Play a random sound from an array of sound names, optionally at a specific transform position
    public void PlayRandomSound(string[] names, Transform spawnTransform = null)
    {
        int rand = Random.Range(0, names.Length);
        PlaySound(names[rand], spawnTransform);
    }
}
