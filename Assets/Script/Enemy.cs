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
    Collider2D coll;

    [Header("KnockBack / Hit Animation Settings")]
    public float knockBackDuration = 0.2f;    // 넉백 효과 지속시간
    public float hitAnimationDuration = 0.5f;   // 히트 애니메이션 지속시간
    public float deadAnimationDuration = 1.0f;  // Dead 애니메이션 재생 시간

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
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
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
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
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;
        if (health > 0)
        {
            // 히트 애니메이션과 넉백 효과를 각각 독립적으로 처리
            StartCoroutine(HitAnimationReaction());
            StartCoroutine(Knockback());
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            // Dead 애니메이션이 재생될 시간을 기다린 후에 비활성화
            GameManager.instance.kill++;
            StartCoroutine(DeadAnimationCoroutine());
            GameManager.instance.GetExp();
        }
    }

    // 히트 애니메이션 전용 코루틴
    IEnumerator HitAnimationReaction()
    {
        anim.SetTrigger("Hit");
        yield return new WaitForSeconds(hitAnimationDuration);
    }

    // 넉백 전용 코루틴
    IEnumerator Knockback()
    {
        isKnockBack = true;
        yield return wait; // 물리 업데이트와 동기화

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockBackDuration);
        isKnockBack = false;
    }

    // Dead 애니메이션이 끝난 후에 게임 오브젝트를 비활성화하는 코루틴
    IEnumerator DeadAnimationCoroutine()
    {
        yield return new WaitForSeconds(deadAnimationDuration);
        gameObject.SetActive(false);
    }
}
