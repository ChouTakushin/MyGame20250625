using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneratorController : MonoBehaviour
{
    [SerializeField] GameObject[] _enemyPrefabs = default;
    [SerializeField] GameObject _gameMasterGo = default;
    [SerializeField] Vector3 _wizardSpawnOffset = default;
    [SerializeField] float _spawnInterval = default;
    [SerializeField] float _spawnIntervalFinalRush = default;
    [SerializeField] int _wizardIndex = default;
    private float _spawnTimer = 0;
    private bool _canSpawn = false;
    private GameMasterController _gamemasterController = default;

    public float SpawnInterval { get { return _spawnInterval; } }
    public bool CanSpawn { get { return _canSpawn; } set { _canSpawn = value; } }
    void Start()
    {
        _spawnTimer = _spawnInterval - 0.1f;
        _gamemasterController = _gameMasterGo.GetComponent<GameMasterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_canSpawn)
        {
            return;
        }

        _spawnTimer += Time.deltaTime;
        if(_spawnTimer >= _spawnInterval)
        {
            int enemyIndex = Random.Range(0, _enemyPrefabs.Length);
            if (enemyIndex != _wizardIndex)
            {
                Instantiate(_enemyPrefabs[enemyIndex], transform.position, Quaternion.identity);
            } else
            {
                float xOffset = Random.Range(-1f, 1f);
                Instantiate(_enemyPrefabs[enemyIndex], transform.position + _wizardSpawnOffset + new Vector3(xOffset, 0f, 0f), Quaternion.identity);
            }
            _spawnTimer = 0;
        }
    }
    public void SetSpawnInterval(float time)
    {
        _spawnInterval = time;
    }
    public void EnterFinalRush()
    {
        _spawnInterval = _spawnIntervalFinalRush;
    }
    public void StopSpawning()
    {
        _canSpawn = false;
    }

    //public void SpawnOneSlime()
    //{
    //    Instantiate(_enemyPrefabs[0], transform.position, Quaternion.identity);
    //}
    //public void SpawnOneSkeleton()
    //{
    //    Instantiate(_enemyPrefabs[0], transform.position, Quaternion.identity);
    //}
    //public void SpawnOneMushroom()
    //{
    //    Instantiate(_enemyPrefabs[0], transform.position, Quaternion.identity);
    //}
    //public void SpawnOneFlyEye()
    //{
    //    Instantiate(_enemyPrefabs[0], transform.position, Quaternion.identity);
    //}
    //public void SpawnOneWizard()
    //{
    //    Instantiate(_enemyPrefabs[0], transform.position + _wizardSpawnOffset, Quaternion.identity);
    //}
}
