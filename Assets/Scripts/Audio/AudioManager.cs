using UnityEngine;
using UnityEngine.Audio;
// This script handles the AudioManager, volume sliders and the "play sound effect" method.
// Last updated: 2/12/25
public enum eMixers { music, effects }
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    static float currentSongPosition;

    [NamedArray(typeof(eMixers))] public AudioMixerGroup[] mixers;
    [NamedArray(typeof(eMixers))] public float[] volume = { 1f, 1f };
    [NamedArray(typeof(eMixers))] private string[] strMixers = { "MusicVol", "EffectsVol" };

    [SerializeField] public AudioSource BGM;
    [SerializeField] private AudioSource Effects;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy New AudioManager");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    public void SetMixerLevel(eMixers _mixer, float _soundLevel)
    {
        mixers[(int)_mixer].audioMixer.SetFloat(strMixers[(int)_mixer], Mathf.Log10(_soundLevel) * 20f);
        volume[(int)_mixer] = _soundLevel;
    }
    //This method calls a sound effect from the array to be played.
    public static void PlayEffect(eEffects _effect)
    {
        Instance.Effects.PlayOneShot(GameManager.Instance.soundLibrary.effects[(int)_effect]);
    }

    //This method calls a song from the array to be played.
    public static void PlaySong(eSongs _song)
    {
        Instance.BGM.Stop();
        Instance.BGM.clip = GameManager.Instance.soundLibrary.songs[(int)_song];
        Instance.BGM.Play();
    }
    //This method exists to stop a current song playing.
    public static void StopSong()
    {
        Instance.BGM.Stop();
    }
    //"PauseLevelSong()" and "ResumeLevelSong()" are called when the Pause menu is pressed so it can resume the level song where it left off
    public static void PauseLevelSong()
    {
        currentSongPosition = Instance.BGM.time;
        Instance.BGM.Pause();
    }
    public static void ResumeLevelSong()
    {
        PlaySong(eSongs.level);
        Instance.BGM.time = currentSongPosition;
        Instance.BGM.UnPause();
    }
    //This catches Pause menu x Options menu x Main menu music interactions
    public static void ResumePrevious(string _sceneName)
    {
        if (_sceneName == "FrontEnd")
        {
            Instance.BGM.Stop();
            PlaySong(eSongs.mainmenu);
        }
        else if (_sceneName == "zone1Level2")
        {
            Instance.BGM.Stop();
        }
    }
}
