using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    [Header("プレイヤーのコンポーネントを集める")]
    [Tooltip("PlayerSprite オブジェクト"),SerializeField]
    private GameObject playerSpriteObj = null;

    public Rigidbody2D myRigidbody2D { get; private set; }
    public Collider2D myCollider2D { get; private set; }
    public PlayerAnimation playerAnimation { get; private set; }
    public SpriteRenderer mySpriteRenderer { get; private set; }
    public PlayerHitBlockDetection wallTouchComponent = null;
    public PlayerHitBlockDetection groundTouchComponnet = null;

    public Vector2 boundsSize { get; private set; }







    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myCollider2D = GetComponent<Collider2D>();

        playerAnimation = playerSpriteObj.GetComponent<PlayerAnimation>();
        mySpriteRenderer = playerSpriteObj.GetComponent<SpriteRenderer>();


        boundsSize = mySpriteRenderer.bounds.size;
    }


    private void Update()
    {
        
    }
}
