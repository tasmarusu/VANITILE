using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public PlayerComponent playerComponent { get; private set; }

    [Header("移動スピード"), SerializeField]
    private float moveSpeed = 1.0f;
    [Header("ジャンプ力"), SerializeField]
    private float jumpPower = 1.0f;
    [Header("壁ジャンプの強さ"), SerializeField]
    private Vector2 jumpWallPower = new Vector2();
    [Header("壁ジャンプ後の横の遅くなる量"),SerializeField,Range(0.0f,1.0f)]
    private float jumpWallDownValue = 1.0f;
    [Header("壁ジャンプ後の横の遅くなる量が0になる最低値"), SerializeField]
    private float minJumpWallDownValue = 1.0f;
    [Header("壁ジャンプ後のスティック移動が出来ない時間"), SerializeField]
    private float dontJumpWallTime = 1.0f;
    [Header("壁ジャンプ後の壁ジャンプ方向に進むスピード"), SerializeField]
    private float jumpWallStickValue = 1.0f;

    [Header("通常時の重力"), SerializeField]
    private float addNormalGravityValue = 1.0f;
    [Header("重力の最大値"), SerializeField]
    private float maxNormalGravityValue = 1.0f;
    [Header("壁張り付き時の重力"), SerializeField]
    private float addWallGravityValue = 1.0f;
    [Header("壁張り付き時の重力の最大値"), SerializeField]
    private float maxWallGravityValue = 1.0f;


    [Header("センシティブ量"), SerializeField]
    private float sensitiveValue = 0.1f;

    private bool isTouchWallPrevious;   // 1f前に壁に触れていたかどうか
    private float dontJumpWallTimer;
    private float jumpWallXPower = 0.0f;    // 壁ジャンプした際のx軸の移動量 0に近づく
    private float moveValue;  // 移動量をセット
    private float playerGravity; // -が下
    private Vector2 inputValue;
    private bool isInputDown;
    private uint previousWallTouchCount;  // 1f前の壁タッチ個数

    private PlayerState playerState = PlayerState.Wait;
    private PlayerState playerStatePrevious = PlayerState.Wait;
    public PlayerAngle playerAngleState { get; private set; }


    [SerializeField] private ContactFilter2D filter2d;

    private int touchingDownAndSideNum; // 0-1と2以外  1-下と左右どちらか接触状態 2-1の状態でジャンプしたら2になる

    private bool isGroundTouch1fAgo;
    public bool isGroundTouch { get; private set; }


    void Start()
    {
        playerComponent = GetComponent<PlayerComponent>();
    }

    void Update()
    {
        inputValue = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isInputDown = Input.GetKeyDown(KeyCode.Space);

        TouchingJumpLeftDownOrRightDown();

        Jump();
        //CloserWallJumpPowerToZero();
        LookAtInputValue();
    }


    private void FixedUpdate()
    {
        CheckPreviousWallTouch();
        SetMoveValue();

        Gravity();
        Move();
    }



    /// <summary>
    /// 1f前に壁ブロックに触れていたかどうか
    /// </summary>
    private void CheckPreviousWallTouch()
    {
        uint wallTouchCount = playerComponent.wallTouchComponent.touchCount;

        if (wallTouchCount != 0) return;
        if (wallTouchCount < previousWallTouchCount)
        {
            if (playerState == PlayerState.Jump && playerStatePrevious == PlayerState.Wall) return;
            playerStatePrevious = playerState;
            playerState = PlayerState.Jump;
        }
    }



    /// <summary>
    /// inputValue.xで向いている方向を決める
    /// </summary>
    private void LookAtInputValue()
    {
        if (!IsStickControll()) return;

        // 右向き
        if (inputValue.x >= sensitiveValue)
        {
            LookAtDesignationAngle(PlayerAngle.Right);
        }
        // 左向き
        else if (inputValue.x <= -sensitiveValue)
        {
            LookAtDesignationAngle(PlayerAngle.Left);
        }
    }


    /// <summary>
    /// 指定した方向を向く
    /// </summary>
    private void LookAtDesignationAngle(PlayerAngle playerAngle)
    {
        Vector3 angle = transform.eulerAngles;

        playerAngleState = playerAngle;
        angle.y = (float)playerAngle;

        transform.eulerAngles = angle;
    }


    /// <summary>
    /// プレイヤーの向きを逆にする
    /// </summary>
    private void LookAtReverseAngle()
    {
        if (playerAngleState == PlayerAngle.Right)
            LookAtDesignationAngle(PlayerAngle.Left);
        else
            LookAtDesignationAngle(PlayerAngle.Right);
    }



    /// <summary>
    /// 移動量を設定する
    /// </summary>
    private void SetMoveValue()
    {
        bool isJumpWallStickValue = false;

        float absInput = Mathf.Abs(inputValue.x);
        if (absInput > sensitiveValue && playerState == PlayerState.Jump)
        {
            if (jumpWallXPower >= 0.1f && inputValue.x >= sensitiveValue)
            {
                moveValue = inputValue.x * jumpWallStickValue;
                isJumpWallStickValue = true;
            }
            else if (jumpWallXPower <= -0.1f && inputValue.x <= -sensitiveValue)
            {
                moveValue = inputValue.x * jumpWallStickValue;
                isJumpWallStickValue = true;
            }
        }

        if (!IsStickControll()) return;

        if (absInput < sensitiveValue)
        {
            // 地面に接していれば待機状態に出来る
            if (isGroundTouch)
            {
                playerStatePrevious = playerState;
                playerState = PlayerState.Wait;
            }

            moveValue = 0.0f;   // 移動させないように
            return;
        }
        else
        {
            // 地面に接していてinput入力状態
            if (isGroundTouch)
            {
                playerStatePrevious = playerState;
                playerState = PlayerState.Run;
            }


            // 壁ジャンプ後、一定時間経過した後に操作が行われたらジャンプパワーを亡くす
            if (playerState == PlayerState.Jump && playerStatePrevious == PlayerState.Wall)
            {
                if (jumpWallXPower >= 0.1f && inputValue.x <= -sensitiveValue)
                {
                    jumpWallXPower = 0.0f;
                }
                else if (jumpWallXPower <= -0.1f && inputValue.x >= sensitiveValue)
                {
                    jumpWallXPower = 0.0f;
                }
            }
            else jumpWallXPower = 0.0f;


            touchingDownAndSideNum = 0;
            playerComponent.wallTouchComponent.SetActive(true);
        }

        if (!isJumpWallStickValue)
            moveValue = inputValue.x * moveSpeed;
    }


    /// <summary>
    /// スティック移動が可能かどうか
    /// 壁ジャンプした後少しの間スティック移動出来ない
    /// </summary>
    /// <returns> dontJumpWallTimer >= dontJumpWallTime </returns>
    private bool IsStickControll()
    {
        dontJumpWallTimer += Time.deltaTime;
        return dontJumpWallTimer >= dontJumpWallTime;
    }


    /// <summary>
    /// セットした移動量から移動させる
    /// </summary>
    private void Move()
    {
        Vector2 pos = playerComponent.myRigidbody2D.transform.position;
        Vector2 move = playerState == PlayerState.Wall ?
                    new Vector2(0.0f, playerGravity) * Time.fixedDeltaTime:     // 壁張り付きなので動かない
                    new Vector2(moveValue, playerGravity) * Time.fixedDeltaTime;// 通常動き
        Vector2 wall = new Vector2(jumpWallXPower, 0.0f) * Time.fixedDeltaTime;

        playerComponent.myRigidbody2D.MovePosition(pos + move + wall);
    }






    /// <summary>
    /// 壁ジャンプのXの移動量を0に近づける 
    /// jumpWallXPower
    /// </summary>
    private void CloserWallJumpPowerToZero()
    {
        float value = Mathf.Abs(jumpWallXPower);

        if (jumpWallXPower > 0.0f)
            jumpWallXPower -= jumpWallDownValue;
        else if (jumpWallXPower < 0.0f)
            jumpWallXPower += jumpWallDownValue;

        // jumpWallXPowerの値がminJumpWallDownValue以下なら0にする
        if (value < minJumpWallDownValue)
            jumpWallXPower = 0;
    }



    /// <summary>
    /// ジャンプ関連
    /// </summary>
    private void Jump()
    {
        // ジャンプボタン押してれば入る
        if (!isInputDown) return;

        if (!WallJump()) NormalJump();
    }


    /// <summary>
    /// 上にジャンプ
    /// </summary>
    private void NormalJump()
    {
        // 壁に触れてる   false
        // 地面に触れてる true
        //if (playerComponent.wallTouch.isTouch) return;
        if (!isGroundTouch) return;

        Debug.Log(Time.frameCount + "上ジャンプ");

        playerGravity = jumpPower;
        playerStatePrevious = playerState;
        playerState = PlayerState.Jump;

        touchingDownAndSideNum = 2;
    }


    /// <summary>
    /// 壁に張り付いている時のジャンプ
    /// </summary>
    private bool WallJump()
    {
        // 壁に触れてる   true
        // 地面に触れてる false
        if (!playerComponent.wallTouchComponent.isTouch) return false;
        if (isGroundTouch) return false;

        Debug.Log("壁ジャンプ開始");

        // 移動量代入
        //jumpWallXPower = transform.eulerAngles.y == 0.0f ? -jumpWallPower.x : jumpWallPower.x;  // 右を向いていれば左向きに力を加える
        bool isLookRightAngle = playerComponent.wallTouchComponent.angle == PlayerHitBlockDetection.Angle.Right;
        jumpWallXPower = isLookRightAngle ? -jumpWallPower.x : jumpWallPower.x;  // 右を向いていれば左向きに力を加える
        playerGravity = jumpWallPower.y;
        moveValue = 0.0f;

        // 向きを逆にする
        //LookAtReverseAngle();
        PlayerAngle playerAngle = isLookRightAngle ? PlayerAngle.Left : PlayerAngle.Right;
        LookAtDesignationAngle(playerAngle);

        // スティック移動一定時間出来ない
        dontJumpWallTimer = 0.0f;

        playerStatePrevious = playerState;
        playerState = PlayerState.Jump;

        return true;
    }



    /// <summary>
    /// 重力
    /// </summary>
    private void Gravity()
    {
        isGroundTouch1fAgo = isGroundTouch;
        isGroundTouch = playerComponent.myRigidbody2D.IsTouching(filter2d) ?   // filter掛けて地面判定を取る
                         true :         
                         playerComponent.groundTouchComponnet.isTouch;         // 地面と接している時の判定も確認する

        // 今地面に初めて着いた
        if (isGroundTouch && !isGroundTouch1fAgo) FirstOnGroundTouch();


        if (isGroundTouch) return;


        float addGravityValue, maxGravityValue;

        // True 壁に張り付いている状態
        if (playerComponent.wallTouchComponent.isTouch && touchingDownAndSideNum == 0)
        {
            if (playerComponent.wallTouchComponent.touchCount > 0 && previousWallTouchCount == 0)
                FirstOnWallTouch();

            addGravityValue = addWallGravityValue;
            maxGravityValue = maxWallGravityValue;
        }
        // False 壁に張り付いていない状態
        else
        {
            addGravityValue = addNormalGravityValue;
            maxGravityValue = maxNormalGravityValue;
        }


        // 重力を与える
        playerGravity += addGravityValue;

        // 重力の最大値を超えたので重力最大値を入れる
        if (playerGravity <= maxGravityValue)
            playerGravity = maxGravityValue;

        // 1f前の壁の接触個数を取得する
        previousWallTouchCount = playerComponent.wallTouchComponent.touchCount;
    }



    /// <summary>
    /// 左と下が接触しているか、右と下が接触している状態でジャンプすれば
    /// 左右の接触している方向に入力が入ると壁滑りに移行する。
    /// </summary>
    private void TouchingJumpLeftDownOrRightDown()
    {
        //  左と下が接触しているか、右と下が接触している状態なら true
        bool isWallTouch = playerComponent.wallTouchComponent.isTouch;
        bool isGroundTouch = playerComponent.groundTouchComponnet.isTouch;
        if (isWallTouch == true && isGroundTouch == true)
        {
            Debug.Log("下と左右が接触中");
            if (touchingDownAndSideNum == 0) touchingDownAndSideNum = 1;
            else if (touchingDownAndSideNum == 2) touchingDownAndSideNum = 1;

            playerComponent.wallTouchComponent.SetActive(false);
        }
    }



    /// <summary>
    /// 地面に張り付いた瞬間
    /// </summary>
    private void FirstOnGroundTouch()
    {
        jumpWallXPower = 0.0f;

        touchingDownAndSideNum = 0;
        playerComponent.wallTouchComponent.SetActive(true);
    }


    /// <summary>
    /// 壁に張り付いた瞬間
    /// </summary>
    private void FirstOnWallTouch()
    {
        Debug.Log("壁張り付き瞬間");
        playerGravity = 0.0f;
        jumpWallXPower = 0.0f;
        playerStatePrevious = playerState;
        playerState = PlayerState.Wall;
    }


    /// <summary>
    /// 壁に剥がれた瞬間
    /// </summary>
    private void FirstOutWallTouch()
    {
        playerGravity = 0.0f;
        jumpWallXPower = 0.0f;
        playerStatePrevious = playerState;
        playerState = PlayerState.Jump;
    }



    public enum PlayerAngle
    {
        Left = 180,
        Right = 0,
    }


    private enum PlayerState
    {
        Wait,
        Run,
        Jump,
        Wall,
    }
}
