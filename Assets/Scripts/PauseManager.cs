using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public KeyCode PauseKey;

    public GameObject PausePanel;
    public GameObject ExitConfirmationPanel;

    public float PopupAnimationDuration;

    public int MenuSceneIndex;

    private bool _isGamePaused;

    private void Start()
    {
        PausePanel.transform.localScale = Vector3.zero;
        ExitConfirmationPanel.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetKeyDown(PauseKey) && !_isGamePaused) { OnPauseButtonPressed(); }
    }

    void SetPanelScale(GameObject panel, Vector3 scale)
    {
        panel.transform.DOScale(scale, PopupAnimationDuration).SetUpdate(true);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeScale">0 to pause, 1 to resume</param>
    public void SetGameTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    #region BUTTONS

    public void OnPauseButtonPressed()
    {
        SetGameTimeScale(0);
        SetPanelScale(PausePanel, Vector3.one);
        _isGamePaused = true;
    }

    public void OnResumeGameButtonPressed()
    {
        SetPanelScale(PausePanel, Vector3.zero);
        SetGameTimeScale(1);
        _isGamePaused = false;
    }

    public void OnExitButtonPressed()
    {
        SetPanelScale(ExitConfirmationPanel, Vector3.one);
    }

    public void OnExitConfirmed()
    {
        LoadSceneByIndex(MenuSceneIndex);
        SetGameTimeScale(1);
    }

    public void OnExitRejected()
    {
        SetPanelScale(ExitConfirmationPanel, Vector3.zero);
    }

    #endregion
}
