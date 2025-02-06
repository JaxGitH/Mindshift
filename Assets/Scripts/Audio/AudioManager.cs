using UnityEngine;
using UnityEngine.Audio;
// This script handles the AudioManager, volume sliders and the "play sound effect" method.
// TODO: Be able to play multiple songs, making the current one stop before playing the new one
// Last updated: 2/6/25
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
    //Call this method and the label of the sound effect in order to play it.
    public static void PlayOneShot(eEffects _effect)
    {
        Instance.Effects.PlayOneShot(GameManager.Instance.soundLibrary.effects[(int)_effect]);
    }
    public static void PlaySong(eSongs _song)
    {
        Instance.BGM.PlayOneShot(GameManager.Instance.soundLibrary.songs[(int)_song]);
    }
}
