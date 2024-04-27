using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Menus;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private Button PlayButton;
    private Button UpgradesButton;
    private Button OptionsButton;
    private Button CreditsButton;
    private Button QuitButton;


    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get the buttons from the UI
        this.PlayButton = root.Q<Button>("PlayButton");
        this.UpgradesButton = root.Q<Button>("UpgradesButton");
        this.OptionsButton = root.Q<Button>("OptionsButton");
        this.CreditsButton = root.Q<Button>("CreditsButton");
        this.QuitButton = root.Q<Button>("QuitButton");

        //// Add the click event to the buttons
        this.PlayButton.RegisterCallback<ClickEvent, ESceneIndex>(OpenScene, ESceneIndex.Game);
        this.UpgradesButton.RegisterCallback<ClickEvent, ESceneIndex>(OpenScene, ESceneIndex.Upgrades);
        this.OptionsButton.RegisterCallback<ClickEvent, ESceneIndex>(OpenScene, ESceneIndex.Settings);
        this.CreditsButton.RegisterCallback<ClickEvent, ESceneIndex>(OpenScene, ESceneIndex.Credits);
        this.QuitButton.RegisterCallback<ClickEvent, ESceneIndex>(OpenScene, ESceneIndex.None);
    }

    private void OpenScene(ClickEvent eve, ESceneIndex sceneIndex)
    {
        if (sceneIndex == ESceneIndex.None)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

            return;
        }

        if (sceneIndex == ESceneIndex.Game)
        {
            SceneLoader.Instance.LoadSceneAsync(sceneIndex, true);
            return;
        }

        SceneLoader.Instance.LoadScene(sceneIndex);

    }
}