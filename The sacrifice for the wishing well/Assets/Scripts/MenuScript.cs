using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LoadSave;
using static GameManager;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    Text text_time;
    public static MenuScript mScript;
    EventSystem evSystem;

    private bool noControl;

    private void Start()
    {
        mScript = this;
        evSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        text_time = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        StartCoroutine(UpdateTime());

        UpdateText();
    }

    IEnumerator UpdateTime()
    {
        yield return new WaitWhile(() => progress == null);

        int time_sec = 0;
        while (!gameComplete && progress != null)
        {
            if (!gameRun || gamePause) yield return new WaitUntil(() => gameRun && !gamePause);
            progress.time += Time.deltaTime;
            //Update der Anzeige:
            if ((int)progress.time != time_sec)
            {
                time_sec = (int)progress.time;
                text_time.text = time_sec.ToString();
            }
            yield return new WaitForEndOfFrame();
        }
    }
    
    public void UpdateText()
    {
        char[] splitChars = new char[] { '<', '>' };

        //Markiere die nächste passage:
        Text text = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        string[] oldText = text.text.Split('\n');
        string[] newText = new string[oldText.Length];

        for(int i = 0; i < oldText.Length; i++)
        {
            string[] lineparts = oldText[i].Split(splitChars);
            for (int j = 0; j < lineparts.Length; j += 2) newText[i] += lineparts[j];
        }

        string tip = "";
        if (progress.level < taskList.Length)
        {
            tip = newText[progress.level];
            newText[progress.level] = "<color=red>" + newText[progress.level] + "</color>";
            if (progress.level + 1 == taskList.Length)
            {
                newText[progress.level + 1] = "<color=red>" + newText[progress.level + 1] + "</color>";
                tip += "\n" + newText[progress.level + 1];
            }
        }
        text.text = "";
        foreach (var line in newText) text.text += line + "\n";

        //spiele tip ab:
        if (!playingTip && tip != "")
            StartCoroutine(PlayTip(tip));
    }

    bool playingTip;
    IEnumerator PlayTip(string tip)
    {
        playingTip = true;
        Text tipText = transform.GetChild(3).GetComponent<Text>();
        tipText.text = tip;
        tipText.gameObject.SetActive(true);
        CanvasGroup group = tipText.GetComponent<CanvasGroup>();

        float timeStep = Time.fixedDeltaTime / 2;
        Vector2 diff = Random.insideUnitCircle * 3;
        tipText.transform.position = (Vector2)Camera.main.transform.position + diff + Vector2.up * 2;
        Vector3 step = -Mathf.Sign(diff.x) * Vector3.right * 3 * timeStep;


        for(float count = 0; count < 1; count += timeStep)
        {
            group.alpha = count;
            tipText.transform.position += step;
            yield return new WaitForFixedUpdate();
        }
        for (float count = 0; count < 1; count += timeStep)
        {
            group.alpha = 1-count;
            tipText.transform.position += step;
            yield return new WaitForFixedUpdate();
        }

        tipText.gameObject.SetActive(false);
        playingTip = false;
    }

    public void ReturnToMenu() => SceneManager.LoadScene(0);

    public void OpenMenu() => StartCoroutine(Opening());
    IEnumerator Opening()
    {
        if (noControl) yield break;
        noControl = true;

        bool pausing = !gamePause;
        if (pausing)
        {
            gamePause = true;
            Physics2D.autoSimulation = false;
        }

        CanvasGroup group = transform.GetChild(1).GetComponent<CanvasGroup>();
        group.gameObject.SetActive(true);
        //Panel1:
        group.transform.GetChild(0).gameObject.SetActive(true);
        group.transform.GetChild(1).gameObject.SetActive(false);
        //Buttons aktivieren:
        group.transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);

        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            group.alpha = pausing? count : 1-count;
            yield return new WaitForFixedUpdate();
        }

        if (!pausing)
        {
            gamePause = false;
            Physics2D.autoSimulation = true;
            group.gameObject.SetActive(false);
        }

        noControl = false;
        yield break;
    }

    public void ShowControls() => StartCoroutine(ShowingControls());
    IEnumerator ShowingControls()
    {
        if (noControl) yield break;
        noControl = true;

        evSystem.enabled = false;
        CanvasGroup controlDisplay = transform.GetChild(2).GetComponent<CanvasGroup>();
        controlDisplay.gameObject.SetActive(true);
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
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
        noControl = false;
        yield break;
    }

    public void ShowEnd() => StartCoroutine(ShowingEnd());

    IEnumerator ShowingEnd()
    {
        noControl = true;
        gamePause = true;
        Physics2D.autoSimulation = false;

        CanvasGroup group = transform.GetChild(1).GetChild(1).GetComponent<CanvasGroup>();
        CanvasGroup timegroup = group.transform.GetChild(1).GetComponent<CanvasGroup>();
        timegroup.alpha = 0;
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        group.gameObject.SetActive(true);

        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            group.alpha = count;
            yield return new WaitForFixedUpdate();
        }

        timegroup.GetComponent<Text>().text = "Time\n<size=50%>" + (int)(progress.time / 60) + " min " + (int)(progress.time % 60) + " sec</size>";
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            timegroup.alpha = count;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitWhile(() => Input.anyKey);
        yield return new WaitUntil(() => Input.anyKey);
        noControl = false;

        SceneManager.LoadScene(0);
        yield break;
    }
}
