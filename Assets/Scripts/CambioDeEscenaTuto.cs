using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeEscenaTuto : MonoBehaviour
{
    public Animator Anim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            DoOutAnimation();
        }
    }

    public void DoOutAnimation()
    {
        Anim.SetTrigger("Out");
        Invoke(nameof(ChangeScene), 0.5f);
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
