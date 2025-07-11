using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyControllerBase : MonoBehaviour
{
    [SerializeField] private int _hp;
    [SerializeField] private int _killScore = default;
    [SerializeField] private int _killMp = default;
    [SerializeField] private Material _origMaterial = default;
    [SerializeField] private Material _whiteMaterial = default;
    [SerializeField] private GameMasterController _GameMasterController;
    [SerializeField] private GameObject _slashFx = default;
    [SerializeField] private Vector3 _vSlashFxOffset = default;

    private Rigidbody2D _rb;

    private GameObject _PlayerGo;
    private PlayerMovementController _pMovementController;
    private bool _isDead = false;

    private Vector3 _vFlipX = new Vector3(-1f, 1f, 1f);
}
