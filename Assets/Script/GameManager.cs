using System.Collections;
using System.Collections.Generic;
using System.Media;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;

    void Awake()
    {
        Instance = this;
    }
}
