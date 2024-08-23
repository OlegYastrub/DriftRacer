using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
    public string SceneTuning;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void ClickLoadScene()
    {
        SceneManager.LoadScene(SceneTuning);
       
    }
    public void Exit()
    {
        #if UNITY_EDITOR
                
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                   
                    Application.Quit();
        #endif 
    }
}
