using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEndSceneController : MonoBehaviour
{
    [SerializeField] GameObject _chestGo;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChestUp()
    {
        _chestGo.transform.Translate(Vector2.up * 0.0625f);
    }
    public void ChestDown()
    {
        _chestGo.transform.Translate(Vector2.down * 0.0625f);
    }
}
