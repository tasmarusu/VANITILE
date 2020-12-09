using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    public Rigidbody2D myRigidbody2D { get; private set; }
    public Collider2D myCollider2D { get; private set; }
    public SpriteRenderer mySpriteRenderer { get; private set; }
    public PlayerHitBlockDetection wallTouchComponent = null;
    public PlayerHitBlockDetection groundTouchComponnet = null;
    public Vector2 boundsSize { get; private set; }







    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myCollider2D = GetComponent<Collider2D>();
        mySpriteRenderer =transform.GetChild(0).GetComponent<SpriteRenderer>();

        boundsSize = mySpriteRenderer.bounds.size;

    }


    private void Update()
    {
        
    }
}
