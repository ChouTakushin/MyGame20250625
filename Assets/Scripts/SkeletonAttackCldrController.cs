using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttackCldrController : MonoBehaviour
{
    [SerializeField] int _guardMp = default;
    SkeletonController _parentController = default;
    GameObject _playerGo = default;
    PlayerMovementController _playerMovementController = default;
    PlayerCombatManager _playerCombatManager = default;
    // Start is called before the first frame update
    void Start()
    {
        _parentController = GetComponentInParent<SkeletonController>();
        _playerGo = GameObject.Find("Player");
        _playerMovementController = _playerGo.GetComponent<PlayerMovementController>();
        _playerCombatManager = _playerGo.GetComponent<PlayerCombatManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerGuardCldr"))
        {
            _parentController.AttackRepelled();
            _playerMovementController.GuardHit();
            _playerMovementController.GainMp(_guardMp);
        }
        if (collision.gameObject.CompareTag("Player") && !_playerMovementController.CheckGuaring())
        {
            _playerMovementController.TakeDamage(1);
        }
    }
}
