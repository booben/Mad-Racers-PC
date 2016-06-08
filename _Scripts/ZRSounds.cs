using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sound manager
/// </summary>
public class ZRSounds : MonoBehaviour
{
    public static ZRSounds                  instance;
    public List<ZRClipSettings>             music = new List<ZRClipSettings>();
    public List<ZRClipReference>            references = new List<ZRClipReference>();
    public Dictionary<string, AudioClip[]>  refs = new Dictionary<string, AudioClip[]>();
    public AudioSource                      source;

    private AudioSource                     dynamicSource;
    private Dictionary<string, float>       volumes = new Dictionary<string, float>();

    private void Awake()
    {
        instance = this;
        if ( !volumes.ContainsKey( "Common" ) )
        {
            volumes.Add( "Common", PlayerPrefs.GetFloat( "Common", 0.5f ) );
            volumes.Add( "Music", PlayerPrefs.GetFloat( "Music", 0.5f ) );
            volumes.Add( "Sounds", PlayerPrefs.GetFloat( "Sounds", 0.5f ) );
        }
        
        DCSSettingsMenu.OnOptionsChangedEvent += OnOptionChangedReaction;
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        source.volume = (focusStatus ? volumes["Common"] : 0) * volumes["Music"];
        if (dynamicSource != null)
        {
            dynamicSource.volume = (focusStatus ? volumes["Common"] : 0) * volumes["Sounds"];
        }
    }

    private void OnDestroy ()
    {
        DCSSettingsMenu.OnOptionsChangedEvent -= OnOptionChangedReaction;
    }

    /// <summary>
    /// Reset sound volumes
    /// </summary>
    /// <param name="name"></param>
    private void ResetVolumes (string name)
    {
        switch ( name )
        {
            case "Common":
                source.volume = volumes["Common"] * volumes["Music"];
                if ( dynamicSource != null )
                {
                    dynamicSource.volume = volumes["Common"] * volumes["Sounds"];
                }
                break;
            case "Music":
                source.volume = volumes["Common"] * volumes["Music"];
                break;
            case "Sounds":
                if ( dynamicSource != null )
                {
                    dynamicSource.volume = volumes["Common"] * volumes["Sounds"];
                }
                break;
        }        
    }


    private void Start()
    {
        Init();
        ResetVolumes( "Common" );
    }

    private void OnOptionChangedReaction (DCSOptionGroup group, string name, float value)
    {
        if ( group != DCSOptionGroup.Audio )
        {
            return;
        }
        if ( !volumes.ContainsKey( name ) )
        {
            return;
        }
        volumes[name] = value;
        ResetVolumes( name );
    }

    private void Init()
    {
        for (int i = 0; i < references.Count; i++)
        {
            if (refs.ContainsKey(references[i].name))
            {
                continue;
            }
            refs.Add(references[i].name, references[i].clips);
        }
    }

    public static void Play(AudioSource audio, string sound)
    {
        if (sound == "")
        {
            return;
        }
        if (!audio.isPlaying)
        {
            audio.clip = GetRef(sound);
            audio.Play();
        }
    }

    public static void PlaySelf(string sound)
    {
        if (sound == "")
        {
            return;
        }
        instance.GetComponent<AudioSource>().PlayOneShot(GetRef(sound));
    }

    public static void PlayForce(AudioSource audio, string sound)
    {
        if (sound == "")
        {
            return;
        }
        audio.volume = instance.volumes["Common"] * instance.volumes["Sounds"];
        audio.PlayOneShot(GetRef(sound));
    }


    public static void Play(string sound, Vector3 position)
    {
        if (sound == "")
        {
            return;
        }
        if (instance.dynamicSource == null)
        {
            instance.dynamicSource = (new GameObject("DynamicAudioSource")).AddComponent<AudioSource>();
            instance.dynamicSource.rolloffMode = AudioRolloffMode.Linear;
            instance.ResetVolumes( "Sounds" );
        }
        AudioSource audio = instance.dynamicSource;        
        if (!audio.isPlaying)
        {
            audio.clip = GetRef(sound);
            audio.Play();
        }
    }

    public static AudioClip GetRef(string name)
    {
        return GetRefVar(name, -1);
    }

    public static AudioClip GetRefVar(string name, int variant)
    {
        AudioClip[] clips = instance.refs[name];
        if (variant == -1)
        {
            variant = Random.Range(0, clips.Length);
        }
        return clips[variant];
    }

    // Random music
    private void Update()
    {        
        if (source == null)
        {
            return;
        }
        if (source.isPlaying)
        {
            return;
        }
        if (music.Count == 0)
        {
            return;
        }
        source.clip = refs[music.RandomIndex<ZRClipSettings>().name][0];
        source.Play();
    }
}

public enum ZRLevel
{
    Login,
    Hangar,
    Queue,
    Tutorial,
    Desert,

}



[System.Serializable]
public class ZRClipSettings
{
    public string name;    
    [Range(0f, 1f)]
    public float probability = 1;
    public List<ZRLevel> levels = new List<ZRLevel>();
}

[System.Serializable]
public class ZRClipReference
{
    public string name = "Song01";
    public AudioClip[] clips;
}