using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    #region BUTTONS

    public void OnPlayButtonPressed(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }

    #endregion
}
