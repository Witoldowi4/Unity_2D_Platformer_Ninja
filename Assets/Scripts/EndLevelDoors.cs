using System.Collections;
using System.Collections.Generic;

//using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelDoors : MonoBehaviour
{
    [SerializeField] private int _coinsToNextLevel;
    [SerializeField] private int _levelToLaod;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _openDoorsSprite;
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null && player.CoinsAmount >= _coinsToNextLevel)
        {
            //if (player.CoinsAmount >= _coinsToNextLevel)
            //{
            Debug.Log("Doors opend");
            _spriteRenderer.sprite = _openDoorsSprite;
            Invoke(nameof(LoadLevel), 2f);
            //}
          
        }
    }

    private void LoadLevel()
    {
        //SceneManager.LoadScene("Level_2");
        SceneManager.LoadScene(_levelToLaod);
    }
}