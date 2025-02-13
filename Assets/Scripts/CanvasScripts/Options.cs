using UnityEngine;
using UnityEngine.UI;
// This script handles the Options menu, including the volume sliders currently.
// Last update: 2/12/2025
public class Options : MonoBehaviour
{
    public static Options Instance;
    public Slider BGMSlider;
    public Slider SFXSlider;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        BGMSlider.value = AudioManager.Instance.volume[(int)eMixers.music];
        SFXSlider.value = AudioManager.Instance.volume[(int)eMixers.effects];
    }
    public void OnMusicVolumeChanged(float _value)
    {
        AudioManager.Instance.SetMixerLevel(eMixers.music, _value);
    }
    public void OnEffectsVolumeChanged(float _value)
    {
        AudioManager.Instance.SetMixerLevel(eMixers.effects, _value);
    }
    public void OnReturnClicked()
    {
        AudioManager.PauseSong();
        AudioManager.PlayEffect(eEffects.click);
        Destroy(this.gameObject);
    }
}
