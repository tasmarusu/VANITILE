using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public static Goal instance { get; private set; }

    public bool isGoal { get; private set; }

    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // KeyManagerが無ければ飛ばす
        if (KeyManager.instance == null)
        {
            isGoal = true;
            return;
        }

        // ゴールの条件を満たしているかどうか
        CheckEnableGoal();
    }


    /// <summary>
    /// ゴールの条件を満たしているかどうか
    /// </summary>
    private void CheckEnableGoal()
    {
        if (isGoal) return;

        int getKeyCount = KeyManager.instance.getKeyCount;
        int startStageKeyCount = KeyManager.instance.startStageKeyCount;
        if (getKeyCount == startStageKeyCount)
        {
            isGoal = true;
            spriteRenderer.color = Color.green;
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isGoal) return;

            Debug.Log("プレイヤーがゴールに入った");

            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
    }
}
