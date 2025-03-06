using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;                                // 이 핸드가 왼손인지 여부 (true: 왼손, false: 오른손)
    public SpriteRenderer spriter;                     // 이 핸드의 스프라이트 렌더러

    SpriteRenderer player;                             // 플레이어의 스프라이트 렌더러를 저장할 변수

    Vector3 rightPos = new Vector3(1.86f, -1.14f, 0);    // 오른손의 기본 위치 좌표
    Vector3 rightPosReverse = new Vector3(2.41f, -1.14f, 0); // 오른손의 반전(역방향) 위치 좌표 (현재는 기본값과 동일)
    Vector3 leftPos = new Vector3(-2.53f, -1.14f, 0);    // 오른손의 기본 위치 좌표
    Vector3 leftPosReverse = new Vector3(-1.98f, -1.14f, 0); // 오른손의 반전(역방향) 위치 좌표 (현재는 기본값과 동일)
    // Quaternion leftRot = Quaternion.Euler(0, 0, -35);    // 왼손의 기본 회전 (Euler 각도로 -35도)
    // Quaternion leftRotReverse = Quaternion.Euler(0, 0, -35); // 왼손의 반전(역방향) 회전 (현재는 기본값과 동일)

    void Awake()                                      // 스크립트가 활성화될 때 한 번 호출되는 함수
    {
        // 부모 객체들에서 SpriteRenderer 컴포넌트를 찾아 두 번째 요소를 player 변수에 할당
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }

    void LateUpdate()
    {
        bool isReverse = player.flipX;

        if (isLeft)
        {
            transform.localPosition = isReverse ? leftPosReverse : leftPos;
            spriter.flipX = isReverse;
            //spriter.flipY = isReverse;
            spriter.sortingOrder = isReverse ? 6 : 3;
        }
        else
        {
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            spriter.flipX = isReverse;
            spriter.sortingOrder = isReverse ? 3 : 6;
        }
    }
}
