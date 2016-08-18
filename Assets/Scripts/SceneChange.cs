using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChange : MonoBehaviour {

	public void ChangeScene (string scene) {
        SceneManager.LoadScene(scene);
	}

    public void QuitGame()
    {
        Application.Quit();
    }
}
