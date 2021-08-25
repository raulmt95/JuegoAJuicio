using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject CreditsPanel;
    public GameObject SettingsPanel;

    [Header("Move Panels")]
    public float MoveTime;
    public Transform ShowPanelPos;
    public Transform HidePanelPos;

    private Vector3 hiddenPanelPos;
    private GameObject shownPanel;
    private GameObject hiddenPanel;

    private void Start()
    {
        hiddenPanelPos = CreditsPanel.transform.position;
    }

    void MoveMenuPanels(GameObject panelToShow, GameObject panelToHide)
    {
        panelToShow.transform.DOMoveY(ShowPanelPos.position.y, MoveTime).SetEase(Ease.OutQuad);
        panelToHide.transform.DOMoveY(HidePanelPos.position.y, MoveTime).SetEase(Ease.OutQuad);
        shownPanel = panelToShow;
        hiddenPanel = panelToHide;
        Invoke(nameof(SetPanelsPosition), MoveTime + 0.5f);
    }

    private void SetPanelsPosition()
    {
        hiddenPanel.transform.position = hiddenPanelPos;
    }

    #region BUTTONS

    public void OnPlayButtonPressed(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void OnCreditsButtonPressed()
    {
        MoveMenuPanels(CreditsPanel, MainPanel);
    }
    public void OnSettingsButtonPressed()
    {
        MoveMenuPanels(SettingsPanel, MainPanel);
    }
    public void OnBackButtonPressed()
    {
        MoveMenuPanels(MainPanel, shownPanel);
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }

    #endregion
}
