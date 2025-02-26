using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 에디터에서 할당할 프리팹 배열
    public GameObject[] prefabs;

    // 각 프리팹마다 GameObject를 관리할 풀 리스트 배열
    private List<GameObject>[] pools;

    // 삭제 주기를 Unity 에디터에서 조절 가능 (기본값: 20초)
    public float deletionPeriod = 20.0f;

    void Awake()
    {
        // 프리팹 배열 길이만큼 풀 리스트 배열 초기화
        pools = new List<GameObject>[prefabs.Length];

        // 각 요소마다 새로운 리스트 생성
        for (int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }
    }

    void Start()
    {
        // 주기적으로 비활성화 오브젝트 삭제를 실행하는 코루틴 시작
        StartCoroutine(DeleteInactiveRoutine());
    }

    /// <summary>
    /// 지정된 인덱스의 오브젝트를 풀에서 가져옵니다.
    /// </summary>
    /// <param name="index">프리팹 배열의 인덱스</param>
    /// <returns>사용 가능한 GameObject</returns>
    public GameObject Get(int index)
    {
        GameObject select = null;

        // 선택한 풀에서 비활성화된 게임 오브젝트 검색
        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // 사용 가능한 오브젝트가 없으면 새로 생성하여 풀에 추가
        if (select == null)
        {
            select = Instantiate(prefabs[index], transform);
            // Poolable 컴포넌트가 없다면 추가 (비활성화 시 시간 기록용)
            if (select.GetComponent<Poolable>() == null)
            {
                select.AddComponent<Poolable>();
            }
            pools[index].Add(select);
        }

        return select;
    }

    // 지정한 주기마다 비활성화 오브젝트를 삭제하는 코루틴
    IEnumerator DeleteInactiveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(deletionPeriod);
            DeleteInactiveObjects();
        }
    }

    // 각 풀의 비활성화된 오브젝트 중 20초 이상 재활성화되지 않은 오브젝트만 삭제
    void DeleteInactiveObjects()
    {
        for (int i = 0; i < pools.Length; i++)
        {
            // 리스트를 역순으로 순회하여 삭제 시 인덱스 오류를 방지
            for (int j = pools[i].Count - 1; j >= 0; j--)
            {
                GameObject obj = pools[i][j];
                if (!obj.activeSelf)
                {
                    Poolable poolable = obj.GetComponent<Poolable>();
                    if (poolable != null)
                    {
                        // 현재 시간과 비활성화된 시간의 차이가 deletionPeriod 이상인 경우 삭제
                        if (Time.time - poolable.deactivatedTime >= deletionPeriod)
                        {
                            Destroy(obj);
                            pools[i].RemoveAt(j);
                        }
                    }
                    else
                    {
                        // 만약 Poolable 컴포넌트가 없다면 바로 삭제
                        Destroy(obj);
                        pools[i].RemoveAt(j);
                    }
                }
            }
        }
    }
}

// Poolable 클래스: 오브젝트가 비활성화될 때 비활성화 시각을 기록하기 위한 컴포넌트
public class Poolable : MonoBehaviour
{
    public float deactivatedTime;

    // 오브젝트가 비활성화될 때 호출됨
    void OnDisable()
    {
        deactivatedTime = Time.time;
    }
}
