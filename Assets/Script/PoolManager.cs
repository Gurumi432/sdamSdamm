using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
1) 게임상 기능  
    오브젝트 풀링: 필요할 때마다 새로 생성하는 것이 아니라, 미리 생성된 오브젝트를 재사용하여 성능 최적화  
    게임 내 최적화: 오브젝트의 반복적인 생성/삭제를 최소화하여 CPU와 메모리 사용량 절감  
    다양한 프리팹 관리: 여러 개의 프리팹을 풀링하여, 몬스터, 총알, 효과 등 다양한 오브젝트 관리 가능  
   
2) 주요 로직  
    프리팹 배열(`prefabs`): 게임 내에서 사용될 다양한 오브젝트 원본을 보관  
    오브젝트 풀(`pools`): 각 프리팹에 대한 리스트 배열을 생성하여 재사용 가능한 오브젝트 저장  
    객체 요청(`Get`):  
      기존 풀에서 비활성화된 오브젝트 탐색 후 활성화하여 반환  
      사용 가능한 오브젝트가 없으면 새로 생성 후 풀에 추가  
   
3) 단계별 전개 흐름  
   1. 초기화(`Awake`)  
       프리팹 배열의 길이만큼 풀 리스트(`pools[]`) 배열을 생성  
       각 리스트를 개별적으로 초기화하여 오브젝트를 저장할 준비  
   2. 객체 요청(`Get`)  
       특정 프리팹 인덱스(`index`)를 기반으로 객체 풀에서 비활성화된 오브젝트 검색  
       사용 가능한 오브젝트가 있으면 활성화 후 반환  
       사용 가능한 오브젝트가 없으면 새롭게 생성하여 풀에 추가 후 반환  
*/

public class PoolManager : MonoBehaviour
{
    // 에디터에서 할당할 프리팹 배열
    public GameObject[] prefabs;

    // 각 프리팹마다 GameObject를 관리할 풀 리스트 배열
    private List<GameObject>[] pools;

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
            pools[index].Add(select);
        }

        return select;
    }
}
