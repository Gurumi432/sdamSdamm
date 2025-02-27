using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    bool isKnockBack = false;

    public float health;
    public float maxHealth;

    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive = true;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    WaitForFixedUpdate wait;

    [Header("KnockBack / Hit Animation Settings")]
    public float knockBackDuration = 0.2f;    // 넉백 효과 지속시간
    public float hitAnimationDuration = 0.5f;   // 애니메이션 노출 지속시간

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (!isLive || isKnockBack)
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

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

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.AnimType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet"))
            return;

        health -= collision.GetComponent<Bullet>().damage;
        if (health > 0)
        {
            // 애니메이션과 넉백 효과를 각각 독립적으로 처리
            StartCoroutine(HitAnimationReaction());
            StartCoroutine(HitKnockbackReaction());
        }
        else
        {
            Dead();
        }
    }

    // 애니메이션 전용 코루틴 (Trigger 방식 사용)
    IEnumerator HitAnimationReaction()
    {
        anim.SetTrigger("Hit");
        // 별도로 대기할 필요가 없다면 hitAnimationDuration 제거 가능
        // 애니메이션 클립의 길이에 맞춰 자연스럽게 재생됨
        yield return new WaitForSeconds(hitAnimationDuration);
    }

    // 넉백 전용 코루틴
    IEnumerator HitKnockbackReaction()
    {
        isKnockBack = true;
        yield return wait; // 물리 업데이트와 동기화

        // 플레이어 반대 방향으로 넉백 처리
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockBackDuration);
        isKnockBack = false;
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
