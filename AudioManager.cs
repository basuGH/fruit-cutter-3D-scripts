using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance {  get { return _instance; } }
    public AudioSource AudioSource { get; set; }
    public AudioSource BladeAudio { get; set; }
    [SerializeField] private AudioClip _throwAudio, _explosionAudio;
    [SerializeField] private AudioClip[] _targetSliced;
    [SerializeField] private AudioClip[] _comboAudio;
    [SerializeField] private AudioClip _targetMissIndicator;
    [SerializeField] private AudioClip _backgroundMusic;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        AudioSource = GetComponent<AudioSource>();
        BladeAudio = transform.GetChild(0).GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    public void PlayBackgroundMusic()
    {
        if (!AudioSource.isPlaying)
        {
            AudioSource.clip = _backgroundMusic;
            AudioSource.Play();
            AudioSource.loop = true;
        }
    }
    public void ThrowAudio()
    {
        AudioSource.PlayOneShot(_throwAudio);
    }
    public void TargetSlicedAudio()
    {
        int index = Random.Range(0, _targetSliced.Length);
        AudioSource.PlayOneShot(_targetSliced[index]);
    }
    public void ComboAudio()
    {
        int index = Random.Range(0, _comboAudio.Length);
        BladeAudio.PlayOneShot(_comboAudio[index]);
    }
    public void ExplosionAudio()
    {
        AudioSource.PlayOneShot(_explosionAudio);
    }
    public void TargetMissAudio()
    {
        AudioSource.PlayOneShot(_targetMissIndicator, AudioSource.volume * 0.15f);
    }
    public void PlayBladeAudio(AudioClip audio)
    {
        if (!BladeAudio.isPlaying)
        {
            BladeAudio.clip = audio;
            BladeAudio.Play();
        }
    }

}
