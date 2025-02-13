using UnityEngine;
// This script handles the Scriptable Object that will hold the sound effects and music to draw from.
// To add new sound effects to the array, add the name to the appropiate enum list.
// Last updated: 2/12/25
public enum eEffects { click, confirm, back, pause, balloon, teleporter }
public enum eSongs { mainmenu, options, level, extras }

[CreateAssetMenu(fileName = "New SoundLibrary", menuName = "Create SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    [NamedArray(typeof(eEffects))] public AudioClip[] effects;
    [NamedArray(typeof(eSongs))] public AudioClip[] songs;
}
