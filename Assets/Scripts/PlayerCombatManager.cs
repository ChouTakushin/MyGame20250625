using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    [SerializeField] private int _maxHp = default;
    [SerializeField] private int _maxMp = default;
    [SerializeField] private int _initMp = default;
    [SerializeField] GameObject _gameMaster;
    private GameMasterController _gameMasterController;
    private int _hp = default;
    private int _mp = default;
    public int Hp { get { return _hp; } }
    public int Mp { get { return _mp; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int MaxMp { get { return _maxMp; } set { _maxMp = value; } }


    private CapsuleCollider2D _atkCldrG001;
    private CapsuleCollider2D _atkCldrG002;
    private CapsuleCollider2D _atkCldrA001;
    private BoxCollider2D _guardCldr;
    private PlayerMovementController _pmc;

    private int _life = default;


    void Start()
    {
        _atkCldrG001 = GameObject.Find("AtkColliderGround001").GetComponent<CapsuleCollider2D>();
        _atkCldrG002 = GameObject.Find("AtkColliderGround002").GetComponent<CapsuleCollider2D>();
        _atkCldrA001 = GameObject.Find("AtkColliderAerial001").GetComponent<CapsuleCollider2D>();
        _guardCldr = GameObject.Find("GuardCollider").GetComponent<BoxCollider2D>();
        _gameMasterController = _gameMaster.GetComponent<GameMasterController>();
        _pmc = gameObject.GetComponent<PlayerMovementController>();
        _hp = _maxHp;
        _mp = _initMp;
        _gameMasterController.SetUiHpText(_hp);
        _gameMasterController.SetUiMpText(_mp);
        _gameMasterController.SetUiECText(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateAtkCldrG001()
    {
        _atkCldrG001.enabled = true;
    }
    public void DeactivateAtkCldrG001()
    {
        _atkCldrG001.enabled = false;
    }
    public void ActivateAtkCldrG002()
    {
        _atkCldrG002.enabled = true;
    }
    public void DeactivateAtkCldrG002()
    {
        _atkCldrG002.enabled = false;
    }
    public void ActivateAtkCldrA001()
    {
        _atkCldrA001.enabled = true;
    }
    public void DeactivateAtkCldrA001()
    {
        _atkCldrA001.enabled = false;
    }
    public void ActivateGuardCldr()
    {
        _guardCldr.enabled = true;
    }
    public void DeactivateGuardCldr()
    {
        _guardCldr.enabled = false;
    }

    public void HpAlteration(int value)
    {
        //if (value > 0)
        //{
        //    // TODO 回復エフェクト
        //    _hp += value;
        //    if (_hp > _maxHp)
        //    {
        //        _hp = _maxHp;
        //    }
        //}
        if (value < 0)
        {
            if (_hp + value > 0)
            {
                if (!_gameMasterController.IsInTutorial())
                {
                    _hp += value;
                    _gameMasterController.SetUiHpText(_hp);
                }
                _pmc.DoTakeDamage();
            }
            else
            {
                _gameMasterController.SetUiHpText(0);
                StartCoroutine(_gameMasterController.DoGameOver());
            }
        }
    }
    public void MpAlteration(int value)
    {
        _mp += value;
        if (_mp > _maxMp)
        {
            _mp = _maxMp;
        }
        _gameMasterController.SetUiMpText(_mp);
        if(_mp >= _gameMasterController.FinalRushMaxMp)
        {
            _gameMasterController.EnterToFinish();
        }
    }
}
