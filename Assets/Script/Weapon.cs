using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
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
    public float ReloadSpeed;
    // 인스펙터에서 자식 Bullet의 회전 잠금 여부 선택
    // true이면 자식 Bullet이 월드 회전(0,0,0)으로 고정되어 원형 배치 시 회전 효과 유지
    public bool lockChildRotation = true;

    // 타이머 변수: 발사 주기를 제어하기 위해 사용
    float timer;
    // Player 컴포넌트 참조: 무기의 부모 객체에서 플레이어 정보를 얻습니다.
    Player player;

    // Awake 함수: 스크립트 인스턴스가 활성화될 때 한 번 호출되며,
    // 부모 객체에서 Player 컴포넌트를 가져옵니다.
    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    // Start 함수: 게임 시작 시 호출되어 무기를 초기화합니다.
    void Start()
    {
        Init();
    }

    // Update 함수: 매 프레임마다 호출되어 무기의 동작을 처리합니다.
    void Update()
    {
        // 무기 id에 따른 동작 분기 처리
        switch (id)
        {
            // id가 0인 경우: 예를 들어, 원형으로 Bullet을 배치하는 무기
            case 0:
                // 부모 오브젝트를 회전시킵니다.
                // Vector3.back 방향으로 회전하며, speed와 프레임의 델타타임을 곱해 부드러운 회전을 구현합니다.
                transform.Rotate(Vector3.back * ReloadSpeed * Time.deltaTime);

                // lockChildRotation이 true이면, 모든 자식 Bullet의 회전을 월드 기본값(Quaternion.identity)으로 고정합니다.
                if (lockChildRotation)
                {
                    foreach (Transform child in transform)
                    {
                        child.rotation = Quaternion.identity;
                    }
                }
                break;
            // 기본 케이스: id가 0이 아닌 경우, 타이머를 사용해 Fire() 함수를 호출합니다.
            default:
                // 지난 프레임의 시간을 타이머에 누적합니다.
                timer += Time.deltaTime;

                // 타이머가 speed 값보다 커지면, 타이머를 초기화하고 Fire() 함수를 호출하여 Bullet을 발사합니다.
                if (timer > ReloadSpeed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
        }

        /// .. Test Code..
        // "Jump" 버튼 입력 시 무기 레벨업을 테스트합니다.
        // 데미지 20 증가 및 Bullet 수 1 증가
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10, 1);
        }
    }

    // LevelUp 함수: 무기의 데미지와 Bullet 수를 증가시켜 레벨업 효과를 적용합니다.
    public void LevelUp(float damage, int count)
    {
        // 전달받은 데미지와 추가 Bullet 수를 현재 무기의 속성에 반영합니다.
        this.damage = damage;
        this.count += count;

        // 무기 id가 0인 경우, Bullet 배치(Batch) 함수를 호출하여 화면의 Bullet 배치를 업데이트합니다.
        if (id == 0)
        {
            CircleSkill();
        }
    }

    // Init 함수: 무기를 초기화하며, 무기 id에 따라 초기 설정을 다르게 적용합니다.
    public void Init()
    {
        switch (id)
        {
            case 0:
                CircleSkill();
                break;
            default:

                break;
        }
    }

    // Batch 함수: 현재 count 값만큼 Bullet을 생성하고, 원형으로 배치합니다.
    void CircleSkill()
    {
        // count만큼 반복하여 각 Bullet의 생성 및 배치를 수행합니다.
        for (int index = 0; index < count; index++)
        {
            Transform bullet;

            // 이미 생성된 자식 Bullet이 있다면 재사용합니다.
            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index); // 기존 Bullet 재사용
            }
            else
            {
                // 풀 시스템에서 prefabId에 해당하는 Bullet을 가져와 부모를 이 무기로 설정합니다.
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }

            // Bullet의 로컬 위치와 회전을 초기화합니다.
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            // Bullet을 원형으로 배치하기 위한 회전 각도 계산:
            // 360도를 Bullet의 총 개수(count)만큼 균등하게 분할합니다.
            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            // Bullet의 up 방향으로 1 단위 이동하여 원 형태 배치를 완성합니다.
            bullet.Translate(bullet.up * 1f, Space.World);

            // Bullet 스크립트의 Init 함수를 호출하여 데미지와 per 값을 초기화합니다.
            // per 값이 -1이면, 근접 무기에서 항상 적을 관통하는 효과를 의미합니다.
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
        }
    }

    // Fire 함수: 플레이어 위치에서 Bullet을 발사합니다.
    void Fire()
    {
        // 만약 플레이어의 scanner가 가까운 타겟을 찾지 못하면 함수를 종료합니다.
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        // 풀 시스템에서 prefabId에 해당하는 Bullet을 가져와 플레이어 위치에 배치합니다.
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = player.transform.position; // 플레이어 위치에서 Bullet 생성
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir); //(float damage, int per 대신 int count 입력, Vector3 dir)
    }
}
