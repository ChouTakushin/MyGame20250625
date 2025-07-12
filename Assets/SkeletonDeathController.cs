using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDeathController : EnemyDeathBehaviourBase
{
    private SkeletonController _controller = default;
    private Animator _animator = default;
    private GameMasterController _gameMasterController = default;
    private PlayerMovementController _pMovementController = default;

    private void Start()
    {
        _controller = GetComponent<SkeletonController>();
        _animator = GetComponent<Animator>();
        _pMovementController = GameObject.Find("Player").GetComponent<PlayerMovementController>();
        _gameMasterController = GameObject.Find("GameMaster").GetComponent<GameMasterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void DoDeath()
    {
        AudioManager.Instance.PlaySE("SE_Enemy_Skeleton_Death");
        _controller.IsDead = true;
        _animator.SetTrigger("T_Death");
        _pMovementController.GainMp(_controller.KillMp);
        _gameMasterController.CountEnemyKill(1);
    }
}
