using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door : MonoBehaviour
{
    public PuzzleColor _doorColor;
    public Vector2 _directionToMove;
    public float _speed;

    private float _height;
    private Vector2 _moveStart;
    private Vector2 _moveEnd;
    private float _startTime;
    private bool _buttonPressed = false;

    private void Awake()
    {
        Button.AddButtonPressedListener(ButtonPressedHandler);
        _height = GetComponent<SpriteRenderer>().bounds.size.y - .6f;
        _moveStart = transform.position;
        _moveEnd = _moveStart + _directionToMove.normalized * _height;
    }


    private void ButtonPressedHandler(PuzzleColor color)
    {
        if (color != _doorColor)
        {
            return;
        }

        _buttonPressed = true;
    }

    private void Update()
    {
        if (_buttonPressed)
        {
            // Distance moved equals elapsed time times speed.
            Vector3 newPosition = _directionToMove * Time.deltaTime * _speed + (Vector2)transform.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = (_moveStart - (Vector2)transform.position).magnitude / _height;


            if (fractionOfJourney >= 1)
            {
                _directionToMove *= -1;
                _moveEnd = _moveStart;
                _moveStart = transform.position;
                _buttonPressed = false;
            }
        }
    }
}
