using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
1) 게임상 기능  
   - **타일맵 무한 스크롤**: "Ground" 태그 객체를 이동시켜 계속적인 지형 유지  
   - **몹 재배치 시스템**: "Enemy" 태그 객체가 일정 거리 이상 멀어질 경우, 플레이어 주변으로 다시 배치  
   - **시야 관리**: "Area" 태그를 벗어난 객체를 감지하여 맵 밖 불필요한 오브젝트 정리  
   
2) 주요 로직 (태그별 동작)  
   - **"Area" 태그**: 감지 영역 → 객체가 벗어나면 해당 객체를 재배치  
   - **"Ground" 태그**: 타일맵 무한 확장을 위해 이동 방향 기준으로 40만큼 좌우 또는 상하 이동  
   - **"Enemy" 태그**: 플레이어와 너무 멀어질 경우, 일정 거리 내로 다시 이동 (랜덤 오프셋 추가)  
   
3) 단계별 전개 흐름  
   1. **"Area" 태그 영역을 벗어난 객체 감지**  
   2. **태그 확인 후 분기 처리**  
      - "Ground" → 방향 분석 후 40 단위로 이동  
      - "Enemy" → 플레이어 진행 방향을 기준으로 재배치 (랜덤 오프셋 포함)  
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
                    transform.position = playerPos + playerDir * 10 + new Vector3(
                        UnityEngine.Random.Range(-3f, 3f),  // X축 랜덤 오프셋
                        UnityEngine.Random.Range(-3f, 3f),  // Y축 랜덤 오프셋
                        0f);
                }
                break;
        }
    }
}
