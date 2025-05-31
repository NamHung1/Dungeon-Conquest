using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Treasure : MonoBehaviour
{
    public event Action OnTreasureCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnTreasureCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}
