using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionEffect : MonoBehaviour
{
    public float _transitionTime = 5f;

    public float _minTransformScaleValue;
    public float _maxTransformScaleValue;

    private Transform[] _transitionSquares;
    private float _timePassed;


    private void Start()
    {
        _transitionSquares = GetComponentsInChildren<Transform>();
        SetUpTransition();

        _timePassed = 0f;
        StartCoroutine("SquareAnimation", false);
    }

    private void SetUpTransition()
    {
        foreach (Transform squareTransform in _transitionSquares)
        {
            if (squareTransform == transform)
            {
                continue;
            }

            squareTransform.localScale = new Vector3(_maxTransformScaleValue, _maxTransformScaleValue, 0f);
        }
    }

    private IEnumerator SquareAnimation(bool forward)
    {
        float min = forward ? _minTransformScaleValue : _maxTransformScaleValue;
        float max = forward ? _maxTransformScaleValue : _minTransformScaleValue; 

        float rotationAmount = (360f / _transitionTime) * .1f;
        yield return new WaitForSeconds(1f);

        while (_timePassed <= _transitionTime)
        { 
            foreach (Transform squareTransform in _transitionSquares)
            {
                if (squareTransform == transform)
                {
                    continue;
                }

                float timePassedPercent = Mathf.Min(_timePassed / _transitionTime, 1f);

                float newScaleValue = Mathf.Lerp(min, max, timePassedPercent);
                squareTransform.localScale = new Vector3(newScaleValue, newScaleValue, 0f);
                squareTransform.rotation = Quaternion.AngleAxis(rotationAmount, Vector3.forward) * squareTransform.rotation;
            }

            yield return new WaitForSeconds(.1f);
            _timePassed += .1f;
        }
    }


    public void EndLevel()
    {
        _timePassed = 0f;
        StartCoroutine("SquareAnimation", true);
    }
}
