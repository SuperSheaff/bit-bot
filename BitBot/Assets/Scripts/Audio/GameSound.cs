using UnityEngine.Audio;
using UnityEngine;

// Class representing a game sound with its properties
[System.Serializable]
public class GameSound
{
    public string name; // Name of the sound
    public AudioClip clip; // Audio clip of the sound
    public bool loop; // Flag to indicate if the sound should loop

    [HideInInspector]
    public AudioSource source; // Audio source to play the sound

    [Range(0f, 1f)]
    public float volume; // Volume of the sound

    [Range(0.1f, 3f)]
    public float pitch; // Pitch of the sound

    [Range(0f, 1f)]
    public float spatialBlend = 0.5f; // Spatial blend of the sound (0 is 2D, 1 is 3D)
}
