using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Lever : MonoBehaviour
{
    [SerializeField] private Sprite _activeSprite;
    [SerializeField] private Chest _chest;

    private SpriteRenderer _spriteRenderer;

    private Sprite _inactiveSprite;

    private bool _activated;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _inactiveSprite = _spriteRenderer.sprite;
        _chest.Activated = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null && !_activated)
        {
            _spriteRenderer.sprite = _activeSprite;
            _activated = true;
            _chest.Activated = true;
            _chest.activetOpenChest();

            Debug.Log("Activated");
        }
    }
}
