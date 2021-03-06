﻿using UnityEngine;
using System.Collections;

public class Newspaper : Projectile
{

    void Start() {
        gameObject.tag = "paper";
        initComponents();
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/paper");
        coll.size = new Vector2(3.6f, 1.4f);
    }

    void Update()
    {
        move();
        transform.eulerAngles = new Vector3(0, 0, 360 * clock * 2);
    }

    public new void OnTriggerEnter2D(Collider2D coll) { }

}