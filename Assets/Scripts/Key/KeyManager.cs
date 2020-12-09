using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    [Header("ステージにある鍵")]
    private KeyEach[] keyEachs = null;

    public int startStageKeyCount { get; private set; }

    public int getKeyCount { get; set; }


    public static KeyManager instance { get; private set; }


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        startStageKeyCount = transform.childCount;
        keyEachs = new KeyEach[startStageKeyCount];
        for (int i = 0; i < startStageKeyCount; i++)
        {
            keyEachs[i] = transform.GetChild(i).GetComponent<KeyEach>();
        }
    }

    void Update()
    {

    }
}
