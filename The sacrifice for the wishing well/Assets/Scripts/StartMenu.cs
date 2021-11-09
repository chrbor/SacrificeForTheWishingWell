using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using static LoadSave;

public class StartMenu : MonoBehaviour
{

    EventSystem evSystem;
    Controls controls;

    private void Start()
    {
        controls = new Controls();
        controls.GameControl.Enable();

        evSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        if (!SaveFileExists()) transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
        StartCoroutine(ShowMenu());
    }

    IEnumerator ShowMenu()
    {
        CanvasGroup group = transform.GetChild(1).GetComponent<CanvasGroup>();
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            group.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        group.gameObject.SetActive(false);
        yield break;
    }

    public void Quit() => Application.Quit(0);
    public void StartNew() { ResetGame(); Continue(); }
    public void Continue() => StartCoroutine(GotoNewScene((SceneManager.sceneCountInBuildSettings - 2)));
    public void ShowCredits() => StartCoroutine(GotoNewScene((SceneManager.sceneCountInBuildSettings - 1)));
    public void ShowControls() => StartCoroutine(ShowingControls());

    IEnumerator ShowingControls()
    {
        evSystem.enabled = false;
        CanvasGroup controlDisplay = transform.GetChild(0).GetChild(2).GetComponent<CanvasGroup>();
        controlDisplay.gameObject.SetActive(true);
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            controlDisplay.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        controlDisplay.alpha = 1;

        yield return new WaitWhile(() => Input.anyKey);
        yield return new WaitUntil(() => Input.anyKey);

        for (float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            controlDisplay.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        controlDisplay.gameObject.SetActive(false);

        evSystem.enabled = true;
        yield break;
    }
    IEnumerator GotoNewScene(int sceneIndex)
    {
        CanvasGroup group = transform.GetChild(1).GetComponent<CanvasGroup>();
        group.gameObject.SetActive(true);
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            group.alpha = count;
            yield return new WaitForFixedUpdate();
        }

        SceneManager.LoadScene(sceneIndex);
        yield break;
    }
}
