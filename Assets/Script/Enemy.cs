using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/* 
1) 게임상 기능  
   - 적(Enemy) 오브젝트, 지정 대상(target)으로 자동 이동

2) 주요 로직 
   - Rigidbody2D 활용, 물리 기반 이동
   - FixedUpdate(): 목표(target)와 현재 위치, 방향 벡터 계산 → 일정 속도, 대상 접근
   - rigid.velocity 0 할당, 불필요한 잔여 물리 효과 제거
*/

public class Enemy : MonoBehaviour
{
    // 적 이동 속도
    public float speed;
    // 현재 체력, 최대 체력
    public float health;
    public float maxHealth;
    // 추적 대상의 Rigidbody2D (예: 플레이어)
    public Rigidbody2D target;

    // 생존 여부 (향후 기능 확장용)
    bool isLive = true;

    // 물리 연산용 Rigidbody2D
    Rigidbody2D rigid;
    // 시각 표현용 SpriteRenderer
    SpriteRenderer spriter;
    // 애니메이션 제어용 Animator (자체 컴포넌트 자동 참조)
    Animator anim;

    // Awake() - 오브젝트 활성화 시, 컴포넌트 초기화
    void Awake()
    {
        // 현재 오브젝트, Rigidbody2D 컴포넌트 획득
        rigid = GetComponent<Rigidbody2D>();
        // 현재 오브젝트, SpriteRenderer 컴포넌트 획득
        spriter = GetComponent<SpriteRenderer>();
        // 현재 오브젝트, Animator 컴포넌트 획득 (애니메이션 컨트롤러 자동 참조)
        anim = GetComponent<Animator>();
    }

    // FixedUpdate() - 일정 시간 간격, 물리 연산 용도
    void FixedUpdate()
    {
        if (!isLive)
            return;

        // 목표(target)와 현재 위치, 방향 벡터 계산
        Vector2 dirVec = target.position - rigid.position;
        // 방향 벡터 정규화, 속도 및 Time.fixedDeltaTime 곱 → 이동 벡터 산출
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        // 이동 벡터 합산, Rigidbody2D.MovePosition() 사용
        rigid.MovePosition(rigid.position + nextVec);
        // 이전 물리 효과 누적 방지, rigid.velocity 0 할당
        rigid.velocity = Vector2.zero;
    }

    // LateUpdate() - 모든 Update() 후, 시각 보정 용도
    void LateUpdate()
    {
        if (!isLive)
            return;

        // 목표(target) 위치 비교, 스프라이트 좌우 반전 결정
        bool shouldFlip = target.position.x > rigid.position.x;

        // Enemy 및 자식 오브젝트, SpriteRenderer 컴포넌트 검색 → flipX 값 할당
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.flipX = shouldFlip;
        }
    }

    // OnEnable() - 오브젝트 활성화 시
    // GameManager의 플레이어 Rigidbody2D 할당, 적 상태 및 체력 초기화
    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        // 생존 상태 활성화
        isLive = true;
        // 체력, 최대 체력 할당
        health = maxHealth;
    }

    /*
    [data 흐름도]
    data.speed  (수동 입력) → Enemy.speed 결정
    data.health (수동 입력) → Enemy.maxHealth 결정
    data.health (수동 입력) → Enemy.health 결정
    (애니메이션 컨트롤러는 Enemy 오브젝트 자체의 Animator 컴포넌트 자동 참조)
    */

    // Init() - SpawnData 기반, 적 속성(속도, 체력) 초기화
    // 적 생성 시, 외부 설정 적용 용도
    public void Init(SpawnData data)
    {
        // 애니메이션 컨트롤러 자동 참조하므로, 별도 할당 없음

        // SpawnData.speed: 이동 속도 결정 → Enemy.speed 대입함.
        speed = data.speed;

        // SpawnData.health: 최대 체력 결정 → Enemy.maxHealth 대입함.
        maxHealth = data.health;

        // SpawnData.health: 초기 체력 결정 → Enemy.health 대입함.
        health = data.health;
    }
}
