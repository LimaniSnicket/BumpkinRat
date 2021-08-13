using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class NameSetMenu : MonoBehaviour
{
    public Button continueButton;
    public TMP_InputField nameInput;
    public Image shirtBackground;

    public SetImageButton[] setShirtBackgrounds;

    bool CanContinue => nameInput.text != string.Empty;

    private void Start()
    {
        continueButton.onClick.AddListener(ContinueToNextScene);
        InitializeAllSetImageButtons();
    }

    private void Update()
    {
        continueButton.interactable = CanContinue;
    }

    void SetShirtBackgroundImage(Sprite setButton)
    {
        if(shirtBackground != null)
        {
            shirtBackground.sprite = setButton;
        }
    }

    void ContinueToNextScene()
    {
        string playerName = nameInput.text;
        GameDataManager.SetPlayerName(playerName);
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }

    void InitializeAllSetImageButtons()
    {
        if (setShirtBackgrounds.CollectionIsNotNullOrEmpty())
        {
            for(int i = 0; i < setShirtBackgrounds.Length; i++)
            {
                SetImageButton setButton = setShirtBackgrounds[i];
                setButton.setButton.onClick.AddListener(() => SetShirtBackgroundImage(setButton.setting));
            }
        }
    }
}

[Serializable]
public class SetImageButton
{
    public Button setButton;
    public Sprite setting;
    public string spritePath;
}
