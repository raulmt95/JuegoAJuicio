using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OpenSettings : MonoBehaviour
{
    public RectTransform CreditsPanel;
    public Vector3 startScale;
    private Vector3 startPos;

    private void Awake()
    {
        startPos = transform.localPosition;
        transform.localScale = startScale;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        OpenSettingsSequence();
    }

    private void OnDisable()
    {
        transform.localScale = startScale;
        transform.localPosition = startPos;
    }

    void OpenSettingsSequence()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScaleX(1, .1f));
        seq.Append(transform.DOScaleY(1, .25f));
        seq.Join(transform.DOMoveX(CreditsPanel.position.x, .25f));
    }
}
