using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

/* 
1) 게임상 기능  
 타일맵 무한 스크롤 / 자동 확장

2) 주요 로직 
"Area" 태그를 가진 영역에서 객체가 이탈할 때, 
플레이어의 위치와 입력 방향을 기반으로 
"Ground" 태그를 가진 객체를 
적절한 방향으로 재배치(이동)하는 역할을 합니다.

3) 단계별 전개 흐름  
1. 트리거 이탈 감지  
    Collider가 "Area" 태그를 가진 트리거 영역을 벗어나면 OnTriggerExit2D 이벤트가 발생합니다.
2. 태그 확인  
    이탈한 충돌체가 "Area" 태그가 아닐 경우 함수가 종료됩니다.
3. 위치 차 계산  
    플레이어의 위치와 현재 객체의 위치 차이를 X축(diffX)와 Y축(diffY)로 계산합니다.
4. 입력 방향 부호 결정  
    플레이어의 입력 벡터를 기반으로 X, Y 방향의 부호(dirX, dirY)를 결정합니다.
5. 객체 태그에 따른 분기 처리  
    Ground 태그의 경우:  
      diffX와 diffY를 비교하여, X축 차이가 크면 X 방향, Y축 차이가 크면 Y 방향으로 일정 거리(40) 이동시켜 재배치합니다.  
    Enemy 태그의 경우:  
      추가 처리(현재 미구현)  
*/

public class Reposition : MonoBehaviour
{
    // 충돌체 이탈 이벤트
    // 충돌체 이탈 이벤트
    void OnTriggerExit2D(Collider2D collision)
    {
        // 태그 확인: Area
        if (!collision.CompareTag("Area"))
            return;

        // 플레이어 위치
        Vector3 playerPos = GameManager.instance.player.transform.position;
        // 현재 객체 위치
        Vector3 myPos = transform.position;
        // X 축 차이
        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        // Y 축 차이
        float diffY = Mathf.Abs(playerPos.y - myPos.y);

        // 플레이어 입력 벡터
        Vector3 playerDir = GameManager.instance.player.inputVec;
        // X 방향 부호
        float dirX = playerDir.x < 0 ? -1 : 1;
        // Y 방향 부호
        float dirY = playerDir.y < 0 ? -1 : 1;

        // 태그 분기
        switch (transform.tag)
        {
            case "Ground":
                // 바닥: X 우세
                if (diffX > diffY)
                {
                    // X 방향 이동
                    transform.Translate(Vector3.right * dirX * 40);
                }
                // 바닥: Y 우세
                else if (diffX < diffY)
                {
                    // Y 방향 이동
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            case "Enemy":
                // 적 태그
                break;
        }
    }
}