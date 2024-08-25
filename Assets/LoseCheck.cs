using System;
using UnityEngine;

public class LoseCheck : MonoBehaviour
{
    private Game _game;

    private void Start() =>
        _game = FindObjectOfType<Game>();


    private void OnTriggerEnter2D(Collider2D other)
    {
        _game.Lose();
    }
}