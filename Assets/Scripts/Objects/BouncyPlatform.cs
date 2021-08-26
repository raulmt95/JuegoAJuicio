using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BouncyPlatform : MonoBehaviour
{
    public float MaxDesplazamiento = 0.3f;
    public float Cooldown = 1f;
    public bool BounceBack = true;

    private bool _cooldown = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && !_cooldown)
        {
            float disp = Mathf.Lerp(0, MaxDesplazamiento, -collision.relativeVelocity.y / 10f);
            if(BounceBack)
                transform.DOMoveY(transform.position.y - disp, Cooldown * 0.25f).SetLoops(2, LoopType.Yoyo).Play();
            else
                transform.DOMoveY(transform.position.y - disp, Cooldown * 0.9f).Play();
            _cooldown = true;
            Invoke(nameof(RestoreCooldown), Cooldown);
        }
    }

    private void RestoreCooldown()
    {
        _cooldown = false;
    }
}
