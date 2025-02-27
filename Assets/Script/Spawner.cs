using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 1) 게임상 기능
 *    - 적(Enemy) 생성(Spawn) 기능을 담당합니다.
 *    - 게임 진행 시간에 따라 생성 주기가 조절되며, 난이도가 반영됩니다.
 *
 * 2) 주요 로직
 *    - Update() 메서드에서 Time.deltaTime을 이용해 스폰 타이머(timer)를 증가시킵니다.
 *    - 게임 진행 시간(GameManager.instance.gameTime)에 따라 난이도(level)를 계산합니다.
 *    - 난이도에 따라 스폰 간격이 달라지며, timer가 일정 값보다 크면 Spawn() 메서드를 호출합니다.
 *    - Spawn() 메서드는 PoolManager를 사용해 적 객체를 가져와서, 자식 오브젝트들 중 랜덤하게 선택한 스폰 포인트에 배치합니다.
 *
 * 3) 단계별 전개 흐름
 *    1. Awake() 메서드:
 *       - 현재 오브젝트의 모든 자식 Transform들을 spawnPoint 배열에 저장합니다.
 *    2. Update() 메서드:
 *       - 매 프레임마다 timer를 증가시키고, 게임 진행 시간에 기반하여 난이도(level)를 계산합니다.
 *       - level에 따라 스폰 간격(0.5초 또는 0.2초)이 결정되며, 지정된 간격마다 Spawn()를 호출합니다.
 *    3. Spawn() 메서드:
 *       - PoolManager를 통해 난이도에 맞는 적 객체를 가져옵니다.
 *       - UnityEngine.Random.Range를 사용해 자식 오브젝트(인덱스 1부터 시작) 중 하나의 위치를 랜덤하게 선택하여 적을 해당 위치에 배치합니다.
 */

public class Spawner : MonoBehaviour
{
    // 스폰 포인트로 사용할 자식 오브젝트들의 Transform 배열
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    
    // 적 생성 타이머 (시간 누적용)
    float timer;
    
    // 게임 난이도를 결정하는 변수 (게임 진행 시간에 기반)
    int level;

    // Awake(): MonoBehaviour가 활성화될 때 가장 먼저 호출되는 메서드
    void Awake()
    {
        // 현재 오브젝트의 모든 자식 Transform들을 spawnPoint 배열에 저장
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    // Update(): 매 프레임마다 호출되는 메서드
    void Update()
    {
        // Time.deltaTime을 이용해 timer에 경과 시간을 누적
        timer += Time.deltaTime;
        // 게임 진행 시간에 따라 난이도(level)를 계산 (10초마다 난이도 증가)
        // 시작한지 총 10초 흘렀으면 1단계 페이즈, 20초 흘렀으면 2단계 페이즈, 30초 흘렀으면 3단계 페이즈 이런 방식
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 10f), spawnData.Length - 1);

        // 난이도에 따라 스폰 간격 설정:
        // level이 0이면 0.5초, 그 외에는 0.2초 간격으로 적 생성
        if (timer > spawnData[level].spawnTime)
        {
            // 스폰 간격이 경과하면 timer를 초기화하고 WSpawn() 메서드를 호출
            timer = 0;
            Spawn();
        } 
    }

    // Spawn(): 적 객체를 생성하여 스폰 포인트에 배치하는 메서드
    void Spawn()
    {
        // PoolManager를 통해 현재 난이도(level)에 맞는 적 객체를 가져옴
        GameObject enemy = GameManager.instance.pool.Get(level);
        // UnityEngine.Random.Range를 사용하여 자식 오브젝트(인덱스 1부터 시작) 중 랜덤한 위치를 선택한 후 적 객체의 위치를 설정
        enemy.transform.position = spawnPoint[UnityEngine.Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

[System.Serializable]
public class SpawnData
{
    public int AnimType;
    public float spawnTime;
    public int health;
    public float speed;

}