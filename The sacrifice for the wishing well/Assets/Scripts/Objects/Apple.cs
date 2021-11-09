using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LoadSave;

public class Apple : Box
{
    public bool awake;

    // Start is called before the first frame update
    void Start()
    {
        if (!awake) rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    protected override void SaveObjData() => progress.objects.Add(new ObjectData(gameObject, rb.velocity, awake, 0));

    private void OnCollisionEnter2D(Collision2D other)
    {
        awake = true;
        rb.constraints = RigidbodyConstraints2D.None;
    }
}
