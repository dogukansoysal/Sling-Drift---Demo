using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameConstants;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    
    [Header("UI Panels")] public List<GameObject> Panels;
    
    //public Image CompleteImage;
    public Image LogoImage;
    

    // Main Menu
    public GameObject TopToPlayTextMenu;
    public GameObject LevelTextMenu;

    // In Game
    public GameObject LevelTextInGame;
    public GameObject AnimationNameTextInGame;
    
    // Success
    public GameObject LevelTextSuccess;
    public GameObject NextLevelTextSuccess;

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;
    }
    
    public void Start()
    {
        HandleScaleTween(TopToPlayTextMenu.GetComponent<RectTransform>());
        
        HandleScaleTween(NextLevelTextSuccess.GetComponent<RectTransform>());
        
        SetLevelText(LevelTextMenu.GetComponent<TextMeshProUGUI>());
        SetLevelText(LevelTextInGame.GetComponent<TextMeshProUGUI>());
        SetLevelText(LevelTextSuccess.GetComponent<TextMeshProUGUI>());

        ShowPanel(0);
    }
    
    
    /// <summary>
    /// General function for User Interface panels
    /// </summary>
    /// <param name="panelIndex"></param>
    public void ShowPanel(int panelIndex)
    {
        for (var i = 0; i < Panels.Count; i++)
        {
            Panels[i].SetActive(i == panelIndex);
        }
    }
    
    public void RestartButtonClick()
    {
        GameManager.Instance.RestartScene();
    }

    public void PlayButtonClick()
    {
        GameManager.Instance.Play();
    }

    public void NextLevelButtonClick()
    {
        GameManager.Instance.NextScene();
    }

    private void SetLevelText(Component component)
    {
        if (component == null) return;
        
        var levelIndex = GameManager.Instance.GetCurrentLevelIndex();
        if (levelIndex == 0)
        {
            component.GetComponent<TextMeshProUGUI>().text = "TUTORIAL";

        }
        else
        {
            component.GetComponent<TextMeshProUGUI>().text = "LEVEL " + levelIndex;
        }
    }


    /*
    public void ShowAnimationNamePanel()
    {
        StartCoroutine(ShowHideAnimationNamePanel());
    }


    public IEnumerator ShowHideAnimationNamePanel()
    {
        AnimationNamePanel.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        AnimationNamePanel.SetActive(false);
    }
    */    
}
