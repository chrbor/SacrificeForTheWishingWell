using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static GameManager;
using static TouchAndScreen;
using static LoadSave;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript pScript;

    Rigidbody2D rb;
    LineRenderer projectory;
    private Vector2 minMoveDist = new Vector2(.3f, .6f); 
    bool picking, aiming;

    public GameObject sensor;

    public float jumpForce;
    public int jumpSteps;
    private int jumpSteps_real;
    public float jumpDelay;
    private int jumpStepsLeft;
    private float jumpDelayLeft;

    public float moveSpeed;
    public float drag;

    private GameObject holdingObj;

    private Transform Tsprite;

    //Raycast-stuff für Bodenabfrage:
    public static int groundMask = (1<<8) | (1<<11) | (1<<13) | (1<<14);
    public static int wallMask   = (1<<8) | (1<<14);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectory = GetComponent<LineRenderer>();
        Tsprite = transform.GetChild(0);

        savingProgress += SaveObjData;
        clearObjects += DestroyObj;        
    }

    // Start is called before the first frame update
    void Start()
    {
        pScript = this;
        jumpSteps_real = jumpSteps;

        controls.GameControl.Jump.performed += StartJump;
        controls.GameControl.Move.performed += StartMove;
        controls.GameControl.PickUp.performed += PickUp;
        controls.GameControl.Drop.performed += Drop;
        controls.GameControl.Throw.performed += StartThrow;


        StartCoroutine(RunPlayerControl());
    }

    //*
    private void OnDestroy()
    {
        controls.GameControl.Jump.performed -= StartJump;
        controls.GameControl.Move.performed -= StartMove;
        controls.GameControl.PickUp.performed -= PickUp;
        controls.GameControl.Drop.performed -= Drop;
        controls.GameControl.Throw.performed -= StartThrow;
        savingProgress -= SaveObjData;
        clearObjects -= DestroyObj;
    }
    //*/

    void DestroyObj() => Destroy(gameObject);
    void SaveObjData() { progress.objects.Add(new ObjectData(gameObject, rb.velocity)); }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 9)
        {
            //normal:spiele death-animation ab
            gamePause = true;
            if (progress.level == 0) progress = null;

            manager.DoReset();
        }
    }


    IEnumerator RunPlayerControl()
    {
        while (true)
        {
            //Stoppe, wenn das Spiel nicht läuft:
            if(!gameRun || gamePause)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                yield return new WaitWhile(() => !gameRun || gamePause);
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            rb.velocity = new Vector2(rb.velocity.x * (1-drag), rb.velocity.y);

            //Jump-Ability test:
            if(jumpRefresh > 0)
                jumpRefresh -= Time.fixedDeltaTime;
            else
            {
                RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(-.175f, -.55f), Vector2.right, .35f, groundMask);
                if (hit.collider != null) jumpDelayLeft = jumpDelay;
                else jumpDelayLeft -= Time.fixedDeltaTime;

                if (hit.collider != null || (jumpDelayLeft > 0 && controls.GameControl.Jump.phase == InputActionPhase.Waiting))
                    jumpStepsLeft = jumpSteps_real;
                else if (controls.GameControl.Jump.phase == InputActionPhase.Waiting) jumpStepsLeft = 0;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    float jumpRefresh;

    void StartMove(InputAction.CallbackContext ctxt) => StartCoroutine(Move(ctxt));
    IEnumerator Move(InputAction.CallbackContext ctxt)
    {
        if (!gameRun || gamePause) yield break;

        float movedir = ctxt.ReadValue<float>();
        while (ctxt.phase != InputActionPhase.Waiting)
        {
            if (!gameRun || gamePause)
                yield return new WaitWhile(()=> !gameRun || gamePause);

            if (movedir != 0) transform.GetChild(0).transform.localScale = new Vector3(movedir * Tsprite.localScale.x, Tsprite.localScale.y, 1);
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(movedir * minMoveDist.x, -.3f), Vector2.up, minMoveDist.y, wallMask);
            if(hit.collider == null) rb.velocity = new Vector2(moveSpeed * ctxt.ReadValue<float>(), rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
    void StartJump(InputAction.CallbackContext ctxt) => StartCoroutine(Jump(ctxt));
    IEnumerator Jump(InputAction.CallbackContext ctxt)
    {
        if (!gameRun || gamePause) yield break;

        while (ctxt.phase != InputActionPhase.Waiting)
        {
            if (!gameRun || gamePause)
                yield return new WaitWhile(() => !gameRun || gamePause);

            if (jumpStepsLeft > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpStepsLeft--;
            }
            yield return new WaitForFixedUpdate();
        }
        jumpStepsLeft = 0;

        yield break;
    }

    void PickUp(InputAction.CallbackContext ctxt)
    {
        if (!gameRun || gamePause || holdingObj != null) return;
        Instantiate(sensor, PixelToWorld(Input.mousePosition), Quaternion.identity);
    }
    public void StartPickUp(GameObject obj, Vector2 diffPos) => StartCoroutine(PickingUp(obj, diffPos));
    IEnumerator PickingUp(GameObject obj, Vector2 diffPos)
    {
        Rigidbody2D rb_obj = obj.GetComponent<Rigidbody2D>();
        Collider2D addedCol;
        picking = true;
        holdingObj = obj;


        if(obj.layer == 14)//bridge
        {
            WheelJoint2D wheel = obj.GetComponent<WheelJoint2D>();
            wheel.connectedBody = rb;

            //teste griffe:
            GameObject testObj = new GameObject();
            testObj.transform.parent = obj.transform;

            testObj.transform.localPosition = wheel.anchor;
            if((testObj.transform.position - transform.position).magnitude > 1.5f)
            {
                testObj.transform.localPosition *= -1;
                if ((testObj.transform.position - transform.position).magnitude > 1.5f)
                    picking = false;
                wheel.anchor *= -1;
            }
            RaycastHit2D hit = Physics2D.Raycast(obj.transform.position, testObj.transform.position - obj.transform.position, wheel.anchor.magnitude, (1<<8) );
            if (hit.collider != null && hit.distance > Mathf.Abs(wheel.anchor.y) - .4f)
                picking = false;

            Destroy(testObj);
            if (!picking) yield break;

            obj.layer = 13;
            wheel.enabled = true;

            yield return new WaitUntil(() => controls.GameControl.PickUp.ReadValue<float>() == 0);
        }
        else
        {
            noReset = true;

            //Setze Objekt über den Kopf:
            rb_obj.simulated = false;
            obj.transform.parent = gameObject.transform;
            Vector2 diff = obj.transform.localPosition - Vector3.up * 2f;
            float rot = obj.transform.eulerAngles.z;
            float tstep = Time.fixedDeltaTime / .5f;
            for (float count = 1; count > 0; count -= tstep)
            {
                obj.transform.localPosition = new Vector3(diff.x * Mathf.Sin(Mathf.PI * .5f * count), 2 + count * diff.y);
                obj.transform.eulerAngles = Vector3.forward * rot * count;
                yield return new WaitForFixedUpdate();
            }
            obj.transform.rotation = Quaternion.identity;
            diff = Vector3.up;
            tstep *= 2;
            for(float count = 1; count > 0; count -= tstep)
            {
                obj.transform.localPosition = new Vector3(0,1 + Mathf.Sin(Mathf.PI * .5f * count) * diff.y);
                yield return new WaitForFixedUpdate();
            }
            obj.transform.localPosition = Vector3.up;

            //Ändere Collider:
            if(obj.GetComponent<BoxCollider2D>() != null)
            {
                BoxCollider2D boxCol_obj = obj.GetComponent<BoxCollider2D>();
                BoxCollider2D boxCol_pl = gameObject.AddComponent<BoxCollider2D>();
                addedCol = boxCol_pl;

                boxCol_pl.size = boxCol_obj.size;
                boxCol_pl.offset = boxCol_obj.offset + Vector2.up;

                minMoveDist = new Vector2(Mathf.Max(.3f, boxCol_pl.size.x/2 + .05f), boxCol_pl.size.y + 1f);
            }
            else
            {
                CircleCollider2D cirCol_obj = obj.GetComponent<CircleCollider2D>();
                CircleCollider2D cirCol_pl = gameObject.AddComponent<CircleCollider2D>();
                addedCol = cirCol_pl;

                cirCol_pl.radius = cirCol_obj.radius;
                cirCol_pl.offset = cirCol_obj.offset + Vector2.up;

                minMoveDist = new Vector2(Mathf.Max(.3f, cirCol_pl.radius + .05f), cirCol_pl.radius * 2 + 1f);
            }
            noReset = false;
        }

        jumpSteps_real = obj.GetComponent<IPortable>().GetJumpSteps();
        picking = false;
        yield break;
    }

    void Drop(InputAction.CallbackContext ctxt) => Dropping();
    public void Dropping()
    {
        if (!gameRun || gamePause || holdingObj == null || picking) return;

        if(transform.childCount == 1)//nur bei bridge
        {
            holdingObj.GetComponent<WheelJoint2D>().enabled = false;
            holdingObj.layer = 14;
        }
        else
        {
            //drop:
            holdingObj.transform.parent = null;
            Rigidbody2D rb_obj = holdingObj.GetComponent<Rigidbody2D>();
            rb_obj.simulated = true;
            rb_obj.velocity = rb.velocity + Vector2.right * 2 * transform.GetChild(0).localScale.x;
            Destroy(GetComponents<Collider2D>()[1]);
            minMoveDist = new Vector2(.3f, .6f);
        }

        jumpSteps_real = jumpSteps;
        holdingObj = null;
    }

    void StartThrow(InputAction.CallbackContext ctxt) => StartCoroutine(Throw(ctxt));
    IEnumerator Throw(InputAction.CallbackContext ctxt)
    {
        if (!gameRun || gamePause || holdingObj == null || picking) yield break;
        if (transform.childCount == 1) { Drop(ctxt); yield break; }
        IPortable portable = transform.GetChild(1).GetComponent<IPortable>();

        //aim:
        Vector2 oldPoint, newPoint, diff;
        Vector2 mouseDiff = Vector2.zero, vel;
        float stepLength = Time.fixedDeltaTime;
        float g = Physics2D.gravity.y * 3 * stepLength * stepLength;

        while (ctxt.phase != InputActionPhase.Waiting && controls.GameControl.Abort.ReadValue<float>() == 0)
        {
            if (!gameRun || gamePause)
                yield return new WaitWhile(() => !gameRun || gamePause);

            oldPoint = (Vector2)transform.position + Vector2.up;
            newPoint = oldPoint;
            mouseDiff = (PixelToWorld(Input.mousePosition) - (Vector2)(transform.position + Vector3.up)).normalized;
            vel = mouseDiff * portable.GetStrength() * stepLength;
            //Berechne Bogen
            int count = 0;
            for(; count < 120; count++)
            {
                //Debug.Log(vel + "\n" + newPoint);
                vel += Vector2.up * g;
                newPoint = oldPoint + vel;
                diff = newPoint - oldPoint;
                RaycastHit2D hit = Physics2D.Raycast(oldPoint, diff, diff.magnitude, groundMask);

                projectory.positionCount = count+1;
                projectory.SetPosition(count, oldPoint);
                if (hit.collider != null) break;

                oldPoint = newPoint;
            }
            projectory.positionCount = count + 1;
            projectory.SetPosition(count, newPoint);

            //Debug.Break();
            yield return new WaitForFixedUpdate();
        }
        projectory.positionCount = 0;

        if (controls.GameControl.Abort.ReadValue<float>() != 0) yield break;

        //throw:
        vel = mouseDiff * portable.GetStrength();
        GameObject obj = transform.GetChild(1).gameObject;
        obj.transform.parent = null;
        Rigidbody2D rb_obj = obj.GetComponent<Rigidbody2D>();
        rb_obj.simulated = true;
        rb_obj.velocity = vel;
        Destroy(GetComponents<Collider2D>()[1]);

        minMoveDist = new Vector2(.3f, .6f);
        jumpSteps_real = jumpSteps;
        holdingObj = null;
        yield break;
    }

}
