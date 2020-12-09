using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }

    private PlayerComponent playerComponent;


    void Start()
    {
        instance = this;
        playerComponent = GetComponent<PlayerComponent>();
    }
}
