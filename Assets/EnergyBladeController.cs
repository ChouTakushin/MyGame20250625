using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBladeController : MonoBehaviour
{
    [SerializeField] float _speedScale = 5;
    private Vector3 _vFlipX = new Vector3(-1f, 1f, 1f);
    private Vector2 _velocity = Vector2.right;
    private int _hitDamage = 3;

    public int HitDamage {  get { return _hitDamage; } set { _hitDamage = value; } }
    void Start()
    {
        transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        _velocity *= _speedScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.Translate(_velocity * Time.deltaTime);
    }
}
