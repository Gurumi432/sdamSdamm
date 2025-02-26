using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

// Bullet 클래스: 총알 오브젝트의 동작을 정의하는 스크립트
public class Bullet : MonoBehaviour
{
    // 총알이 줄 피해량을 나타냅니다.
    public float damage;
    // 'per' 변수는 추가적인 상태나 속성을 나타낼 수 있습니다.
    public float per;

    // Rigidbody2D 컴포넌트를 저장할 변수 (물리 연산에 사용)
    Rigidbody2D rigid;

    public BoxCollider2D Area;

    // Awake 함수: 스크립트가 활성화될 때 한 번 호출됩니다.
    void Awake()
    {
        // Rigidbody2D 컴포넌트를 가져와서 'rigid' 변수에 할당합니다.
        rigid = GetComponent<Rigidbody2D>();
    }

    // Init 함수: 총알의 초기 상태를 설정하는 함수
    // 파라미터:
    //   damage - 총알의 피해량
    //   per - 추가 속성값 (특정 조건 판단에 사용)
    //   dir - 총알이 이동할 방향 및 속도 (Vector3 형태)
    public void Init(float damage, int per, Vector3 dir)
    {
        // 전달받은 피해량과 추가 속성값을 클래스 변수에 할당합니다.
        this.damage = damage;
        this.per = per;

        // 'per' 값이 -1보다 큰 경우에만 총알에 속도를 부여합니다.
        // 이는 특정 조건(예: 발사된 총알일 때)에만 이동을 시작하도록 하기 위함입니다.
        if (per > -1)
        {
            // Rigidbody2D의 속도를 설정하여 총알을 주어진 방향으로 이동시킵니다.
            rigid.velocity = dir * 15f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {


        if (!collision.CompareTag("Enemy") || per == -1)
            return;

        per--;

        if (per == -1)
        {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area"))
        {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

}
