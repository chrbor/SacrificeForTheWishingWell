using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerScript;
using static LoadSave;

public class Pot : Box
{
    [HideInInspector]
    public bool awake;
    private Vector3 startPosition;

    [Range(1, 60)]
    public float timer;
    [HideInInspector]
    public float timeLeft;
    [HideInInspector]
    public float process;

    private Material timer_mat;
    private int percent_id;
    private int color_id;


    [ColorUsage(false, true)]
    public Color color_start;
    [ColorUsage(false, true)]
    public Color color_middle;
    [ColorUsage(false, true)]
    public Color color_end;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        if (!awake) rb.constraints = RigidbodyConstraints2D.FreezeAll;

        process = 1;
        timeLeft = timer;
        timer_mat = transform.GetChild(1).GetComponent<SpriteRenderer>().material;
        percent_id = timer_mat.shader.GetPropertyNameId(timer_mat.shader.FindPropertyIndex("_percent"));
        color_id   = timer_mat.shader.GetPropertyNameId(timer_mat.shader.FindPropertyIndex("_colorTint"));

        timer_mat.SetFloat(percent_id, 1);
        timer_mat.SetColor(color_id, color_start);

        StartCoroutine(RunTimer());
    }

    protected override void SaveObjData() => progress.objects.Add(new ObjectData(gameObject, rb.velocity, awake, timeLeft));

    IEnumerator RunTimer()
    {
        if(!awake) yield return new WaitUntil(() => transform.parent != null);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        while (timeLeft > 0)
        {
            process = timeLeft / timer;
            timer_mat.SetFloat(percent_id, process);

            timer_mat.SetColor(color_id, process > 0.5f?
                (process-.5f) * 2 * color_start + (1-process) * 2 * color_middle :
                process * 2 * color_middle + (.5f - process) * 2 * color_end);
            timeLeft -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        timer_mat.SetFloat(percent_id, 0);
        Debug.Log("Meal went cold");
        awake = false;

        //werfe ab:
        if(transform.parent != null)
            pScript.Dropping();

        SpriteRenderer sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color spriteColor = sprite.color * new Color(1, 1, 1, 0);
        float timeStep = Time.fixedDeltaTime/2;
        for(float count = 1; count > 0; count -= timeStep)
        {
            sprite.color = spriteColor + Color.black * count;
            yield return new WaitForFixedUpdate();
        }
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        timeStep *= 2;
        for (float count = 0; count < 1; count += timeStep)
        {
            sprite.color = spriteColor + Color.black * count;
            yield return new WaitForFixedUpdate();
        }
        sprite.color = spriteColor + Color.black;

        Start();
    }
}
