using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class EndGame : MonoBehaviour
{
    public GameObject Player;
    public GameObject DoorOpen;
    public GameObject CartelCerrado;
    public GameObject TriggerWin;
    public TimeGame Timer;
    public RuntimeAnimatorController LoseAnimator;
    public CinemachineVirtualCamera camCM;

    [Header("Bye")]
    public Goodbye bye;

    private PlayerController _playerController;
    private Animator _anim;
    void Start()
    {
        _anim = Player.GetComponent<Animator>();
        _playerController = Player.GetComponent<PlayerController>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (Timer.OnTime())
                Win();
            else
                Lose();
        }
    }

    private void Lose()
    {
        //_playerController.enabled = false;
        _playerController.BlockChar();
        _anim.runtimeAnimatorController = LoseAnimator;
        CartelCerrado.SetActive(true);
        DoorOpen.SetActive(false);
        TriggerWin.SetActive(false);
        DOTween.To(() => camCM.m_Lens.OrthographicSize, x => camCM.m_Lens.OrthographicSize = x, 2f, 10f).Play();
        camCM.m_LookAt = _playerController.transform;
        Invoke(nameof(ByeCall), 8f);
    }

    private void ByeCall()
    {
        bye.ByeBye();
    }

    private void Win()
    {
        CartelCerrado.SetActive(false);
        DoorOpen.SetActive(true);
        TriggerWin.SetActive(true);
    }
}
