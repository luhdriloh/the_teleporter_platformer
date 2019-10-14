using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EndFlagReached : MonoBehaviour
{
    private Collider2D _collider;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        _animator.SetBool("end", true);
    }
}
