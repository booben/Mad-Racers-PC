using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DCSSettingsSlider : MonoBehaviour
{
    public event OnOptionChangedHandler OnOptionChangedEvent = delegate { };
    public DCSOptionGroup               group;
    public string                       playerPref = "";
    public float                        defaultValue = 0.5f;
    public Slider                       slider;
    public Text                         value;
    public Text                         min;
    public Text                         max;

    public string[]                     values;
    public string                       postfix = "%";

    void Awake ()
    {        
#if !UNITY_EDITOR
        if ( playerPref == "Resolution" )
        {
            values = new string[Screen.resolutions.Length];
            for ( int i=0 ; i < Screen.resolutions.Length ; i++ )
            {
                values[i] = Screen.resolutions[i].width + "x" + Screen.resolutions[i].height;
            }
            min.text = values[0];
            max.text = values[values.Length - 1];
            slider.maxValue = Screen.resolutions.Length-1;
        }
#endif
        slider.value = PlayerPrefs.GetFloat( playerPref, playerPref == "Resolution" ? slider.maxValue : defaultValue );      
        if ( playerPref == "ControlMode" )
        {
            ZRUIDispatcher.SetControl( ( ZRControlMode )slider.value );
        }
        OnSliderChanged();
    }

    public void OnSliderChanged ()
    {
        string tmp = "";

        if ( values.Length > 1 )
        {
            int val = ( int )slider.value;
            if ( values.Length > val )
            {
                tmp = values[val].Translate();
            }
        }
        else
        {
            tmp = Mathf.Round( slider.value * 100 ) + postfix;
        }
        value.text = tmp;
        Save();
        if ( playerPref == "ControlMode" )
        {
            ZRUIDispatcher.SetControl( (ZRControlMode)slider.value );
        }
        OnOptionChangedEvent( group, playerPref, slider.value );        
    }

    public void Save ()
    {
        PlayerPrefs.SetFloat( playerPref, slider.value );
    }
}
