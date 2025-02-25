using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

// Scanner 클래스: 주어진 범위 내에서 특정 레이어의 객체를 탐지하고, 그 중 가장 가까운 객체를 찾습니다.
public class Scanner : MonoBehaviour
{
    // 탐지 반경 (스캐너의 범위를 결정)
    public float scanRange;
    // 탐지할 대상 레이어 (어떤 레이어의 객체를 탐지할지 지정)
    public LayerMask targetLayer;
    // CircleCastAll 결과로 반환되는 객체들의 배열
    public RaycastHit2D[] targets;
    // 탐지된 대상 중 가장 가까운 객체의 Transform
    public Transform nearestTarget;

    // FixedUpdate는 물리 계산 주기에 따라 호출됩니다.
    void FixedUpdate()
    {
        // 현재 위치를 중심으로 scanRange 반경 내의 객체들을 CircleCastAll로 탐지합니다.
        // Vector2.zero 방향과 0 거리로 설정하여, 단순히 반경 내 객체들을 찾습니다.
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);

        // 탐지된 객체들 중 가장 가까운 객체를 찾아서 nearestTarget에 할당합니다.
        nearestTarget = GetNearest();
    }

    // GetNearest 함수: 탐지된 객체들 중 현재 스캐너와 가장 가까운 객체를 반환합니다.
    Transform GetNearest()
    {
        // 가장 가까운 객체의 Transform을 저장할 변수, 초기값은 null
        Transform result = null;
        // 현재까지 발견된 최소 거리, 초기값은 임의로 큰 값 (100)
        float diff = 100;

        // 탐지된 모든 객체들을 반복 처리
        foreach (RaycastHit2D target in targets)
        {
            // 스캐너(현재 객체)의 위치
            Vector3 myPos = transform.position;
            // 탐지된 대상 객체의 위치
            Vector3 targetPos = target.transform.position;
            // 스캐너와 대상 객체 사이의 거리 계산
            float curDiff = Vector3.Distance(myPos, targetPos);

            // 만약 계산된 거리가 현재까지의 최소 거리보다 작다면
            if (curDiff < diff)
            {
                // 최소 거리를 갱신하고, 가장 가까운 객체를 result에 저장
                diff = curDiff;
                result = target.transform;
            }
        }

        // 가장 가까운 객체의 Transform을 반환 (없을 경우 null)
        return result;
    }
}
