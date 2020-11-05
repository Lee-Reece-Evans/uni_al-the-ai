using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameoverUI : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI restartText;
    [SerializeField] private Button leaveButton;
    [SerializeField] private TextMeshProUGUI leaveText;

    void Start()
    {
        StartCoroutine(MakeButtonsInteractable()); // delay button interaction to avoid misclicks
    }

    private IEnumerator MakeButtonsInteractable()
    {
        restartText.text = "3";
        leaveText.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        restartText.text = "2";
        leaveText.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        restartText.text = "1";
        leaveText.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        restartText.text = "RESTART";
        leaveText.text = "LEAVE GAME";
        restartButton.interactable = true;
        leaveButton.interactable = true;
    }
}
