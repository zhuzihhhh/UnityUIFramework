using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Tile : MonoBehaviour {
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public BoxCollider2D collider2D;

    public bool IsUsed { get; private set; }

    public TileMark User { get; set; }

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<BoxCollider2D>();
    }

    public void Use(TileMark user) {
        IsUsed = true;
        User = user;
        User.UsedTile = this;
    }

    public void UnUse() {
        IsUsed = false;
        User.UsedTile = null;
        User = null;
    }

    public void SetColor(Color color) {
        spriteRenderer.color = color;
    }
}