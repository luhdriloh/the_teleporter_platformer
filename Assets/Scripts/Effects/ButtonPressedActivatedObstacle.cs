using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPressedActivatedObstacle : MonoBehaviour
{
    //public Transform _moveDirectionArrow;
    public PuzzleColor _puzzleColor;
    public Vector2 _travelDirection;
    public float _maxDistance;
    public float _maxSpeed;

    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private bool _buttonPressed;

    private void Awake()
    {
        FloorButton.AddButtonPressedListener(ButtonHandler);

        _startPosition = transform.position;
        _endPosition = _startPosition + _travelDirection * _maxDistance;
    }

    protected void FixedUpdate()
    {
        // get how far we are along to the end position
        float fractionOfJourney = ((Vector2)transform.position - _startPosition).magnitude / _maxDistance;
        bool inMiddleOfRange = ((Vector2)transform.position - _endPosition).magnitude < _maxDistance;

        if (_buttonPressed && fractionOfJourney < 1f)
        {
            Vector3 delta = _travelDirection * Time.fixedDeltaTime * _maxSpeed;
            Vector3 newPosition = (Vector2)delta + (Vector2)transform.position;
            fractionOfJourney = ((Vector2)newPosition - _startPosition).magnitude / _maxDistance;
            newPosition = fractionOfJourney > 1f ? _endPosition : (Vector2)newPosition;
            newPosition.z = transform.position.z;

            transform.position = newPosition;
        }
        else if (!_buttonPressed && fractionOfJourney > 0f && inMiddleOfRange)
        {
            Vector3 delta = -_travelDirection * Time.fixedDeltaTime * _maxSpeed;
            Vector3 newPosition = (Vector2)delta + (Vector2)transform.position;
            fractionOfJourney = ((Vector2)newPosition - _startPosition).magnitude / _maxDistance;
            newPosition = fractionOfJourney < 0f ? _endPosition : (Vector2)newPosition;
            newPosition.z = transform.position.z;

            transform.position = newPosition;
        }
    }


    private void ButtonHandler(PuzzleColor color, bool state)
    {
        if (color != _puzzleColor)
        {
            return;
        }

        _buttonPressed = state;
    }
}
