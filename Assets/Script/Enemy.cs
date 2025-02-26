using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

/* 
1) 게임상 기능  
   - 적(Enemy) 오브젝트가 지정된 대상(target)으로 자동 이동하여 공격 또는 추적하는 기능 구현
*/

public class Enemy : MonoBehaviour
{
    // 적의 이동 속도 (값이 클수록 빠르게 이동)
    public float speed;
    bool isKnockBack = false;

    // 적의 현재 체력과 최대 체력
    public float health;
    public float maxHealth;

    // 애니메이터 컨트롤러 배열, 스프라이트 타입에 따라 애니메이션 변경에 사용
    public RuntimeAnimatorController[] animCon;
    // 추적 대상의 Rigidbody2D (예: 플레이어의 물리 컴포넌트)
    public Rigidbody2D target;

    // 적의 생존 상태 (false일 경우 행동 중지 등 향후 기능 확장에 사용)
    bool isLive = true;

    // 물리 연산을 위한 Rigidbody2D 컴포넌트 (이동, 충돌 처리 등)
    Rigidbody2D rigid;
    // 시각 표현을 위한 SpriteRenderer 컴포넌트 (스프라이트 렌더링)
    SpriteRenderer spriter;
    // 애니메이션 제어를 위한 Animator 컴포넌트 (애니메이션 재생)
    Animator anim;
    // FixedUpdate의 대기 시간을 위한 WaitForFixedUpdate (물리 연산 동기화)
    WaitForFixedUpdate wait;

    // Awake() - 오브젝트가 활성화될 때 최초로 호출, 필요한 컴포넌트들을 캐싱
    void Awake()
    {
        // 현재 오브젝트에 있는 Rigidbody2D, SpriteRenderer, Animator 컴포넌트를 가져옴
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        // FixedUpdate에서 기다릴 시간(고정된 업데이트 주기)를 초기화
        wait = new WaitForFixedUpdate();
    }

    // FixedUpdate() - 일정한 시간 간격(물리 프레임)마다 호출, 물리 연산(이동 처리)에 사용
    void FixedUpdate()
    {

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") )
            return;

        if (!isKnockBack)
        {
            // 추적 대상과의 방향 벡터 계산 (목표 위치 - 현재 위치)
            Vector2 dirVec = target.position - rigid.position;
            // 방향 벡터를 정규화한 후, 속도와 FixedDeltaTime을 곱해 이동할 거리를 계산
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            // 계산된 거리를 기준으로 현재 위치에서 이동
            rigid.MovePosition(rigid.position + nextVec);
            // 이동 후, 기존의 속도값을 0으로 초기화하여 누적되지 않도록 함
            rigid.velocity = Vector2.zero;
        }
    }

    // LateUpdate() - 모든 Update() 호출 후에 실행, 스프라이트의 방향(좌우 반전) 보정에 사용
    void LateUpdate()
    {
        // 적이 살아있지 않으면 처리하지 않음
        if (!isLive)
            return;

        // 대상의 x좌표가 현재 위치보다 오른쪽에 있으면 flip 처리 (방향 전환)
        bool shouldFlip = target.position.x > rigid.position.x;
        // 현재 오브젝트 및 자식들의 SpriteRenderer를 모두 찾아 flipX 값을 설정
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.flipX = shouldFlip;
        }
    }

    // OnEnable() - 오브젝트가 활성화될 때 호출, 상태 초기화 및 대상 지정
    void OnEnable()
    {
        // GameManager의 인스턴스에서 플레이어의 Rigidbody2D 컴포넌트를 가져와 추적 대상으로 설정
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        // 적을 활성 상태로 설정하고 체력을 최대 체력으로 초기화
        isLive = true;
        health = maxHealth;
    }

    // Init() - SpawnData를 기반으로 적의 속성(애니메이션, 속도, 체력 등)을 초기화하는 함수
    public void Init(SpawnData data)
    {
        // data에 저장된 spriteType에 따라 애니메이터 컨트롤러를 설정
        anim.runtimeAnimatorController = animCon[data.spriteType];
        // data에 설정된 속도와 체력 값들을 적용
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    // OnTriggerEnter2D() - Collider2D와 충돌 시 호출, 총알(Bullet)과 충돌하면 체력 감소 처리
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 객체가 "Bullet" 태그가 아니면 함수 종료
        if (!collision.CompareTag("Bullet"))
            return;

        // 충돌한 Bullet의 damage 값을 가져와 현재 체력에서 차감
        health -= collision.GetComponent<Bullet>().damage;
        // 충돌 후 넉백 효과를 주기 위해 코루틴 실행, 코루틴은 일시 중지가 가능한 함수
        

        // 체력이 남아있으면 Hit 애니메이션 재생, 체력이 0 이하이면 Dead() 호출하여 적 제거 처리
        if (health > 0)
        {
            anim.SetTrigger("Hit");
            StartCoroutine(KnockBack());
        }
        else
            Dead();
    }

    // KnockBack() - 총알 충돌 후 넉백 효과를 주기 위한 코루틴, IEnumerator: 코루틴을 구현할 때 사용되는 반환 타입(또는 도구)
    IEnumerator KnockBack()
    {
        // 넉백 시작 플래그 설정
        isKnockBack = true;
        // FixedUpdate 주기만큼 기다린 후 넉백 처리 (물리 연산과 동기화)
        yield return wait;
        // 플레이어 위치를 가져와 적과의 방향 벡터 계산 (적이 플레이어와 반대 방향으로 넉백)
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        // 정규화한 방향 벡터에 힘을 곱해 Rigidbody2D에 impulse 형태의 힘 추가 (즉각적 힘 적용)
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        // 넉백 효과 지속 시간 (예: 0.2초 후에 다시 이동 제어)
        yield return new WaitForSeconds(0.2f);
        isKnockBack = false;
    }

    // Dead() - 적이 죽었을 때 호출, 오브젝트를 비활성화하여 제거 처리
    void Dead()
    {
        // 오브젝트를 비활성화하면 OnEnable()을 통해 재활용 가능 (오브젝트 풀 방식 등)
        gameObject.SetActive(false);
    }
}
