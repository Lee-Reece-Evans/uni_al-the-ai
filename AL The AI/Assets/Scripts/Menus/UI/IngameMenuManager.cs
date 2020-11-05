using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngameMenuManager : MonoBehaviour
{
    public static IngameMenuManager instance;

    public GameObject activeUIObj;
    public GameObject pauseMenuObj;
    public GameObject shopMenuObj;
    public GameObject gameoverObj;
    public GameObject gameWonObj;
    public GameObject ratingImages;
    public GameObject cancelMenuObj;
    public GameObject turretMenuObj;
    public GameObject repairDroneMenuObj;
    public GameObject placementMenuObj;
    public GameObject menuCrosshair;

    public GameObject shopBookObj;

    public TurretUI turretUI;
    public RepairDroneUI repairDroneUI;
    public PlacementUI placementUI;
    public Shop shopUI;

    public TextMeshProUGUI rewardText;
    public Sprite fullStar;

    public bool usingMenu = false;

    private FirstPersonController fpscontroller;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        fpscontroller = GameObject.Find("FPSController").GetComponent<FirstPersonController>();
        turretUI = turretMenuObj.GetComponent<TurretUI>();
        repairDroneUI = repairDroneMenuObj.GetComponent<RepairDroneUI>();
        placementUI = placementMenuObj.GetComponent<PlacementUI>();
        shopUI = shopMenuObj.GetComponent<Shop>();
    }

    void Update()
    {
        if (!GameManager.instance.gamePaused && !GameManager.instance.gameOver) // only accept these inputs if not paused or not gameover
        {
            if (Input.GetKeyDown(KeyCode.B) && !turretMenuObj.activeSelf && !placementMenuObj.activeSelf && !repairDroneMenuObj.activeSelf)
            {
                if (!shopUI.handsFull)
                {
                    if (!shopMenuObj.activeSelf)
                        OpenShop();
                    else
                        CloseShop();
                }
                else
                {
                    if (!cancelMenuObj.activeSelf)
                        OpenCancelMenu();
                    else
                        CloseCancelMenu();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) // shortcut to close menus. // pause game
            {
                if (shopMenuObj.activeSelf)
                    CloseShop();
                else if (cancelMenuObj.activeSelf)
                    CloseCancelMenu();
                else if (turretMenuObj.activeSelf)
                    CloseTurretMenu();
                else if (placementMenuObj.activeSelf)
                    ClosePlacementMenu();
                else if (repairDroneMenuObj.activeSelf)
                    CloseRepairDroneMenu();
                else
                    OpenPauseMenu();
            }
        }
        else if (GameManager.instance.gamePaused) // unpause if in pause menu.
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ClosePauseMenu();
        }
    }

    public void DisablePlayer()
    {
        usingMenu = true;
        fpscontroller.enabled = false;
    }

    public void EnablePlayer()
    {
        usingMenu = false;
        fpscontroller.enabled = true;
    }

    private void CursorEnabled()
    {
        Cursor.lockState = CursorLockMode.None;
        menuCrosshair.SetActive(true);
    }

    private void CursorDisabled()
    {
        Cursor.lockState = CursorLockMode.Locked;
        menuCrosshair.SetActive(false);
    }

    public void OpenShop()
    {
        SFXManager2D.instance.PlayOpenIngameMenu();
        DisablePlayer();
        CursorEnabled();

        shopBookObj.SetActive(false);
        shopMenuObj.SetActive(true);
    }

    public void CloseShop()
    {
        SFXManager2D.instance.PlayCloseIngameMenu();
        EnablePlayer();
        CursorDisabled();

        shopBookObj.SetActive(true);
        shopMenuObj.SetActive(false);
    }

    public void OpenCancelMenu()
    {
        SFXManager2D.instance.PlayOpenIngameMenu();
        DisablePlayer();
        CursorEnabled();

        cancelMenuObj.SetActive(true);
    }

    public void CloseCancelMenu()
    {
        SFXManager2D.instance.PlayCloseIngameMenu();
        EnablePlayer();
        CursorDisabled();

        cancelMenuObj.SetActive(false);
    }

    public void OpenTurretMenu()
    {
        SFXManager2D.instance.PlayOpenIngameMenu();
        DisablePlayer();
        CursorEnabled();

        turretMenuObj.SetActive(true);
    }

    public void CloseTurretMenu()
    {
        SFXManager2D.instance.PlayCloseIngameMenu();
        EnablePlayer();
        CursorDisabled();

        turretMenuObj.SetActive(false);
    }

    public void OpenRepairDroneMenu()
    {
        SFXManager2D.instance.PlayOpenIngameMenu();
        DisablePlayer();
        CursorEnabled();

        repairDroneMenuObj.SetActive(true);
    }

    public void CloseRepairDroneMenu()
    {
        SFXManager2D.instance.PlayCloseIngameMenu();
        EnablePlayer();
        CursorDisabled();

        repairDroneMenuObj.SetActive(false);
    }

    public void OpenPlacementMenu()
    {
        SFXManager2D.instance.PlayOpenIngameMenu();
        DisablePlayer();
        CursorEnabled();

        placementMenuObj.SetActive(true);
    }

    public void ClosePlacementMenu()
    {
        SFXManager2D.instance.PlayCloseIngameMenu();
        EnablePlayer();
        CursorDisabled();

        placementMenuObj.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        Time.timeScale = 0f;

        SFXManager2D.instance.PlayOpenIngameMenu();
        GameManager.instance.gamePaused = true;
        activeUIObj.SetActive(false);
        pauseMenuObj.SetActive(true);

        DisablePlayer();
        CursorEnabled();
    }

    public void ClosePauseMenu()
    {
        Time.timeScale = 1f;

        SFXManager2D.instance.PlayCloseIngameMenu();
        GameManager.instance.gamePaused = false;
        activeUIObj.SetActive(true);
        pauseMenuObj.SetActive(false);

        EnablePlayer();
        CursorDisabled();
    }

    public void CloseAllActiveMenus()
    {
        shopMenuObj.SetActive(false);
        cancelMenuObj.SetActive(false);
        turretMenuObj.SetActive(false);
        placementMenuObj.SetActive(false);
        repairDroneMenuObj.SetActive(false);
    }

    public void OpenGameoverMenu()
    {
        Time.timeScale = 0f;
        CloseAllActiveMenus();

        DisablePlayer();
        CursorEnabled();
        gameoverObj.SetActive(true);
    }

    public void OpenGameWonMenu(int rating, int reward)
    {
        Time.timeScale = 0f;
        CloseAllActiveMenus();

        rewardText.text = "REWARD: $" + reward;

        for (int i = 0; i < rating; i++)
        {
            ratingImages.transform.GetChild(i).GetComponent<Image>().sprite = fullStar;
        }

        DisablePlayer();
        CursorEnabled();
        gameWonObj.SetActive(true);
    }
}
