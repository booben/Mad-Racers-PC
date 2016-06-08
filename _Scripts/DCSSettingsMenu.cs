using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DCSSettingsMenu : MonoBehaviour
{
    public static event OnOptionChangedHandler  OnOptionsChangedEvent = delegate { };
    public List<DCSOptions>                     pages = new List<DCSOptions>();

    private void Start ()
    {
        for ( int i=0 ; i < pages.Count ; i++ )
        {
            for ( int j=0 ; j < pages[i].options.Count ; j++ )
            {
                pages[i].options[j].slider.OnOptionChangedEvent += OnOptionChangedReaction;
            }
            pages[i].page.SetActive(pages[i].toggle.isOn);

        }
    }

    private void OnOptionChangedReaction(DCSOptionGroup group, string name, float value)
    {
        if (group == DCSOptionGroup.Video)
        {
            return;
        }
        OnOptionsChangedEvent(group, name, value);
    }

    private void OnDestroy ()
    {
        for ( int i=0 ; i < pages.Count ; i++ )
        {
            for ( int j=0 ; j < pages[i].options.Count ; j++ )
            {
                pages[i].options[j].slider.OnOptionChangedEvent -= OnOptionChangedReaction;
            }
        }
    }

   
    public void ToggleChanged ( int index )
    {
        pages[index].page.SetActive( pages[index].toggle.isOn );
    }

    public DCSOption GetOption ( DCSOptionGroup group, string name )
    {
        return pages[( int )group].GetByName( name );
    }

    public void OnApply ()
    {
        int res = ( int )pages[( int )DCSOptionGroup.Video].options[0].value;
        int qa = ( int )pages[( int )DCSOptionGroup.Video].options[1].value;
        int full = ( int )pages[( int )DCSOptionGroup.Video].options[2].value;

#if !UNITY_EDITOR
        if ( Screen.fullScreen != ( full == 0 ) )
        {
            int width = 1024;
            int height = 768;
            if ( full == 0 )
            {
                Resolution resol = Screen.resolutions[Screen.resolutions.Length - 1];
                width = resol.width;
                height = resol.height;
            }
            Screen.SetResolution( width, height, full == 0 );
        }
#endif
        QualitySettings.SetQualityLevel( qa );
        OnOptionsChangedEvent( DCSOptionGroup.Video, pages[( int )DCSOptionGroup.Video].options[0].slider.playerPref, pages[( int )DCSOptionGroup.Video].options[0].value );
        OnOptionsChangedEvent( DCSOptionGroup.Video, pages[( int )DCSOptionGroup.Video].options[1].slider.playerPref, pages[( int )DCSOptionGroup.Video].options[1].value );
        OnOptionsChangedEvent( DCSOptionGroup.Video, pages[( int )DCSOptionGroup.Video].options[2].slider.playerPref, pages[( int )DCSOptionGroup.Video].options[2].value );

        for ( int i=0 ; i < pages.Count ; i++ )
        {
            for ( int j=0 ; j < pages[i].options.Count ; j++ )
            {
                pages[i].options[j].slider.Save();
            }

        }

    }

    public void OnCancel ()
    {

    }
}

public enum DCSOptionGroup
{
    Game,
    Audio,
    Video,
    Controls,
}

[System.Serializable]
public class DCSOptions
{
    public string                           name;
    public DCSOptionGroup                   group;
    public Toggle                           toggle;
    public GameObject                       page;
    public List<DCSOption>                  options;
    public Dictionary<string, DCSOption>    opts = new Dictionary<string, DCSOption>();

    public DCSOption GetByName ( string name )
    {
        return opts[name];
    }

}

[System.Serializable]
public class DCSOption
{
    public string               name;    
    public DCSSettingsSlider    slider;

    public float    value
    {
        get
        {
            return slider.slider.value;                 
        }
    }
}