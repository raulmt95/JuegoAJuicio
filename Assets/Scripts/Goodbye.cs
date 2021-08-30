using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Goodbye : MonoBehaviour
{
    public GameObject TheEndText;
    public GameObject Buttons;
    public Volume v;

    public float Duration = 2f;
    private Vignette vig;

    // Start is called before the first frame update
    void Start()
    {
        v.profile.TryGet(out vig);
        Buttons.SetActive(false);
    }

    public void ByeBye()
    {
        TheEndText.transform.DOScale(1, Duration);
        DOTween.To(() => vig.intensity.value, x => vig.intensity.value = x, 1f, Duration).Play();
        Invoke(nameof(ActiveButtons), Duration);
    }

    private void ActiveButtons()
    {
        Buttons.SetActive(true);
    }

    public void Repetir()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
