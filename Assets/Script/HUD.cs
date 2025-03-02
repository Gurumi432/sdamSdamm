using System.Collections;                     // 컬렉션 관련 기능 사용
using System.Collections.Generic;             // 제네릭 컬렉션 사용
using UnityEngine;                            // Unity 엔진 기본 기능 사용
using UnityEngine.UI;                         // Unity UI 관련 기능 사용

public class HUD : MonoBehaviour             // HUD 클래스: MonoBehaviour를 상속받아 Unity 컴포넌트로 사용됨
{
    // 표시할 정보 유형을 정의하는 열거형
    public enum InfoType { Exp, Level, Kill, Time, Health }  // Exp: 경험치, Level: 레벨, Kill: 킬 수, Time: 시간, Health: 체력
    public InfoType type;                      // 현재 HUD에서 표시할 정보 유형을 설정하는 변수

    Text myText;                               // UI 텍스트 컴포넌트를 참조할 변수
    Slider mySlider;                           // UI 슬라이더 컴포넌트를 참조할 변수

    // 컴포넌트가 활성화되기 전에 한 번 호출되는 초기화 메서드
    void Awake()
    {
        myText = GetComponent<Text>();    // 자기 자신의 Text 컴포넌트를 가져와 myText에 할당
        mySlider = GetComponent<Slider>(); // 자기 자신의 Slider 컴포넌트를 가져와 mySlider에 할당
    }

    void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                float curExp = GameManager.instance.exp; //변수 초기화
                // 다음 레벨까지 필요한 최대 경험치 값을 GameManager에서 현재 레벨에 맞게 가져옴
                float maxExp = GameManager.instance.nextExp[GameManager.instance.level];
                mySlider.value = curExp / maxExp;
                break;

            case InfoType.Level:  // 레벨 정보 표시인 경우
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                // 텍스트에 현재 레벨 값을 정수 형태로 포맷하여 표시 ({}은 인수를 넣을 위치를 지정,0은 첫번째 인덱스, F0은 소수점이하 0자리까지 표시)
                break;

            case InfoType.Kill:  // 킬 수 정보 표시인 경우
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                // 텍스트에 현재 레벨 값을 정수 형태로 포맷하여 표시 ({}은 인수를 넣을 위치를 지정,0은 첫번째 인덱스, F0은 소수점이하 0자리까지 표시)
                break;

            case InfoType.Time:  // 시간 정보 표시인 경우
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt( remainTime / 60 );
                int sec = Mathf.FloorToInt( remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec); //D2는 자리수를 항상 2개로 한다는 의미. 예를들어 2면 02가 됨
                break;

            case InfoType.Health:  // 체력 정보 표시인 경우
                // 체력 관련 업데이트 로직 추가 필요
                break;
        }
    }
}
