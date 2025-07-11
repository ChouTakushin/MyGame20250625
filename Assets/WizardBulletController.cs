using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardBulletController : MonoBehaviour
{
    [SerializeField] private GameObject _burstFxPrefab = default;
    [SerializeField] private int _blockMp = 0;
    [SerializeField] private float _SelfDestuctTime = 4f;
    private Vector3 _velocity = Vector3.right;
    private float _speedScale = 0;
    private Vector3 _vXFlip = Vector3.right;
    private float _selfDestuctTimer = 0;

    public Vector2 Velocity {get{return _velocity;} set { _velocity = value; } }
    public float SpeedScale { get { return _speedScale; } set { _speedScale = value; } }
    public Vector3 VXFlip { get { return _vXFlip; } set { _vXFlip = value; } }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _selfDestuctTimer += Time.deltaTime;
        if ( _selfDestuctTimer > _SelfDestuctTime)
        {
            DestroyThis();
        }
    }
    private void FixedUpdate()
    {
        transform.position += _velocity;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("PlayerGuardCldr"))
        {
            collider.GetComponentInParent<PlayerMovementController>().GuardHit();
            collider.GetComponentInParent<PlayerMovementController>().GainMp(_blockMp);
            Instantiate(_burstFxPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if(collider.gameObject.CompareTag("Player"))
        {
            collider.gameObject.GetComponent<PlayerMovementController>().TakeDamage(1);
            Instantiate(_burstFxPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void InitOnCast(Vector3 xFlip, Vector3 velocity)
    {
        transform.localScale = xFlip;
        _velocity = velocity;
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
