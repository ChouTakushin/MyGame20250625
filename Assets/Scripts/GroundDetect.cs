using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetect : MonoBehaviour
{
    private PlayerMovementController _player = default;
    private BoxCollider2D _boxCollider2D = default;
    private bool _isGround = false;
    public bool IsGround { get { return this._isGround; } }
    // Start is called before the first frame update
    void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _player = GetComponentInParent<PlayerMovementController>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _isGround = true;
        }
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Ground")
    //    {
    //        _isGround = true;
    //    }
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            _isGround = false;
        }
    }
}
