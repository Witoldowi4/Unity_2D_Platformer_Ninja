using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]

public class Chest : MonoBehaviour
{
   // private Rigidbody2D _rigidbody;

    [SerializeField] private int _coinsAmount;
   
    [SerializeField] private Sprite _activeChest;
    private SpriteRenderer _spriteRenderer;
    private Sprite _inactiveSprite;

    public bool Activated { private get; set; }
    // Start is called before the first frame update

    public void activetOpenChest( )
    {
        _spriteRenderer.sprite = _activeChest;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Activated)
            return;

        PlayerMover player = other.GetComponent<PlayerMover>();
        if(player != null)
        {
            player.CoinsAmount += _coinsAmount;       
        }
    }
    
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _inactiveSprite = _spriteRenderer.sprite;
    }

 
    
}
