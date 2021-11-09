using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerScript;

public class TouchSensor : MonoBehaviour
{
    public static int touchMask = (1 << 8) | (1 << 13) | (1 << 14) | (1 << 15);

    private void Start() => StartCoroutine(GetDestroyed());
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject);

        IPortable portable = other.GetComponent<IPortable>();
        if (portable == null) return;

        Vector2 diffPos = transform.position - pScript.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pScript.transform.position, diffPos, 1.5f, touchMask);//oder child
        if (hit.collider == null || hit.collider.gameObject != other.gameObject) return;
        Debug.Log("pick" + hit.collider);


        pScript.StartPickUp(other.gameObject, diffPos);
        Destroy(gameObject);
    }

    IEnumerator GetDestroyed()
    {
        yield return new WaitForSeconds(.2f);
        Destroy(gameObject);
    }
}
