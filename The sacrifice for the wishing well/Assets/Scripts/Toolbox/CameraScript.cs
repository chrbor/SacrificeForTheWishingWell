using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript cScript;

    public GameObject target;
    [Tooltip("wie schnell die Kamera dem target folgt")]
    public Vector2 strength = new Vector2(.1f, .5f);

    public void Start()
    {
        cScript = this;
        StartCoroutine(SetTargetOnPlayer());
    }

    IEnumerator SetTargetOnPlayer()
    {
        yield return new WaitForFixedUpdate();
        if(PlayerScript.pScript != null) target = PlayerScript.pScript.gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null) return;

        float diff_x = target.transform.position.x - transform.position.x;
        transform.position = new Vector3(transform.position.x + diff_x * strength.x, target.transform.position.y * strength.y, -10);
    }
}
