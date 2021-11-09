using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static LoadSave;

public class Child : Box
{
    public float jumpForce;
    public float walkSpeed;
    public float runSpeed;
    public float runDuration;
    [HideInInspector]
    public float runCounter = 0;

    public float coolDown;
    private float coolCounter;

    public bool afraid;
    public bool fleeing;

    const int childMask = (1 << 8) | (1 << 11) | (1 << 12) | (1 << 13) | (1<<14);//ground, well, player, object, (bridge) 
    private bool onGround;

    private void Start()
    {
        StartCoroutine(CheckGround());

        if (!afraid) StartCoroutine(Playing());
        else if (runCounter > 0) { fleeing = true; StartCoroutine(Fleeing(runCounter)); }
        else StartCoroutine(Walking());
    }

    protected override void SaveObjData() => progress.objects.Add(new ObjectData(gameObject, rb.velocity, afraid, runCounter));

    IEnumerator Playing()
    {
        while (transform.parent == null && !kidnapped)//solange nicht entführt, spielen
        {
            //Spiele-Animation:
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("flee");

        fleeing = true;
        afraid = true;
        kidnapped = true;
        yield return new WaitUntil(() => transform.parent == null && onGround);

        StartCoroutine(Fleeing(runDuration));

    }

    IEnumerator CheckGround()
    {
        while (true)
        {
            onGround = Physics2D.Raycast((Vector2)transform.position + new Vector2(-.175f, -.55f), Vector2.right, .35f, PlayerScript.groundMask).collider != null;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Walking()
    {
        coolCounter = coolDown;
        bool gapCheck;
        Vector2 newPos = transform.position;
        Vector2 oldPos = newPos + Vector2.one;
        while (!fleeing)//solange nicht entdeckt
        {
            if(transform.parent != null)
            {
                yield return new WaitUntil(() => transform.parent == null && onGround);
                transform.localScale = new Vector3(Mathf.Sign(rb.velocity.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            }

            if (coolCounter > 0) coolCounter -= Time.fixedDeltaTime;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, 5, childMask);

            gapCheck = false;
            if (!onGround)
            {
                newPos = transform.position;
                if((oldPos - newPos).sqrMagnitude < .001f) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
                oldPos = newPos;
            }

            //Move:
            rb.velocity = new Vector2(walkSpeed * transform.localScale.x, rb.velocity.y);

            //Player seen:
            if (hit.collider != null && hit.collider.gameObject.layer == 12)
            {
                fleeing = true;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
                continue;
            }

            //Nothing seen: check ground for gap
            else if (hit.collider == null || hit.distance >= 1.5f)
            {
                gapCheck = true;
                hit = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, -1.5f), 1f, PlayerScript.groundMask);//ehm. (1<<8)
                //Debug.Log("nothing: " + (hit.collider == null ? "null" : hit.collider.name));
                if ((hit.collider != null && hit.collider.gameObject.layer != 14) || coolCounter > 0)
                {
                    yield return new WaitForFixedUpdate();
                    continue;
                }
            }
            if (hit.collider != null && hit.collider.gameObject.layer == 14 && gapCheck)
                coolCounter = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * hit.collider.transform.eulerAngles.z)) < .65f ? coolDown : -1;



            //Wall/Obstacle/gap seen:
            if (onGround && (hit.collider == null || hit.distance < 1.5f))
            {
                if (coolCounter <= 0)
                {
                    //pondering:
                    for(float count = Random.Range(1.5f, 4f); count > 0; count -= Time.fixedDeltaTime)
                    {
                        yield return new WaitForFixedUpdate();
                        hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, 5, childMask);

                        //Player seen:
                        if (hit.collider != null && hit.collider.gameObject.layer == 12) { fleeing = true; break; }
                    }

                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
                    coolCounter = coolDown;
                }
                else if (onGround)
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(Fleeing(runDuration));
        yield break;
    }

    IEnumerator Fleeing(float _runCounter)
    {
        bool gapCheck;
        Vector2 newPos = transform.position;
        Vector2 oldPos = newPos + Vector2.one;
        coolCounter = coolDown;
        runCounter = _runCounter;
        while (runCounter > 0)
        {
            if (transform.parent != null)
            {
                yield return new WaitUntil(() => transform.parent == null && onGround);
                transform.localScale = new Vector3(Mathf.Sign(rb.velocity.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
                runCounter = runDuration;
            }


            if (coolCounter > 0) coolCounter -= Time.fixedDeltaTime;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, 5, childMask);

            gapCheck = false;
            if (!onGround)
            {
                newPos = transform.position;
                if ((oldPos - newPos).sqrMagnitude < .001f) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
                oldPos = newPos;
            }

            //Move:
            rb.velocity = new Vector2(runSpeed * transform.localScale.x, rb.velocity.y);


            //Player seen:
            if (hit.collider != null && hit.collider.gameObject.layer == 12 && hit.distance > 0 && coolCounter <= 0)
            {
                runCounter += (runDuration - runCounter) / 4;
                coolCounter = coolDown;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
                yield return new WaitForFixedUpdate();
                continue;
            }
            
            //Nothing seen: check ground for gap
            else if (hit.collider == null || hit.distance >= 3)
            {
                gapCheck = true;
                hit = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, -1.5f), 1, PlayerScript.groundMask);//ehm. (1<<8)
                if((hit.collider != null && hit.collider.gameObject.layer != 14) || coolCounter > 0)
                {
                    runCounter -= Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                    continue;
                }
            }
            if (hit.collider != null && hit.collider.gameObject.layer == 14 && gapCheck)
                coolCounter = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * hit.collider.transform.eulerAngles.z)) < .65f ? coolDown : -1;


            //Wall/Obstacle/gap seen:
            if (onGround && (hit.collider == null || hit.distance < 3))
            {
                if (coolCounter <= 0)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
                    coolCounter = coolDown;
                }
                else if (onGround)
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            runCounter -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        fleeing = false;
        StartCoroutine(Walking());
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9) { fleeing = true; rb.velocity = new Vector2(rb.velocity.x, jumpForce); }
    }
}
