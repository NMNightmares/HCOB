using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
        [SerializeField] private string gameplaySceneName = "GameScene";

        public void OnPlayClicked()
        {
            SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
        }

        public void OnQuitClicked()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }
    
}
