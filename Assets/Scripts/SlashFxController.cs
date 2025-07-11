using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashFxController : MonoBehaviour
{
    // Start is called before the first frame update
    private float _flatRotateDegree = 19f;
    void Start()
    {
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + _flatRotateDegree + Random.Range(-30f, 30f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
