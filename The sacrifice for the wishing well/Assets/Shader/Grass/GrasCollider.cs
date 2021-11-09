using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GrassCamScript;

public class GrasCollider : MonoBehaviour
{
    //protected Rigidbody2D rb;

    [Header("Gras- Collider:")]
    [Tooltip("einflussbereich in form eines abgerundeten Rechtecks: x=breite, y=höhe, z=Eck-Radius, w=Verschlierung")]
    public Vector4 grasInput;
    [Tooltip("Stärkefaktoren: x=Drück-Faktor, y=Abstoß-Faktor, z=Rotations-Faktor, w=/")]
    public Vector4 grasStrength;

    Vector3 prevPos;
    /*
    // Start is called before the first frame update
    protected void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
    }
    //*/

    // Update is called once per frame
    void Update()
    {
        grasScript.objData.Add(new Vector4(transform.position.x, transform.position.y, transform.position.x - prevPos.x, transform.position.y - prevPos.y));
        //alternativ:
        //grasScript.objData.Add(new Vector4(rb.position.x, rb.position.y, rb.velocity.x, rb.velocity.y));
        grasScript.objScale.Add(grasInput);
        grasScript.objStrength.Add(grasStrength);

        prevPos = transform.position;
    }
}
