using UnityEngine;
using UnityEngine.Audio;
// This script handles the AudioManager, volume sliders and the "play sound effect" method.
// Last updated: 2/12/25
public enum eMixers { music, effects }
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [NamedArray(typeof(eMixers))] public AudioMixerGroup[] mixers;
    [NamedArray(typeof(eMixers))] public float[] volume = { 1f, 1f };
    [NamedArray(typeof(eMixers))] private string[] strMixers = { "MusicVol", "EffectsVol" };

    [SerializeField] private AudioSource BGM;
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
    public static void PauseSong()
    {
        Instance.BGM.Pause();
    }
    public static void ResumeSong()
    {
        Instance.BGM.UnPause();
    }
}
