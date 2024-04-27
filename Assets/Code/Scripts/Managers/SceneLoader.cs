using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ESceneIndex
{
    InitScene = 0,
    MainMenu = 1,
    Game = 2,
    Upgrades = 3,
    GameOver = 4,
    Credits = 5,
    Settings = 6,
    Pause = 7,
    None = 8
}

namespace Code.Scripts.Menus
{


    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private ESceneIndex StartScene = ESceneIndex.MainMenu;

        [SerializeField] private GameObject LoadingScreenObject;
        [SerializeField] private Image LoadingScreenImage;
        [SerializeField] private AnimationCurve LoadingScreenCurve;
        [SerializeField] private float LoadingScreenDuration = 2.5f;

        private List<ESceneIndex> AdditiveScenes = new List<ESceneIndex>();
        private List<ESceneIndex> ActiveScenes = new List<ESceneIndex>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.LoadingScreenObject);
                DontDestroyOnLoad(this.gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            LoadScene(this.StartScene);
        }

        // Load the scene.
        public void LoadScene(ESceneIndex sceneIndex)
        {
            this.ActiveScenes.Add(this.StartScene);
            SceneManager.LoadScene(sceneIndex.ToString(), LoadSceneMode.Single);
        }
        
        // Load the scene asynchronously.
        public void LoadSceneAsync(ESceneIndex sceneIndex, bool loadingScreen = false)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)sceneIndex);

            if (!loadingScreen)
            {
                this.ActiveScenes.Add(this.StartScene);
                return;
            }

            StartCoroutine(ProgressRoutine(asyncLoad));
        }

        // Load the scene additively.
        public void LoadSceneAdditive(ESceneIndex sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex.ToString(), LoadSceneMode.Additive);
            this.AdditiveScenes.Add(sceneIndex);
        }

        // Load the scene additively asynchronously.
        private void LoadSceneAdditiveAsync(ESceneIndex sceneIndex)
        {
            SceneManager.LoadSceneAsync((int)sceneIndex, LoadSceneMode.Additive);
            this.AdditiveScenes.Add(sceneIndex);
        }
        
        private IEnumerator ProgressRoutine(AsyncOperation asyncOperation)
        {
            this.LoadingScreenObject.SetActive(true);
            this.LoadingScreenImage.fillAmount = 0;
            asyncOperation.allowSceneActivation = false;

            float counter = 0;

            while (!Mathf.Approximately(asyncOperation.progress, 0.9f) || counter < this.LoadingScreenDuration)
            {
                yield return new WaitForEndOfFrame();
                counter += Time.deltaTime;
                float progress = Mathf.Min(asyncOperation.progress /0.9f, this.LoadingScreenCurve.Evaluate(counter/ this.LoadingScreenDuration));
                this.LoadingScreenImage.fillAmount = progress;
            }

            asyncOperation.allowSceneActivation = true;
            this.LoadingScreenImage.fillAmount = 1;
            this.LoadingScreenObject.SetActive(false);
        }

    }
}