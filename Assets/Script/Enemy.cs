using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/* 
1) 게임상 기능  
   - 적(Enemy) 오브젝트가 지정된 대상(target)을 향해 자동으로 이동합니다.

2) 주요 로직 
   - Rigidbody2D를 활용하여 물리 기반의 이동을 수행합니다.
   - FixedUpdate()에서 목표(target)와 현재 위치 간의 방향 벡터를 계산해, 
     적이 일정 속도로 대상에게 접근하도록 합니다.
   - rigid.velocity를 0으로 설정하여, 불필요한 잔여 물리 효과를 제거합니다.

3) 단계별 전개 흐름  
   1. 컴포넌트 초기화  
      - Awake()에서 Rigidbody2D와 SpriteRenderer 컴포넌트를 가져와 초기화합니다.
   2. 이동 계산 및 적용  
      - FixedUpdate()에서 대상의 위치와 적의 위치 차이로부터 방향 벡터를 계산합니다.
      - 방향 벡터를 정규화한 후, 이동할 벡터를 속도와 Time.fixedDeltaTime을 곱해 산출합니다.
      - Rigidbody2D.MovePosition()을 사용해 새로운 위치로 이동시키며,
        이후 rigid.velocity를 0으로 초기화해 물리적 잔여 효과를 제거합니다.
*/

public class Enemy : MonoBehaviour
{
    // 적의 이동 속도
    public float speed;
    // 적이 추적할 목표의 Rigidbody2D 컴포넌트 (예: 플레이어)
    public Rigidbody2D target;

    // 적의 생존 여부 (현재 사용되지 않으나 향후 기능 확장을 위해 선언됨)
    bool isLive = true;

    // 물리 연산을 위한 Rigidbody2D 컴포넌트
    Rigidbody2D rigid;
    // 적의 시각적 표현을 위한 SpriteRenderer 컴포넌트
    SpriteRenderer spriter;

    // 오브젝트가 활성화될 때 호출되어 필요한 컴포넌트를 초기화합니다.
    void Awake()
    {
        // 현재 오브젝트의 Rigidbody2D 컴포넌트를 가져옵니다.
        rigid = GetComponent<Rigidbody2D>();
        // 현재 오브젝트의 SpriteRenderer 컴포넌트를 가져옵니다.
        spriter = GetComponent<SpriteRenderer>();
    }

    // FixedUpdate()는 일정한 시간 간격으로 호출되어 물리 연산에 적합합니다.
    void FixedUpdate()
    {
        if (!isLive)
            return;  


        // 목표(target)와 현재 위치 사이의 방향 벡터를 계산합니다.
        Vector2 dirVec = target.position - rigid.position;
        // 방향 벡터를 정규화한 후, 이동 속도와 고정 델타타임을 곱하여 이동할 벡터를 계산합니다.
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        // 계산된 이동 벡터를 현재 위치에 더해, Rigidbody2D를 통해 새로운 위치로 이동시킵니다.
        rigid.MovePosition(rigid.position + nextVec);
        // 이전 프레임의 물리 효과가 누적되지 않도록 속도를 0으로 초기화합니다.
        rigid.velocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!isLive)
            return;

        // 목표(target)와 현재 위치를 비교하여 뒤집어야 할지 결정합니다.
        bool shouldFlip = target.position.x > rigid.position.x;

        // Enemy 오브젝트와 모든 자식 오브젝트에 있는 SpriteRenderer 컴포넌트를 가져와 flipX 값을 설정합니다.
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.flipX = shouldFlip;
        }
    }
}
