using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Gear : MonoBehaviour    // Gear 클래스: 아이템 부착 및 무기 속도 조정
{
    public ItemData.ItemType type;  // gear 종류설정 변수 (ItemData의 ItemType 열거형 사용)
    public float rate;              // gear 효과 비율

    public void Init(ItemData data)    // gear 초기화: 이름 설정, 플레이어에 부착, 효과 값 할당
    {
        name = "Gear " + data.itemId; // gear 이름 설정 ("Gear " + 아이템 아이디)
        transform.parent = GameManager.instance.player.transform; // 부모를 플레이어로 지정하여 함께 이동
        transform.localPosition = Vector3.zero; // 플레이어 기준 위치 (0, 0, 0)

        type = data.itemType; // gear 종류 배정
        rate = data.damages[0]; // gear 효과율 배정
        ApplyGear(); // gear 효과 적용 (gear 종류에 따른 효과 실행)
    }

    public void LevelUp(float rate)    // gear 레벨업: 효과 비율 갱신
    {
        this.rate = rate;
        ApplyGear(); // gear 효과 적용 (gear 종류에 따른 효과 실행)
    }

    void ApplyGear() // gear 종류에 따라 적용할 효과 결정
    {
        switch (type)
        {
            case ItemData.ItemType.Glove: // 글러브 타입: 무기 속도 증가 효과 적용
                {
                    RateUp(); // 무기 속도 증가 효과 실행
                    break;   // 글러브 케이스 종료
                }
            case ItemData.ItemType.Shoe:  // 슈즈 타입: 플레이어 이동 속도 증가 효과 적용
                {
                    SpeedUp(); // 이동 속도 증가 효과 실행
                    break;     // 슈즈 케이스 종료
                }
        }
    }

    // gear 효과에 따른 무기 속도 조정
    void RateUp() // 글러브 gear 효과: 무기 속도 조정
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();    // 플레이어의 모든 무기 연결
        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:
                    weapon.speed = 150 + (150 * rate); // 무기 id 0: 기본 회전속도 150에 gear 효과 반영 (회전 속도 증가)
                    break;
                default:
                    weapon.speed = 0.5f * (1f - rate); // 기타 무기: 기본 재장전시간 0.5에 gear 효과 반영 (장전 대기시간 감소)
                    break;
            }
        }
    }

    void SpeedUp() // 슈즈 gear 효과: 플레이어 이동 속도 증가
    {
        float speed = 3; // 기본 이동 속도 값
        GameManager.instance.player.speed = speed + speed * rate; // gear 효과 반영하여 이동 속도 증가
    }
}
