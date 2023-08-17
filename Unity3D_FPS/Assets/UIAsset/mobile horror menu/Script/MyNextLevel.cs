using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MyNextLevel : MonoBehaviour {

	// Use this for initialization
    public float Timer;
    public string SceneName;
  
	void Start () {
        
		
	}
	
	// Update is called once per frame
	void Update () {
        Timer -= Time.deltaTime;
        if (Timer <= 0.0f)
        {
            MyTime();
        }
		
	}






  void  MyTime()
    {
        SceneManager.LoadScene(SceneName);

    }

}
