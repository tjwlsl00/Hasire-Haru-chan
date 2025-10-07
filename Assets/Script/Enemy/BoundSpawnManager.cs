using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoundSpawnManager : MonoBehaviour
{
    #region 내부 변수 
    public GameObject SpawnObjectPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 10f;
    public int minSpawnCount = 1;
    public int maxSpawnCount = 3;
    #endregion

    #region 충돌 방지 설정
    public float overlapCheckRadius = 0.5f;
    public LayerMask collisionLayer;
    private List<Transform> availableSpawnPoints = new List<Transform>();
    private Coroutine spawnCoroutine;
    #endregion

    void Start()
    {
        initializeAvailableSpawnPoints();

        if (SpawnObjectPrefab != null && spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnCoroutine = StartCoroutine(SpawnObjectRoutine());
        }
        else
        {
            Debug.LogError("ObjectSpawner: 프리팹 또는 스폰 포인트가 설정되지 않았습니다!");
        }
    }

    void initializeAvailableSpawnPoints()
    {
        availableSpawnPoints.Clear();
        foreach (Transform point in spawnPoints)
        {
            availableSpawnPoints.Add(point);
        }
    }

    IEnumerator SpawnObjectRoutine()
    {
        while (GameManager.currentState != GameState.Playing)
        {
            yield return null;
        }

        Debug.Log("게임 시작! 첫 스폰까지 5초 대기합니다.");
        yield return new WaitForSeconds(spawnInterval);

        while (GameManager.currentState == GameState.Playing)
        {
            SpawnObjects();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnObjects()
    {
        // 이번 스폰에서 생성할 오브젝트 개수를 랜덤으로 결정
        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

        // 생성 가능한 스폰 포인트가 부족하면 최대 개수를 조절
        if (spawnCount > spawnPoints.Length)
        {
            spawnCount = spawnPoints.Length;
        }

        // 사용할 수 있는 스폰 포인트를 추려냅니다.
        List<Transform> currentAvailablePoints = new List<Transform>();
        foreach (Transform sp in spawnPoints)
        {
            bool isOccupied = Physics.CheckSphere(sp.position, overlapCheckRadius, collisionLayer);
            if (!isOccupied)
            {
                currentAvailablePoints.Add(sp);
            }
        }

        // 사용할 수 있는 스폰 포인트가 요청된 개수보다 적으면, 가능한 만큼만 생성
        if (spawnCount > currentAvailablePoints.Count)
        {
            spawnCount = currentAvailablePoints.Count;
            if (spawnCount == 0)
            {
                return;
            }
        }

        ShuffleList(currentAvailablePoints);

        for (int i = 0; i < spawnCount; i++)
        {
            if (i >= currentAvailablePoints.Count) break;
            Transform randomSpawnPoint = currentAvailablePoints[i];

            // 오브젝트 생성
            GameObject spawnedObject = Instantiate(SpawnObjectPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);

            // 생성된 오브젝트를 스포너의 자식으로 만들어 정리 
            spawnedObject.transform.parent = transform;
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    //Gizmos로 스폰 포인트를 에디터에서 시각화
    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;
        Gizmos.color = Color.cyan;
        foreach (Transform point in spawnPoints)
        {
            if (point != null)
            {
                // 스폰 위치
                Gizmos.DrawWireSphere(point.position, overlapCheckRadius);
                // 겹침 확인 영역
                Gizmos.DrawCube(point.position, Vector3.one * 0.1f);
            }
        }
    }
}
