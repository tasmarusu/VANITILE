using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBlockDetection : MonoBehaviour
{

    public bool isTouch { get; private set; }   // 接触してるかどうか
    public uint touchCount { get; set; }        // 接触個数
    public float halfValue { get; private set; }// stateがwallなら横向きでgroundなら縦向きが入る
    public bool isActiveDelete { get; private set; }    // このオブジェクトにブロックが触れたら消えるか
    public Angle angle { get; private set; }    // 接触している角度がどこを向いているか

    public State state { get; private set; }
    [SerializeField]
    private State setState;

    private PlayerMove playerMove;
    private BoxCollider2D boxCollider2;


    public void SetActive(bool isActive)
    {
        isActiveDelete = isActive;
    }


    private void Start()
    {
        boxCollider2 = GetComponent<BoxCollider2D>();
        playerMove = transform.parent.GetComponent<PlayerMove>();

        SetActive(true);

        state = setState;

        if (state == State.Wall) halfValue = GetComponent<SpriteRenderer>().bounds.size.x * .4f;
        else if (state == State.Ground) halfValue = GetComponent<SpriteRenderer>().bounds.size.y * .4f;
    }



    /// <summary>
    /// ブロックに触れる
    /// </summary>
    public void HitBlock(GameObject hitBlockObj, Vector2 farValue)
    {
        touchCount++;
        isTouch = true;
        SeparatedPlayerToBlock(hitBlockObj, farValue);
    }


    /// <summary>
    /// ブロックからプレイヤーを離す
    /// 四角形の当たり判定を使用したら、プレイヤーの角とブロックの角が突っかかって降りなくなったので作った
    /// 地面も作る。(ドラえもんみたいに)
    /// </summary>
    private void SeparatedPlayerToBlock(GameObject hitBlockObj, Vector2 farValue)
    {
        GameObject playerObj = transform.parent.gameObject;
        Vector3 playerPos = playerObj.transform.position;
        Vector3 moveValue = new Vector3(farValue.x + halfValue, farValue.y + halfValue, 0.0f);
        Vector2 boundsSize = playerMove.playerComponent.boundsSize; // プレイヤーのSpriteRendererの大きさ
        Vector3 blockPos = hitBlockObj.transform.position;

        // 壁に当たった時
        if (state == State.Wall)
        {
            // 壁に当たった時に 右側 を向いている
            if (playerMove.playerAngleState == PlayerMove.PlayerAngle.Right)
            {
                moveValue.x *= -1.0f;
                moveValue.x -= boundsSize.x * 0.5f;
                angle = Angle.Right;
            }
            // 壁に当たった時に 左側 を向いている
            else
            {
                moveValue.x += boundsSize.x * 0.5f;
                angle = Angle.Left;
            }

            // X軸 ブロックの半分の大きさ+プレイヤーの半分の大きさ+壁判定の0.4倍の大きさ分横に移動させる
            // Y軸Z軸は壁なので気にしない
            moveValue = new Vector3(moveValue.x + blockPos.x, playerPos.y, playerPos.z);
        }
        // 地面当たった時 持ち上げる
        else if (state == State.Ground)
        {
            moveValue.y += boundsSize.y * 0.5f;
            moveValue = new Vector3(playerPos.x, moveValue.y + blockPos.y + playerPos.z);
            angle = Angle.Down;
        }
        else
        {
            moveValue = Vector3.zero + playerPos;
        }

        playerObj.transform.position = moveValue;
    }


    /// <summary>
    /// ブロックから離れる
    /// </summary>
    public void LeaveBlock()
    {
        touchCount--;

        if (touchCount <= 0)
        {
            isTouch = false;
            angle = Angle.None;
        }
    }


    public enum State
    {
        None,
        Wall,
        Ground
    }

    public enum Angle
    {
        None,
        Right,
        Left,
        Down
    }
}
