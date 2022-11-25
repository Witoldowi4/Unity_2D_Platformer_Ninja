using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpPotion : MonoBehaviour
{
    [SerializeField] private int _hpPoints;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();

        if (player != null)
        {
            player.AddHp(_hpPoints);
            Destroy(gameObject);
        }
    }
}