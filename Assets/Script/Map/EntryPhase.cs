using UnityEngine;

public class EntryPhase : MonoBehaviour
{
    public SpawnManager spawnManager;
    public BoundSpawnManager boundSpawnManager;
    public float spawnIntervalChanged;
    public float boundspawnIntervalChanged;

    void Start()
    {
        if (spawnManager == null)
        {
            Debug.LogError("오류: 이 오브젝트에 SpawnManager 컴포넌트가 없습니다! EntryPhase2 스크립트와 같은 오브젝트에 SpawnManager를 추가해야 합니다.", this);
        }

        if (boundSpawnManager == null)
        {
            Debug.LogError("오류: 이 오브젝트에 SpawnManager 컴포넌트가 없습니다! EntryPhase2 스크립트와 같은 오브젝트에 SpawnManager를 추가해야 합니다.", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Phase2 진입 소환 빈도가 빨라집니다.");
            spawnManager.spawnInterval = spawnIntervalChanged;
            boundSpawnManager.spawnInterval = boundspawnIntervalChanged;
        }
    }
}
