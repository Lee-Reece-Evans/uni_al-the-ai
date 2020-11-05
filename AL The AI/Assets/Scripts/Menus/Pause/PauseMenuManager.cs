using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    private Animator anim;
    private string menuName;
    public SceneLoader sceneLoader;
    public GameObject[] menus;

    public Vector3 startingPos;

    private void Start()
    {
        anim = GetComponent<Animator>();
        startingPos = transform.localPosition;
    }

    private void OnDisable()
    {
        transform.localPosition = startingPos; // reset starting position of menu, incase disabled when animating and the position is incorrect.
    }

    public void Resume()
    {
        SFXManager2D.instance.PlayStandardButtonSFX();
        IngameMenuManager.instance.ClosePauseMenu();
    }

    public void Restart()
    {
        SFXManager2D.instance.PlayStandardButtonSFX();
        Time.timeScale = 1f;
        sceneLoader.StartLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void Leave()
    {
        SFXManager2D.instance.PlayStandardButtonSFX();
        Time.timeScale = 1f;
        sceneLoader.StartLevel(0);
    }

    public void MenuTransition(string menuToOpen)
    {
        SFXManager2D.instance.PlayStandardButtonSFX();
        anim.SetTrigger("ChangeMenu");
        menuName = menuToOpen;
    }

    public void EnableMenu()
    {
        foreach (GameObject menu in menus)
        {
            if (menu.name != menuName && menu.activeSelf)
                menu.SetActive(false);
            else if (menu.name == menuName && !menu.activeSelf)
                menu.SetActive(true);
        }
    }
}
