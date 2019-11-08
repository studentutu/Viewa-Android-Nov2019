using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {

    class ClipInfo
    {
        //ClipInfo used to maintain default Audio Source Info
        public AudioSource source { get; set; }
        public float defaultVolume { get; set; }
    }
    private List<ClipInfo> m_activeAudio;
    private AudioSource m_activeMusic;
    private AudioSource m_activeVoiceOver;
    private float m_volumeMod, m_volumeMin;
    private bool m_VOfade;  //Used to fade to quiet for VO

    void Awake()
    {
        Debug.Log("AudioManager Initializing");
        try
        {
            transform.parent = (Transform)GameObject.FindGameObjectWithTag("MainCamera").transform;
            transform.localPosition = new Vector3(0, 0, 0);
        }
        catch
        {
            Debug.Log("Unable to find main camera to put AudioManger");
        }
        m_activeAudio = new List<ClipInfo>();
        m_volumeMod = 1.0f;
        m_volumeMin = 0.2f;
        m_VOfade = false;
        m_activeVoiceOver = null;
        m_activeMusic = null;
    }
    public AudioSource PlayVoiceOver(AudioClip voiceOver, float volume)
    {
        AudioSource source = Play(voiceOver, transform, volume);
        m_activeVoiceOver = source;
        m_volumeMod = 0.2f;
        return source;
    }
    public AudioSource Play(AudioClip clip, Vector3 soundOrigin, float volume)
    {
        //Create an empty Gameobject
        GameObject soundLoc = new GameObject("Audio: " + clip.name);
        soundLoc.transform.position = soundOrigin;

        //Create the source
        AudioSource source = soundLoc.AddComponent<AudioSource>();
        SetSource(source, clip, volume);
        source.Play();
        Destroy(soundLoc, clip.length);

        //Set the source as Active
        m_activeAudio.Add(new ClipInfo { source = source, defaultVolume = volume });
        return source;
    }
    public AudioSource Play(AudioClip clip, Transform emitter, float volume)
    {
        var source = Play(clip, emitter.position, volume);
        source.transform.parent = emitter;
        return source;
    }
    public AudioSource PlayLoop(AudioClip loop, Transform emitter, float volume)
    {
        //Create an empty GameObject
        GameObject movingSoundLoc = new GameObject("Audio: " + loop.name);
        movingSoundLoc.transform.position = emitter.position;
        movingSoundLoc.transform.parent = emitter;
        //create the source
        AudioSource source = movingSoundLoc.AddComponent<AudioSource>();
        SetSource(source, loop, volume);
        source.loop = true;
        source.Play();
        //set the source as active
        m_activeAudio.Add(new ClipInfo { source = source, defaultVolume = volume });
        return source;
    }
    public void StopSound(AudioSource toStop)
    {
        try
        {
            Destroy(m_activeAudio.Find(s => s.source == toStop).source.gameObject);
        }
        catch
        {
            Debug.Log("Error trying to stop audio source" + toStop);
        }
    }

    void SetSource(AudioSource source, AudioClip clip, float volume)
    {
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.dopplerLevel = 0.2f;
        source.minDistance = 150;
        source.maxDistance = 1500;
        source.clip = clip;
        source.volume = volume;
    }
    private void UpdateActiveAudio()
    {
        var toRemove = new List<ClipInfo>();
        try
        {
            if (!m_activeVoiceOver)
            {
                m_volumeMod = 1.0f;
            }
            foreach (var audioClip in m_activeAudio)
            {
                if (!audioClip.source)
                {
                    toRemove.Add(audioClip);
                }
                else if (audioClip.source != m_activeVoiceOver)
                {
                    audioClip.source.volume = audioClip.defaultVolume * m_volumeMod;
                }
            }
        }
        catch
        {
            Debug.Log("Error updating active audio clips");
        }
        //cleanup
        foreach (var audioClip in toRemove)
        {
            m_activeAudio.Remove(audioClip);
        }
    }

    void Update()
    {
        //fade volume for VO
        if (m_VOfade && m_volumeMod >= m_volumeMin)
        {
            m_volumeMod -= 0.1f;
        }
        else if (!m_VOfade && m_volumeMod < 1.0f)
        {
            m_volumeMod += 0.1f;
        }
        UpdateActiveAudio();
    }
    public void PauseFX()
    {
        foreach (var audioClip in m_activeAudio)
        {
            try
            {
                if (audioClip.source != m_activeMusic)
                {
                    audioClip.source.Pause();
                }
            }
            catch
            {
                continue;
            }
        }
    }
    public void UnPauseFX()
    {
        foreach (var audioClip in m_activeAudio)
        {
            try
            {
                if (!audioClip.source.isPlaying)
                {
                    audioClip.source.Play();
                }
            }
            catch
            {
                continue;
            }
        }
    }
}
