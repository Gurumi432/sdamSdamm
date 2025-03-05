 using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour                // Item 클래스 선언 (MonoBehaviour 상속)
{
    public ItemData data;                        // 아이템 데이터 변수
    public int level;                            // 아이템 레벨 변수
    public Weapon weapon;                        // 무기 컴포넌트 변수
    public Gear gear;                            // gear 컴포넌트 변수

    Image icon;                                  // 아이콘 이미지 컴포넌트 (내부 변수)
    Text textLevel;                              // 레벨 표시 텍스트 컴포넌트 (내부 변수)

    void Awake()                                 // Awake 메서드 (초기화 시 호출)
    {
        icon = GetComponentsInChildren<Image>()[1];  // 자식 Image 컴포넌트 중 두 번째 할당
        icon.sprite = data.itemIcon;                   // 아이템 아이콘을 이미지에 적용

        Text[] texts = GetComponentsInChildren<Text>(); // 자식 Text 컴포넌트 배열 획득
        textLevel = texts[0];                           // 첫 번째 Text를 레벨 표시 텍스트로 할당
    }

    void LateUpdate()                           // LateUpdate 메서드 (매 프레임 후반 실행)
    {
        textLevel.text = "Lv." + (level);    // 레벨 텍스트를 갱신하여 표시
    }

    public void OnClick()                       // OnClick 메서드 (아이템 클릭 시 호출)
    {
        switch (data.itemType)
        {                // 아이템 타입에 따라 분기 처리
            case ItemData.ItemType.Melee:       // 근접 아이템 타입 처리
            case ItemData.ItemType.Range:       // 원거리 아이템 타입 처리 (동일 로직)
                if (level == 0)
                {               // 초기 레벨이면 무기 생성
                    GameObject newWeapon = new GameObject();  // 새 무기 오브젝트 생성
                    weapon = newWeapon.AddComponent<Weapon>();  // 무기 컴포넌트 추가 및 할당
                    weapon.Init(data);                        // 무기 초기화 메서드 호출
                }
                else
                {                          // 초기 레벨이 아니면 무기 레벨업
                    float nextDamage = data.baseDamage;       // 기본 데미지 할당
                    int nextCount = 0;                        // 총알 수 초기화

                    nextDamage += data.baseDamage * data.damages[level]; // 레벨 기반 데미지 증가 계산
                    nextCount += data.counts[level];                   // 레벨 기반 총알 수 증가 계산

                    weapon.LevelUp(nextDamage, nextCount);      // 무기 레벨업 메서드 호출
                }
                level++;
                break;                          // 근접/원거리 아이템 케이스 종료

            case ItemData.ItemType.Glove:       // 글러브 아이템 타입 처리
            case ItemData.ItemType.Shoe:        // 슈즈 아이템 타입 처리 (동일 로직)
                if (level == 0)
                {               // 초기 레벨이면 gear 생성
                    GameObject newGear = new GameObject();    // 새 gear 오브젝트 생성
                    gear = newGear.AddComponent<Gear>();        // gear 컴포넌트 추가 및 할당
                    gear.Init(data);                            // gear 초기화 메서드 호출
                }
                else
                {                          // 초기 레벨이 아니면 gear 레벨업
                    float nextRate = data.damages[level];       // 레벨 기반 gear 효과율 계산
                    gear.LevelUp(nextRate);                     // gear 레벨업 메서드 호출
                }
                break;                          // 글러브/슈즈 아이템 케이스 종료
                level++;
            case ItemData.ItemType.Heal:        // 회복 아이템 타입 처리 (미구현)
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;                          // Heal 케이스 종료
        }

        level++;                                // 아이템 레벨 증가

        if (level == data.damages.Length)
        {     // 최대 레벨 도달 시
            GetComponent<Button>().interactable = false; // 버튼 비활성화 (업그레이드 불가)
        }
    }
}
