using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlimeDeathController : EnemyDeathBehaviourBase
{

    private SlimeController _controller = default;
    private Animator _animator = default;
    private GameMasterController _gameMasterController = default;
    private PlayerMovementController _pMovementController = default;

    private void Start()
    {
        _controller = GetComponent<SlimeController>();
        _animator = GetComponent<Animator>();
        _pMovementController = GameObject.Find("Player").GetComponent<PlayerMovementController>();
        _gameMasterController = GameObject.Find("GameMaster").GetComponent<GameMasterController>();
    }
    public override void DoDeath()
    {
        _animator.SetTrigger("T_Death");
        _pMovementController.GainMp(_controller.KillMp);
        _gameMasterController.CountEnemyKill(1);
    }
}
