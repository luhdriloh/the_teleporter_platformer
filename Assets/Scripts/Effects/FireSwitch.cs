using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FireSwitch : MonoBehaviour
{
    private Animator _animator;
    private Collider2D _collider;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }

    public void ToggleSwitch(bool on)
    {
        _animator.SetBool("fireOn", on);
        _collider.enabled = on;
    }
}
