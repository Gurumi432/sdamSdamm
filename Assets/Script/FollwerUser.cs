using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollwerUser : MonoBehaviour
{
    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {
        rect.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position); //월드 상의 오브젝트 위치를 스크린 좌표를 변환  (위치 계산의 기준을 월드에서 스크린으로 전환)

    }
}