using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GrassGeneratorController : MonoBehaviour
{
    [SerializeField] private GameObject _grassPrefab = default;
    private float _generateInterval = 1f;
    private float _timer = 0f;
    void Start()
    {
        _timer = _generateInterval;
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _generateInterval)
        {
            Instantiate(_grassPrefab, transform.position, Quaternion.identity);
            _generateInterval = Random.Range(0.5f, 1.5f);
            _timer = 0f;
        }
    }
}
