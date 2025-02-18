using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
1) 게임상 기능  
   - 타일맵 무한 스크롤  
   - 자동 확장  

2) 주요 로직  
   - "Area" 태그 영역 이탈 감지  
   - 플레이어 위치 & 입력 방향 기준  
   - "Ground" 태그 객체의 위치 재배치  
   - "Enemy" 태그 객체의 위치 재배치 (멀어진 몹을 다시 플레이어 근처로 이동)  

3) 단계별 전개 흐름  
   1. "Area" 태그 영역을 벗어난 객체 감지  
   2. 태그 확인 후, "Area"가 아니면 종료  
   3. 플레이어 위치와 객체 위치 차이 계산  
   4. 입력 방향 분석하여 이동 방향 결정  
   5. 객체 재배치 수행  
      - "Ground" → X, Y 방향 우선 이동  
      - "Enemy" → 플레이어 근처에 다시 배치 (랜덤 위치 추가)  
*/

public class Reposition : MonoBehaviour
{
    Collider2D coll;

    void Awake()
    {
        // 충돌체 저장
        coll = GetComponent<Collider2D>();
    }

    // "Area" 밖으로 나갔을 때 실행됨
    void OnTriggerExit2D(Collider2D collision)
    {
        // "Area" 태그 확인 (해당 영역이 아닐 경우 종료)
        if (!collision.CompareTag("Area"))
            return;

        // 위치 정보
        Vector3 playerPos = GameManager.instance.player.transform.position;  // 플레이어 위치
        Vector3 myPos = transform.position;  // 현재 객체 위치

        // 위치 차이 계산
        float diffX = Mathf.Abs(playerPos.x - myPos.x);  // X축 차이
        float diffY = Mathf.Abs(playerPos.y - myPos.y);  // Y축 차이

        // 입력 방향 분석
        Vector3 playerDir = GameManager.instance.player.inputVec;  // 입력 벡터
        float dirX = playerDir.x < 0 ? -1 : 1;  // X 방향 부호
        float dirY = playerDir.y < 0 ? -1 : 1;  // Y 방향 부호

        // 객체 재배치 분기
        switch (transform.tag)
        {
            case "Ground":
                // 바닥 재배치 (X, Y 축 차이 비교)
                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 40); // X 방향 이동
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 40); // Y 방향 이동
                }
                break;

            case "Enemy":
                // 플레이어와 멀어진 몹 재배치 (근처로 이동)
                if (coll.enabled)
                {
                    transform.position = playerPos + playerDir * 20 + new Vector3(
                        UnityEngine.Random.Range(-3f, 3f),  // X축 랜덤 오프셋
                        UnityEngine.Random.Range(-3f, 3f),  // Y축 랜덤 오프셋
                        0f);
                }
                break;
        }
    }
}
