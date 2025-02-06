using UnityEngine;
// /!\ DO NOT TOUCH THIS SCRIPT UNDER ANY CIRCUMSTANCES /!\
// This script handles the Scriptable Object that will hold the sound effects and music to draw from.
// Last updated: 2/6/25
public enum eEffects { effect1, effect2, effect3, effect4, effect5, effect6 }
public enum eSongs { song1, song2, song3, song4 }

[CreateAssetMenu(fileName = "New SoundLibrary", menuName = "Create SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    [NamedArray(typeof(eEffects))] public AudioClip[] effects;
    [NamedArray(typeof(eSongs))] public AudioClip[] songs;
}
