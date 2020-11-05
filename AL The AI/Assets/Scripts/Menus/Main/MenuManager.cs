using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private string menuName;
    public GameObject[] menus;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        menuName = "Main";
        EnableMenu();
    }

    public void Exit()
    {
        Application.Quit();
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
