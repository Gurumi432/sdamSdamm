using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/* 
1) 게임상 기능  
   - 적(Enemy) 오브젝트, 지정 대상(target)으로 자동 이동
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
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // FixedUpdate() - 일정 시간 간격, 물리 연산 용도
    void FixedUpdate()
    {
        if (!isLive)
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    // LateUpdate() - 모든 Update() 후, 시각 보정 용도
    void LateUpdate()
    {
        if (!isLive)
            return;

        bool shouldFlip = target.position.x > rigid.position.x;
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.flipX = shouldFlip;
        }
    }

    // OnEnable() - 오브젝트 활성화 시, 상태 초기화
    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        health = maxHealth;
    }

    // Init() - SpawnData 기반, 적 속성 초기화
    public void Init(SpawnData data)
    {
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    // 총알과 충돌 시 체력 감소 및 디버그 로그 출력
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet"))
            return;

        health -= collision.GetComponent<Bullet>().damage;

        if (health <= 0)
            Dead();
    }

    // 적 제거 처리
    void Dead()
    {
        gameObject.SetActive(false);
    }
}
