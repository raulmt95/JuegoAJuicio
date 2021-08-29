using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject ChispitasPS;

    private void Start()
    {
        InvokeRepeating(nameof(Chispas), 0f, Random.Range(5f, 15f));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hair"))
        {
            collision.GetComponent<Hair>().TrapHair(true);
        }
    }


    private void Chispas()
    {
        if (ChispitasPS)
        {
            if (Random.value > 0.9)
                Instantiate(ChispitasPS, transform.position, ChispitasPS.transform.rotation);
        }
    }
}
