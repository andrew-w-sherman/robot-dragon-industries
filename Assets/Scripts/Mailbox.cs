﻿using UnityEngine;
using System.Collections;

public class Mailbox : MonoBehaviour {

    public float clock = 0f;
    public int boxtype; // which sprite to use
    public bool open = true; // whether the mailbox is open
    public Plot plot;
    protected BoxCollider2D coll;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    private GameObject dollarSign;

    // initVel is relative to the camera
    public void init(Plot plot)
    {
        this.plot = plot;

        transform.parent = plot.transform;

        coll = gameObject.AddComponent<BoxCollider2D>();
        coll.isTrigger = true;

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        sr = gameObject.AddComponent<SpriteRenderer>();

        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(Random.Range(-2.5f, 2.5f), -2.5f, 1);
        coll.offset = new Vector2(0, .25f);
        coll.size = new Vector2(.5f, .5f);

        boxtype = Random.Range(1, 4);
        sr.sprite = Resources.Load<Sprite>("Sprites/mailbox" + boxtype + "open");

        gameObject.name = "Mailbox";
    }

    // Update is called once per frame
    void Update()
    {
        //animate dollar sign
        if (dollarSign != null)
        {
            float y = dollarSign.transform.localPosition.y;
            dollarSign.transform.localPosition = new Vector3(0, y + Time.deltaTime, 0);
            SpriteRenderer sr3 = dollarSign.GetComponent<SpriteRenderer>();
            float a = sr3.color.a;
            sr3.color = new Color(1, 1, 1, a - Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (open)
        {
            if (!other.gameObject.CompareTag("paper")) return;
            Destroy(other.gameObject);
            plot.manager.totalMoney += plot.manager.aggro * 50;
            if (plot.manager.dropped)
                plot.manager.totalMoney += plot.manager.thresholdBonus;
            plot.manager.reduceAggro(1); 
            sr.sprite = Resources.Load<Sprite>("Sprites/mailbox" + boxtype + "closed");
            open = false;

			// sound
			plot.manager.manager.PlayEffect (plot.manager.manager.money);

            //animation sprite
            makeDollarSign();

        }
    }

    private void makeDollarSign()
    {
        dollarSign = new GameObject();
        dollarSign.transform.parent = this.transform;
        SpriteRenderer sr2 = dollarSign.AddComponent<SpriteRenderer>();
        sr2.sprite = Resources.Load<Sprite>("Sprites/dollar");
        dollarSign.name = "Dollar Sign";
        dollarSign.transform.position = gameObject.transform.position;
    }
}
