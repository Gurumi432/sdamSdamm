using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
namespace MyGame.Characters
{
    public class Player : MonoBehaviour
    {
        public Vector2 inputVec;
        public float speed;
        Rigidbody2D rigid;
        SpriteRenderer spriter;
        Animator anim;

        void Start()
        {
            rigid = GetComponent<Rigidbody2D>();
            spriter = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();
        }

        void FixedUpdate()
        {
            // 1. 힘을 준다
            //rigid.AddForce(inputVec);

            // 2. 속도 제어
            //rigid.velocity = inputVec;

            // 3. 위치 이동
            Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
            // .normalized : 박스에서 대각선 거리가 가로 거리보다 긴 것을 반영
            // speed : 빠른 정를 설정
            // Time.fixedDeltaTime : 기기마다 다른 속도를 일률화

            rigid.MovePosition(rigid.position + nextVec);

            inputVec.x = Input.GetAxis("Horizontal"); // 취향 껏 GetAxis 대신 GetAxisRaw 가능
            inputVec.y = Input.GetAxis("Vertical");
        }

        void OnMove(InputValue value)
        {
            inputVec = value.Get<Vector2>(); // .Get<Vector2> = nomalized

            Debug.Log("OnMove 호출됨: " + inputVec); // 이거 버그 호출도 안됨...
        }

        void LateUpdate()
        {
            anim.SetFloat("Speed", inputVec.magnitude); //magnitude = 방향 대신 순수한 값만 뽑는 메서드

            if (inputVec.x != 0) {;
                spriter.flipX = inputVec.x > 0;
            }
            Debug.Log("inputVec: " + inputVec); // 현재 입력 값 확인
            anim.SetFloat("Speed", inputVec.magnitude);
        }

    }

}