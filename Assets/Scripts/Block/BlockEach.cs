using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEach : MonoBehaviour
{
    [Header("ブロック")]
    private bool isTouch = false;
    private Vector2 boundsSize = Vector2.zero;

    private PlayerHitBlockDetection.State state = PlayerHitBlockDetection.State.None;
    private SpriteRenderer spriteRenderer;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boundsSize = spriteRenderer.bounds.size * 0.5f;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerHitBlockDetection"))
        {
            PlayerHitBlockDetection hitBlock = collision.GetComponent<PlayerHitBlockDetection>();

            if (!hitBlock.isActiveDelete) return;
            if (isTouch) return;

            Vector2 farValue = Vector2.zero;

            // 壁当たった
            if (hitBlock.state == PlayerHitBlockDetection.State.Wall)
            {
                farValue.x = boundsSize.x;
            }
            // 地面当たった
            else if (hitBlock.state == PlayerHitBlockDetection.State.Ground)
            {
                farValue.y = boundsSize.y;
            }

            hitBlock.HitBlock(gameObject, farValue);

            isTouch = true;
            spriteRenderer.color = collision.GetComponent<SpriteRenderer>().color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerHitBlockDetection"))
        {
            if (!isTouch) return;

            collision.GetComponent<PlayerHitBlockDetection>().LeaveBlock();
            
            spriteRenderer.color = Color.white;
            Destroy(gameObject);
        }
    }
} 
