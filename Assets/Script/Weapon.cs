using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

/*
[전체 설명]
이 스크립트는 무기(Weapon) 오브젝트의 동작을 정의하는 클래스입니다.
무기는 플레이어의 자식 객체로 설정되어 플레이어와 함께 움직이며,
원형 배치 또는 타이머 기반 발사를 통해 총알(Bullet)을 생성합니다.
또한, 무기 레벨업(LevelUp) 기능을 통해 데미지와 발사 총알 수를 조절할 수 있습니다.

게임 내 다른 시스템(예: 타일맵 무한 스크롤, 몹 재배치, 시야 관리 등)과는 분리된
무기 고유의 동작 로직을 포함하고 있습니다.
*/

public class Weapon : MonoBehaviour
{
    // 무기의 고유 식별자: 각 무기의 종류를 구분하기 위한 변수입니다.
    public int id;
    // 풀 시스템에서 사용할 프리팹의 인덱스 값
    public int prefabId;
    // 무기의 공격력(데미지) 값
    public float damage;
    // 생성할 총알(Bullet)의 개수
    public int count;
    // 무기의 발사 또는 회전 속도를 제어하는 변수 (ReloadSpeed가 클수록 동작 주기가 느려집니다)
    public float ReloadSpeed;
    // 자식 총알의 회전 잠금 여부:
    // true이면 자식 총알이 부모의 회전과 무관하게 월드 기준 회전(Quaternion.identity)을 유지
    public bool lockChildRotation = true;

    // 타이머 변수: 발사 주기 제어에 사용됩니다.
    float timer;
    // Player 컴포넌트 참조: 무기의 부모 객체에서 플레이어 정보를 얻기 위해 사용합니다.
    Player player;

    // Awake 함수: 스크립트 인스턴스가 활성화될 때 한 번 호출됩니다.
    // 여기서는 GameManager를 통해 플레이어 컴포넌트에 대한 참조를 가져옵니다.
    void Awake()
    {
        player = GameManager.instance.player;
    }

    // Update 함수: 매 프레임마다 호출되어 무기의 동작을 처리합니다.
    void Update()
    {
        // 무기 종류(id)에 따라 서로 다른 동작을 수행합니다.
        switch (id)
        {
            // id가 0인 경우: 원형 배치 무기 로직 적용
            case 0:
                // 무기(부모 오브젝트)를 Vector3.back 방향으로 회전시켜 원형 배치 효과를 만듭니다.
                // 회전 속도는 ReloadSpeed와 Time.deltaTime을 곱해 부드러운 회전을 구현합니다.
                ReloadSpeed = -150;
                transform.Rotate(Vector3.back * ReloadSpeed * Time.deltaTime);

                // 자식 총알의 회전을 고정하여 부모의 회전에 영향을 받지 않도록 합니다.
                if (lockChildRotation)
                {
                    foreach (Transform child in transform)
                    {
                        child.rotation = Quaternion.identity;
                    }
                }
                break;
            // 그 외 무기 id의 경우: 타이머를 이용해 일정 주기마다 발사(Fire) 동작 수행
            default:
                ReloadSpeed = 0.5F;
                // 지난 프레임의 경과 시간을 누적하여 타이머를 증가시킵니다.
                timer += Time.deltaTime;

                // 타이머 값이 ReloadSpeed보다 커지면 발사 동작을 수행합니다.
                if (timer > ReloadSpeed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
        }

        // ── 테스트 코드 ──
        // "Jump" 버튼 입력 시 무기 레벨업(LevelUp)을 테스트합니다.
        // 데미지를 10 증가시키고 총알 수를 1 증가시킵니다.
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10, 1);
        }
    }

    // LevelUp 함수: 외부에서 호출하여 무기의 레벨업 효과를 적용합니다.
    // 전달받은 damage와 count 값을 기반으로 데미지와 총알 수를 업데이트합니다.
    public void LevelUp(float damage, int count)
    {
        // 무기의 데미지를 업데이트 (이 예제에서는 기존 값을 덮어쓰지만,
        // 필요에 따라 누적 방식으로 변경할 수도 있습니다)
        this.damage = damage;
        // 총알 개수를 누적하여 증가시킵니다.
        this.count += count;

        // 무기가 원형 배치 무기(id 0)라면, 새롭게 Bullet 배치를 갱신합니다.
        if (id == 0)
        {
            CircleSkill();
        }
    }

    // Init 함수: 외부(ItemData)를 받아 무기를 초기화합니다.
    // 무기의 속성(id, 데미지, 총알 수, 프리팹 인덱스 등)을 설정하며,
    // 무기를 플레이어의 자식 객체로 배치합니다.
    public void Init(ItemData data)
    {
        // 무기의 이름을 "Weapon [itemId]" 형식으로 지정합니다.
        name = "Weapon " + data.itemId;
        // 무기를 플레이어의 자식으로 설정하여 플레이어와 함께 움직이도록 합니다.
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // 외부 데이터로부터 무기의 속성을 초기화합니다.
        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;

        // 풀 시스템에서 사용할 프리팹 인덱스(prefabId)를 결정합니다.
        // GameManager의 풀에 등록된 프리팹들과 비교하여 일치하는 프리팹의 인덱스를 저장합니다.
        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        // 무기 id에 따른 초기 동작 분기 처리
        switch (id)
        {
            // id가 0인 경우, 원형 배치 무기로 동작하도록 설정합니다.
            case 0:
                CircleSkill();
                break;
            default:
                // 다른 무기 타입에 대해 필요한 초기화 로직을 추가할 수 있습니다.
                break;
        }
    }

    // CircleSkill 함수: 현재 count 값에 맞춰 총알(Bullet)을 원형으로 배치합니다.
    // 각 Bullet은 균등한 각도로 회전시킨 후, up 방향으로 이동하여 원 형태를 완성합니다.
    void CircleSkill()
    {
        // count만큼 반복하여 각 Bullet을 생성 또는 재사용합니다.
        for (int index = 0; index < count; index++)
        {
            Transform bullet;

            // 이미 생성된 자식 총알이 있다면 재사용하여 성능 최적화
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

            // Bullet의 배치를 위한 회전 각도 계산:
            // 전체 360도를 총 Bullet 개수(count)로 나누어 각 Bullet의 회전 값을 결정합니다.
            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            // Bullet의 up 방향으로 1 단위 이동하여 원형 배열을 완성합니다.
            bullet.Translate(bullet.up * 1f, Space.World);

            // Bullet 스크립트의 Init 함수를 호출하여 데미지와 기타 속성을 초기화합니다.
            // 여기서 per 값이 -1이면 근접 무기에서 항상 적을 관통하는 효과를 나타냅니다.
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
        }
    }

    // Fire 함수: 플레이어의 스캐너가 감지한 가장 가까운 타겟 방향으로 총알을 발사합니다.
    void Fire()
    {
        // 플레이어의 스캐너가 타겟을 찾지 못하면 발사하지 않고 함수를 종료합니다.
        if (!player.scanner.nearestTarget)
            return;

        // 타겟의 위치를 가져와 현재 무기 위치에서의 방향 벡터를 계산합니다.
        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        // 풀 시스템에서 prefabId에 해당하는 Bullet을 가져와 플레이어 위치에 배치합니다.
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = player.transform.position; // 플레이어 위치에서 총알 생성
        // 총알의 회전을 타겟 방향으로 설정하여 발사 방향을 맞춥니다.
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        // Bullet 스크립트의 Init 함수를 호출하여 총알의 데미지, count(또는 per) 및 이동 방향을 초기화합니다.
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }
}
