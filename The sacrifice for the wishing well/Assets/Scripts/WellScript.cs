using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static GameManager;
using static LoadSave;

public class WellScript : MonoBehaviour
{
    char[] splitChars = new char[] { ' ', '(' };

    private void Start()
    {
        //bei letzter Task muss man selbst rein springen:
        if (progress.level == taskList.Length - 1)
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            col.size = new Vector2(1, .25f);
            col.offset = Vector2.up * .5f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (progress == null || gamePause || !gameRun || !other.name.Contains(taskList[progress.level])) return;
        Debug.Log("well takes " + other.name);

        //Zerstöre Objekt:
        StartCoroutine(DestroyObject(other.gameObject));
    }

    IEnumerator DestroyObject(GameObject obj)
    {
        noReset = true;

        string objName = obj.name.Split(splitChars)[0];
        if (objName == "Bridge") obj.GetComponent<WheelJoint2D>().enabled = false;

        //Werfe in den Brunnen:
        obj.transform.parent = transform;
        float shrinkFactor;
        if (objName == "Player") shrinkFactor = 1;
        else
            shrinkFactor = obj.GetComponent<BoxCollider2D>() != null ?
                obj.GetComponent<BoxCollider2D>().size.y : obj.GetComponent<CircleCollider2D>().radius;
                obj.GetComponent<Collider2D>().enabled = false;

        shrinkFactor = Mathf.Max(1, shrinkFactor);
        Vector3 stepScale = (1/shrinkFactor - 1) * Time.fixedDeltaTime * obj.transform.localScale;
        Vector2 diff = obj.transform.localPosition - Vector3.up * 2f;

        float timeStep = Time.fixedDeltaTime/.5f;
        for(float count = 1; count > 0; count -= timeStep)
        {
            obj.transform.localScale += stepScale;
            obj.transform.localPosition = new Vector3(diff.x * Mathf.Sin(Mathf.PI * .5f * count), 2 + count * diff.y);
            yield return new WaitForFixedUpdate();
        }
        //timeStep /= .5f;
        diff = Vector3.up * 4f;
        for (float count = 1; count > 0; count -= timeStep)
        {
            obj.transform.localPosition = new Vector3(0, -2 + Mathf.Sin(Mathf.PI * .5f * count) * diff.y);
            yield return new WaitForFixedUpdate();
        }
        Destroy(obj);
        yield return new WaitUntil(() => obj == null);

        //Speichere Fortschritt ab:
        Save(objName);

        //bei letzter Task muss man selbst rein springen:
        if (progress.level == taskList.Length-1)
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            col.size = new Vector2(1, .25f);
            col.offset = Vector2.up * .5f;
        }
        else if(progress.level == taskList.Length)
        {
            Debug.Log("SpielEnde");
            gameComplete = true;
            MenuScript.mScript.ShowEnd();
        }

        if(progress.level < taskList.Length)
        {
            Debug.Log("Well needs " + taskList[progress.level]);
        }

        noReset = false;
        yield break;
    }
}
