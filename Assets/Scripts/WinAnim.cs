using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class WinAnim : MonoBehaviour
{
    public CinemachineVirtualCamera camCM;
    public GameObject Hair;
    public Animator PlayerAnim;
    public PlayerController Controller;
    public RuntimeAnimatorController WinAnimator;

    [Header("Bye")]
    public Goodbye bye;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //Controller.enabled = false;
            Controller.BlockChar();
            PlayerAnim.runtimeAnimatorController = WinAnimator;
            Hair.SetActive(false);
            //camCM.DOOrthoSize(1f, 5f);
            DOTween.To(() => camCM.m_Lens.OrthographicSize, x => camCM.m_Lens.OrthographicSize = x, 1.5f, 5f).Play();
            camCM.m_LookAt = Controller.transform;
            Invoke(nameof(ByeCall), 6f);
        }
    }

    private void ByeCall()
    {
        bye.ByeBye();
    }
}
