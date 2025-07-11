using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GrassController : MonoBehaviour
{
    [SerializeField] private float _speedScale;
    private float _destoryTime = 15f;
    private float _destroyTimer = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _destroyTimer += Time.deltaTime;
        transform.Translate(Vector3.right * _speedScale * Time.deltaTime);
        if(_destroyTimer >= _destoryTime)
        {
            Destroy(gameObject);
        }
    }
}
