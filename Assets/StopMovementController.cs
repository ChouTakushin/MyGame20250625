using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StopMovementController : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopMovement()
    {
        _animator.speed = 0f;
        _rb.velocity = Vector3.zero;
    }
}
