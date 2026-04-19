// =============================================================================
// AudioManager.cs — Central audio controller (Singleton)
// =============================================================================
// One manager per project. Exposes typed Play methods and mixer-backed volume
// controls. Other scripts never touch AudioSources directly for global sounds —
// they go through this manager.
//
// Mirrors the InputManager singleton pattern used elsewhere in this repo.
//
// RESPONSIBILITIES:
//   - Play music (via MusicController)
//   - Play one-shot SFX at a world position (via SFXPlayer / AudioPool)
//   - Read/write mixer volumes for settings menu
//   - Persist across scene loads (DontDestroyOnLoad)
//
// SCENE REQUIREMENTS:
//   - One GameObject in your bootstrap/first scene with this component
//   - AudioMixer asset with exposed parameters: MasterVolume, MusicVolume,
//     SFXVolume, UIVolume (all in dB, -80 to 0)
//   - AudioMixerGroups assigned for Music / SFX / UI
// =============================================================================

using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup uiGroup;

    [Header("Sub-Controllers")]
    [SerializeField] private MusicController music;
    [SerializeField] private AudioPool sfxPool;

    // Exposed-parameter names as defined on the AudioMixer asset
    private const string MasterParam = "MasterVolume";
    private const string MusicParam  = "MusicVolume";
    private const string SFXParam    = "SFXVolume";
    private const string UIParam     = "UIVolume";

    void Awake()
    {
        // Singleton guard — if a second AudioManager loads into a scene,
        // destroy it so this one stays authoritative.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // =========================================================================
    // MUSIC API
    // =========================================================================

    public void PlayMusic(AudioClip clip, float fadeSeconds = 1f)
    {
        if (music != null) music.CrossfadeTo(clip, fadeSeconds);
    }

    public void StopMusic(float fadeSeconds = 1f)
    {
        if (music != null) music.FadeOut(fadeSeconds);
    }

    public void TransitionMood(AudioMixerSnapshot snapshot, float seconds = 1.5f)
    {
        if (snapshot != null) snapshot.TransitionTo(seconds);
    }

    // =========================================================================
    // SFX API
    // =========================================================================

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        if (sfxPool != null) sfxPool.PlayAt(clip, position, volume);
    }

    public void PlayUI(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        // UI sounds are 2D — reuse the pool but force spatialBlend to 0
        if (sfxPool != null) sfxPool.Play2D(clip, volume, uiGroup);
    }

    // =========================================================================
    // VOLUME API (called by Settings menu)
    // =========================================================================
    //
    // Slider is linear 0–1. Mixer is logarithmic dB.
    // Conversion: dB = log10(linear) * 20. Clamp to avoid log(0).
    // =========================================================================

    public void SetMasterVolume(float linear01) => SetMixer(MasterParam, linear01);
    public void SetMusicVolume(float linear01)  => SetMixer(MusicParam,  linear01);
    public void SetSFXVolume(float linear01)    => SetMixer(SFXParam,    linear01);
    public void SetUIVolume(float linear01)     => SetMixer(UIParam,     linear01);

    private void SetMixer(string param, float linear01)
    {
        if (mixer == null) return;
        float clamped = Mathf.Clamp(linear01, 0.0001f, 1f);
        float dB = Mathf.Log10(clamped) * 20f;
        mixer.SetFloat(param, dB);
    }

    public float GetVolume(string exposedParam)
    {
        if (mixer == null) return 1f;
        if (mixer.GetFloat(exposedParam, out float dB))
            return Mathf.Pow(10f, dB / 20f);
        return 1f;
    }
}

// =============================================================================
// INSPECTOR / PROJECT SETUP:
//
// 1. Create AudioMixer asset:
//    Project > Create > Audio Mixer  → "MainMixer"
//
// 2. Add child groups: Music, SFX, UI  (right-click Master → Add child group)
//
// 3. Expose volume parameters:
//    - Select Master group → right-click Volume → Expose to script
//    - Top-right of Mixer window: rename to "MasterVolume"
//    - Repeat for Music/SFX/UI → "MusicVolume", "SFXVolume", "UIVolume"
//
// 4. Create a bootstrap scene (or on your first scene):
//    - Empty GameObject named "AudioManager"
//    - Add AudioManager.cs
//    - Assign Mixer + groups in Inspector
//    - Add MusicController.cs + AudioPool.cs on the same or child GameObjects
//    - Wire references in the AudioManager's Inspector
//
// 5. In Settings menu UI:
//    - OnSliderChanged(float v) → AudioManager.Instance.SetMusicVolume(v);
//
// =============================================================================
