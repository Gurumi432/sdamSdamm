using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

/* 
1) 게임상 기능  
   - 플레이어 이동 및 애니메이션 제어

2) 주요 로직 
   - 플레이어 입력을 받아 이동 벡터(inputVec)를 계산하고, 
     Rigidbody2D를 이용하여 물리적인 이동을 적용합니다.
   - Animator와 SpriteRenderer를 활용해 캐릭터의 애니메이션과 방향(좌우 반전)을 제어합니다.

3) 단계별 전개 흐름  
   1. 컴포넌트 초기화  
      - Start()에서 Rigidbody2D, SpriteRenderer, Animator 컴포넌트를 가져와서 초기화합니다.
   2. 입력 처리  
      - Update()에서 Input.GetAxis()를 사용하여 키보드 입력을 받아 inputVec 값을 갱신합니다.
      - OnMove()를 통해 새 Input System 이벤트에 따른 입력 벡터를 설정합니다.
   3. 물리 이동  
      - FixedUpdate()에서 inputVec와 speed, Time.fixedDeltaTime을 곱해 다음 위치를 계산하고,
        Rigidbody2D.MovePosition()을 이용하여 이동을 적용합니다.
   4. 애니메이션 및 시각 효과  
      - LateUpdate()에서 애니메이터의 "Speed" 파라미터를 업데이트하여 이동 애니메이션을 조정하고,
        입력 방향에 따라 SpriteRenderer.flipX 값을 변경하여 캐릭터의 바라보는 방향을 반전시킵니다.
*/

public class Player : MonoBehaviour
{
    // 플레이어의 입력 방향을 저장하는 벡터 (x: 좌우, y: 상하)
    public Vector2 inputVec;
    // 플레이어의 이동 속도
    public float speed;

    // 물리 계산을 위한 Rigidbody2D 컴포넌트
    Rigidbody2D rigid;
    // 캐릭터 스프라이트 관리를 위한 SpriteRenderer 컴포넌트
    SpriteRenderer spriter;
    // 애니메이션 관리를 위한 Animator 컴포넌트
    Animator anim;

    // 게임 시작 시 한 번 호출되어 컴포넌트들을 초기화합니다.
    void Start()
    {
        // 현재 오브젝트의 Rigidbody2D 컴포넌트를 가져옵니다.
        rigid = GetComponent<Rigidbody2D>();
        // SpriteRenderer 컴포넌트를 가져옵니다.
        spriter = GetComponent<SpriteRenderer>();
        // Animator 컴포넌트를 가져옵니다.
        anim = GetComponent<Animator>();
    }

    // 매 프레임 호출되며 입력값을 업데이트합니다.
    void Update()
    {
        // "Horizontal" 축 (좌우) 입력을 받아 x 값에 할당합니다.
        inputVec.x = Input.GetAxis("Horizontal");
        // "Vertical" 축 (상하) 입력을 받아 y 값에 할당합니다.
        inputVec.y = Input.GetAxis("Vertical");
    }

    // 일정 시간 간격(고정 프레임)마다 호출되어 물리 연산을 처리합니다.
    void FixedUpdate()
    {
        // 이동할 벡터 계산: 입력 벡터 * 속도 * 고정 델타타임
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        // 현재 위치에 nextVec를 더하여 새로운 위치로 이동시킵니다.
        rigid.MovePosition(rigid.position + nextVec);
    }

    // 새로운 Input System 이벤트를 통해 호출되며, 입력 벡터를 갱신합니다.
    void OnMove(InputValue value)
    {
        // InputValue로부터 Vector2 타입의 입력값을 가져와 inputVec에 할당합니다.
        inputVec = value.Get<Vector2>(); // Get<Vector2>는 일반적으로 정규화된 벡터를 반환합니다.
    }

    // 모든 Update() 호출 이후에 호출되어 애니메이션 및 시각 효과를 업데이트합니다.
    void LateUpdate()
    {
        // inputVec의 크기를 "Speed" 파라미터에 전달하여 이동 속도에 따른 애니메이션을 조정합니다.
        anim.SetFloat("Speed", inputVec.magnitude);

        // 좌우 이동 입력이 있을 경우에만 스프라이트의 좌우 반전을 처리합니다.
        if (inputVec.x != 0)
        {
            // 입력 방향이 오른쪽(양수)이면 flipX를 true, 왼쪽(음수)이면 false로 설정합니다.
            spriter.flipX = inputVec.x > 0;
        }
    }
}
