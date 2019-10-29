using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Turn on a set of fireswitches on at a set interval
/// </summary>
public class FireSwitchSet : MonoBehaviour
{
    public float _turnOninterval;
    public float _startDelay;

    private FireSwitch[] _fireSwitches;

    private void Awake()
    {
        _fireSwitches = GetComponentsInChildren<FireSwitch>();
        StartCoroutine("FireSwitchManager");
    }


    private IEnumerator FireSwitchManager()
    {
        yield return new WaitForSeconds(_startDelay);

        while (true)
        {
            ToggleAllSwitches(true);
            yield return new WaitForSeconds(_turnOninterval);

            ToggleAllSwitches(false);
            yield return new WaitForSeconds(_turnOninterval);
        }
    }


    private void ToggleAllSwitches(bool on)
    {
        foreach (FireSwitch fireswitch in _fireSwitches)
        {
            fireswitch.ToggleSwitch(on);
        }
    }
}
