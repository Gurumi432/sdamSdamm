using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;
    // 인스펙터에서 자식 Bullet의 회전 잠금 여부를 선택할 수 있도록 함
    public bool lockChildRotation = true;

    void Start()
    {
        Init();
    }

    void Update()
    {
        switch (id)
        {
            case 7:
                // 부모 오브젝트 회전
                transform.Rotate(Vector3.back * speed * Time.deltaTime);

                // lockChildRotation이 true일 경우 자식 Bullet의 월드 회전을 고정함
                if (lockChildRotation)
                {
                    foreach (Transform child in transform)
                    {
                        child.rotation = Quaternion.identity;
                    }
                }
                break;
            default:
                break;
        }
    }

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
        if (GameManager.instance == null) return;
        if (GameManager.instance.pool == null) return;

        for (int index = 0; index < count; index++)
        {
            GameObject pooledObj = GameManager.instance.pool.Get(prefabId);
            if (pooledObj == null) continue;

            Transform bulletTransform = pooledObj.transform;
            if (bulletTransform == null) continue;

            bulletTransform.parent = transform;

            Bullet bulletComp = bulletTransform.GetComponent<Bullet>();
            if (bulletComp == null) continue;

            bulletComp.Init(damage, -1); // -1은 Infinity Per 의미임.
        }
    }
}
