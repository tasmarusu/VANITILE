using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyEach : MonoBehaviour
{
    public bool isGetPlayerMine { get; private set; }// プレイヤーに取得された



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isGetPlayerMine) return;

            isGetPlayerMine = true;
            GetComponent<SpriteRenderer>().color = Color.grey;

            KeyManager.instance.getKeyCount++;
        }
    }
}
