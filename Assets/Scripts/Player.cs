﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    
    public float speed = 3.5f;
    public float throwSpeed = 5f;
    private Vector3 deltaPos;
    public int hp = 3;
	public int maxhp = 3;

    public int papers;
	public int maxpapers = 20;

    public LevelManager lm = null;

    private Vector2 collOffset = new Vector2(0f, -0.55f);
    private Vector2 collSize = new Vector2(1.54f, 0.48f);

    private float paperCooldownClock;
	private float healthCooldownClock = 0f;

    private float invulnClock = 0f;
    private const float invulnTime = 0.8f;
    private bool invuln = false;

    private SpriteRenderer sr;
    private BoxCollider2D coll;
    private Rigidbody2D rb;

    private Sprite spr1;
    private Sprite spr2;

    private GameObject hurtHeart;

	/*
        Physics: player has a non-trigger collider and a rigidbody
        so it can interact with the bounding box and the enemies.
    */
	void Start () {
        gameObject.tag = "player";
		papers = maxpapers;

        spr1 = Resources.Load<Sprite>("Sprites/bike");
        spr2 = Resources.Load<Sprite>("Sprites/bike2");

        gameObject.AddComponent<SpriteRenderer>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = spr1;

        coll = gameObject.AddComponent<BoxCollider2D>();
        coll.offset = collOffset;
        coll.size = collSize;
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (lm.levelDone || lm.paused || lm.playerDead) return;
        if ((Time.timeSinceLevelLoad*2) % 2 < 1)
            sr.sprite = spr1;
        else
            sr.sprite = spr2;

        paperCooldownClock -= Time.deltaTime;
		healthCooldownClock -= Time.deltaTime;

        deltaPos = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            deltaPos.y += speed*Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            deltaPos.x -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            deltaPos.y -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            deltaPos.x += speed * Time.deltaTime;

        transform.position += deltaPos;

        // stay in bounding box
        if (transform.position.x < lm.boundLeft)
            transform.position = new Vector2(lm.boundLeft, transform.position.y);
        if (transform.position.x > lm.boundRight)
            transform.position = new Vector2(lm.boundRight, transform.position.y);
        if (transform.position.y < lm.boundDown)
            transform.position = new Vector2(transform.position.x, lm.boundDown);
        if (transform.position.y > lm.boundUp)
            transform.position = new Vector2(transform.position.x, lm.boundUp);

		if (Input.GetKey(KeyCode.LeftArrow))
            shoot(Vector2.left);
        if (Input.GetKey(KeyCode.RightArrow))
            shoot(Vector2.right);
        if (Input.GetKey(KeyCode.UpArrow))
            shoot(Vector2.up);
        if (Input.GetKey(KeyCode.DownArrow))
            shoot(Vector2.down);

		heal ();

        invulnClock += Time.deltaTime;
        if (invuln && invulnClock > invulnTime)
        {
            invuln = false;
            coll.enabled = true;
            sr.color = Color.white;
        }
        if (invuln && (Time.timeSinceLevelLoad * 10) % 2 >= 1)
            sr.color = Color.clear;
        else sr.color = Color.white;

        // sorting order hack
        sr.sortingOrder = 10000 - (int)
            ((transform.position.y - (sr.bounds.size.y)) * 100);

        //animate hurt heart
        if (hurtHeart != null)
        {
            float y = hurtHeart.transform.localPosition.y;
            hurtHeart.transform.localPosition = new Vector3(0, y + Time.deltaTime, 0);
            SpriteRenderer sr3 = hurtHeart.GetComponent<SpriteRenderer>();
            float a = sr3.color.a;
            sr3.color = new Color(1, 1, 1, a - Time.deltaTime);
        }
    }

    void shoot(Vector2 dir) {
		if ((paperCooldownClock > 0) || (papers < 1)) return;
        Newspaper paper = (new GameObject()).AddComponent<Newspaper>();
        paper.transform.position = this.transform.position;
        paper.transform.parent = lm.projectileFolder.transform;
        paper.name = "Paper";
        paper.init(dir, throwSpeed, deltaPos/Time.deltaTime/4);
		paperCooldownClock = 0.5f;
        papers -= 1;

		// sound
		lm.manager.PlayEffect(lm.manager.throwPaper);

    }

    public void hurt()
    {
		// sound
		lm.manager.PlayEffect(lm.manager.playerHit);

		// logic
        coll.enabled = false;
        invulnClock = 0f;
        invuln = true;
        hp--;
		healthCooldownClock = 4f;
		if (lm.aggro >= lm.thresholdAggro) {
			lm.reduceAggro (3);
		}
		if (hp == 0) {
			lm.playerDead = true;
            Time.timeScale = 0;
		}

        //animation sprite
        if (hp > 0)
        {
            makeHurtHeart();
        }
    }

	void heal() {
		if (lm.dropped == false && hp < maxhp) {
			if (healthCooldownClock > 0) return;
			hp++;
			healthCooldownClock = 4f;
		}
	}

    private void makeHurtHeart()
    {
        if (hurtHeart != null)
            Destroy(hurtHeart);
        hurtHeart = new GameObject();
        hurtHeart.transform.parent = this.transform;
        SpriteRenderer sr2 = hurtHeart.AddComponent<SpriteRenderer>();
        sr2.sprite = Resources.Load<Sprite>("Sprites/heartLittle");
        hurtHeart.name = "Hurt Heart";
        hurtHeart.transform.position = gameObject.transform.position;
    }

}
