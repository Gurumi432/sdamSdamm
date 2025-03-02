using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
1) 게임상 기능  
   - 전체 게임의 상태 및 전역 데이터를 관리하는 역할을 수행합니다.
   - 주로 플레이어와 같은 주요 오브젝트에 대한 전역 접근을 제공하는 매니저 역할입니다.

2) 주요 로직 
   - Singleton 패턴을 활용하여 GameManager 클래스의 인스턴스가 하나만 존재하도록 보장합니다.
   - 다른 클래스들은 GameManager.instance를 통해 손쉽게 전역 데이터를 접근할 수 있습니다.

3) 단계별 전개 흐름  
   1. 인스턴스 생성 및 할당  
      - Awake() 메서드에서 현재 인스턴스를 static 변수 instance에 할당함으로써 싱글톤을 구현합니다.
   2. 플레이어 객체 참조 관리  
      - public Player player를 통해 씬 내 플레이어 객체에 대한 참조를 관리합니다.
*/

public class GameManager : MonoBehaviour
{
    // GameManager 클래스의 유일한 인스턴스를 저장하기 위한 정적 변수 (싱글톤)
    public static GameManager instance;
    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    [Header("# Player Info")]
    public int level;
    public int health;
    public int maxhealth = 100;
    public int maxHealth = 1;
    public int kill;
    public int exp;
    public int[] nextExp = { 10, 30, 60, 100, 150, 210, 280, 360, 450, 600 };
    [Header("# Game Object")]
    public Player player; // Player라는 스크립트를 컴포넌트로 가진 오브젝트를 할당 받기 (유저가 직접 인스펙터 상에서 삽입)
    public PoolManager pool;

    // Awake()는 MonoBehaviour가 활성화될 때 가장 먼저 호출되는 메서드입니다.
    void Awake()
    {
        // 싱글톤 패턴 구현: 현재 인스턴스를 전역에서 접근 가능한 instance 변수에 할당합니다.
        instance = this;
    }


    void Start()
    {
        health = maxHealth;
    }


    void Update()
    {
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;

        }
    }

    public void GetExp()
    {
        exp++;

        if (exp == nextExp[level])
        {
            level++;
            exp = 0;
        }
    }
}
