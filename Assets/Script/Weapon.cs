using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
1) 게임상 기능  
    타일맵 무한 스크롤: "Ground" 태그 객체를 이동시켜 계속적인 지형 유지  
    몹 재배치 시스템: "Enemy" 태그 객체가 일정 거리 이상 멀어질 경우, 플레이어 주변으로 다시 배치  
    시야 관리: "Area" 태그를 벗어난 객체를 감지하여 맵 밖 불필요한 오브젝트 정리  
   
2) 주요 로직 (태그별 동작)  
    "Area" 태그: 감지 영역 → 객체가 벗어나면 해당 객체를 재배치  
    "Ground" 태그: 타일맵 무한 확장을 위해 이동 방향 기준으로 40만큼 좌우 또는 상하 이동  
    "Enemy" 태그: 플레이어와 너무 멀어질 경우, 일정 거리 내로 다시 이동 (랜덤 오프셋 추가)  
   
3) 단계별 전개 흐름  
   1. "Area" 태그 영역을 벗어난 객체 감지  
   2. 태그 확인 후 분기 처리  
       "Ground" → 방향 분석 후 40 단위로 이동  
       "Enemy" → 플레이어 진행 방향을 기준으로 재배치 (랜덤 오프셋 포함)
*/

public class Weapon : MonoBehaviour
{
    // 무기 고유 ID (무기 종류를 구분하기 위한 변수)
    public int id;
    // 풀에서 가져올 프리팹의 ID
    public int prefabId;
    // 무기의 데미지 값
    public float damage;
    // 생성할 Bullet(총알) 개수
    public int count;
    // 회전 속도 (무기 혹은 오브젝트의 회전 속도)
    public float speed;
    // 인스펙터에서 자식 Bullet의 회전 잠금 여부 선택 (true일 경우, 자식 Bullet의 월드 회전 고정)
    public bool lockChildRotation = true;

    // 게임 시작 시 호출: 초기화 함수 실행
    void Start()
    {
        Init();
    }

    // 매 프레임마다 호출: Weapon 동작 처리
    void Update()
    {
        // id 값에 따른 분기 처리
        switch (id)
        {
            // id가 7인 경우
            case 0:
                // 부모 오브젝트 회전 (벡터 뒤쪽 방향, 속도와 deltaTime을 곱해 부드럽게 회전)
                transform.Rotate(Vector3.back * speed * Time.deltaTime);

                // lockChildRotation이 true일 경우, 모든 자식 Bullet의 회전을 월드 기본값(0,0,0)으로 고정
                if (lockChildRotation)
                {
                    foreach (Transform child in transform)
                    {
                        child.rotation = Quaternion.identity;
                    }
                }
                break;
            default:
                // 그 외 id의 경우 별도 처리 없음
                break;
        }

        /// .. Test Code..
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(20, 1);
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if (id == 0)
        {
            Batch();
        }
    }

    // 무기 초기화: id에 따른 초기 설정 및 Bullet 생성 처리 호출
    public void Init()
    {
        switch (id)
        {
            case 0:
                speed = -150;
                Batch();
                break;
            default:
                break;
        }
    }
    void Batch()
    {
        // count만큼 반복하여 Bullet 생성
        for (int index = 0; index < count; index++)
        {
            // 풀에서 prefab Id에 해당하는 오브젝트 가져오기
            Transform bullet;

            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index); // 수정된 부분
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            // 회전 적용: bullet이 원 궤도를 그리도록
            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1f, Space.World);

            bullet.GetComponent<Bullet>().Init(damage, -1); // -1은 Infinity Per(무한), 근접무기라 무조건 적을 관통한다는 뜻 
        }
    }

}