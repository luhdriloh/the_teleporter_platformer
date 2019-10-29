using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSwitchController : MonoBehaviour
{
    public float _timeForSwitch;

    private List<FireSwitch> _fireSwitches;
    private int _forwardIndex;
    private int _backwardsIndex;
    private int _switchCount;
    private int i;
    private void Start()
    {
        _fireSwitches = new List<FireSwitch>(GetComponentsInChildren<FireSwitch>());
        _forwardIndex = 0;
        _backwardsIndex = _fireSwitches.Count - 1;
        _switchCount = _fireSwitches.Count;
        StartCoroutine("StartFireSwitches");
    }

    private IEnumerator StartFireSwitches()
    {
        // turn on the first fire switches
        _fireSwitches[_forwardIndex].ToggleSwitch(true);
        _fireSwitches[_backwardsIndex].ToggleSwitch(true);

        while (true)
        {
            yield return new WaitForSeconds(_timeForSwitch);

            _fireSwitches[_forwardIndex].ToggleSwitch(false);
            _fireSwitches[_backwardsIndex].ToggleSwitch(false);

            if (_forwardIndex >= _fireSwitches.Count - 1)
            {
                _forwardIndex = 0;
                _backwardsIndex = _fireSwitches.Count - 1;
            }

            _forwardIndex++;
            _backwardsIndex--;

            _fireSwitches[_forwardIndex].ToggleSwitch(true);
            _fireSwitches[_backwardsIndex].ToggleSwitch(true);
        }
    }
}
