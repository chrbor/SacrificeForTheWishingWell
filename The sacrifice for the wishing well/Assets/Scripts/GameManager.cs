using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using static MenuScript;
using static LoadSave;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    public static bool showIntro;
    public static bool gameComplete;

    public static bool gameRun;
    public static bool gamePause;
    public static bool noReset;

    public static bool kidnapped;
    public static string[] taskList = new string[] { "Apple", "Pot", "Child", "Box", "Bridge", "Player"};


    public static Controls controls;


    void Start()
    {
        manager = this;

        gameRun = true;
        gamePause = false;

        //control-system:
        controls = new Controls();
        controls.GameControl.Enable();
        controls.GameControl.Reset.performed += Reseting;
        controls.GameControl.Pause.performed += OpenMenu;

        showIntro = !Load() && showIntro;
        if (showIntro)
        {
            Debug.Log("show intro");
            showIntro = false;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else StartCoroutine(ResetWindow());
    }

    private void OnDestroy()
    {
        controls.GameControl.Reset.performed -= Reseting;
        controls.GameControl.Pause.performed -= OpenMenu;
    }

    void OpenMenu(InputAction.CallbackContext ctxt) { Debug.Log("open"); mScript.OpenMenu(); }

    private bool resetAvailable;
    IEnumerator ResetWindow()
    {
        resetAvailable = true;
        yield return new WaitForSeconds(1);
        resetAvailable = false;
    }


    void Reseting(InputAction.CallbackContext ctxt)
    {
        if (noReset) return;

        gamePause = true;
        if (progress.level == 0) progress = null;
        if(resetAvailable) ResetProgress();

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        DoReset();
    }
    public void DoReset()
    {
        Load();
        StartCoroutine(ResetWindow());
        CameraScript.cScript.Start();

        gamePause = false;
    }

    public void SpawnObjects()
    {
        Debug.Log("Spawn");
        GameObject obj;
        foreach (var objData in progress.objects)
        {
            /*if (objData.name == "Player")
            {
                obj = GameObject.FindGameObjectWithTag("Player");
                obj.transform.position = new Vector3(objData.position[0], objData.position[1]);
                obj.transform.eulerAngles = Vector3.forward * objData.rotation;
            }
            else*/ obj = Instantiate(Resources.Load<GameObject>("prefabs/" + objData.name), new Vector3(objData.position[0], objData.position[1]), Quaternion.Euler(0, 0, objData.rotation));

            obj.GetComponent<Rigidbody2D>().velocity = new Vector2(objData.velocity[0], objData.velocity[1]);

            switch (objData.name)
            {
                case "Child":
                    Child childScript = obj.GetComponent<Child>();
                    childScript.afraid = objData.activated;
                    childScript.runCounter = objData.stateVal;
                    obj.transform.localScale = new Vector3(Mathf.Sign(objData.velocity[0]) * obj.transform.localScale.x, obj.transform.localScale.y, 1);
                    break;
                case "Pot":
                    Pot potScript = obj.GetComponent<Pot>();
                    potScript.awake = objData.activated;
                    potScript.timeLeft = objData.stateVal;
                    break;
            }

            Debug.Log(obj.name + ": " + obj.transform.position);
        }
    }
}
