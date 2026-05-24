using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform _spawner;
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private float _delay = 1f;
    [SerializeField] private int _poolCapacity = 20;
    [SerializeField] private int _poolMaxSize = 20;

    private float _shiftPositionX = 10f;
    private float _shiftPositionZ = 10f;
    private WaitForSecondsRealtime _wait;
    private ObjectPool<Cube> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Cube>(
            createFunc: () => Instantiate(_cubePrefab),
            actionOnGet: (obj) => ActionOnGet(obj),
            actionOnRelease: (obj) => ActionOnRelease(obj),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _poolMaxSize);

        _wait = new WaitForSecondsRealtime(_delay);
    }

    private void Start()
    {
        StartCoroutine(CreateCubes());
    }

    private void ActionOnGet(Cube obj)
    {
        obj.transform.position = GetStartPosition();
        obj.gameObject.SetActive(true);
        obj.LifeExpired += ReturnToPool;
    }

    private void ActionOnRelease(Cube obj)
    {
        obj.ResetState();
        obj.gameObject.SetActive(false);
        obj.LifeExpired -= ReturnToPool;
    }

    private Vector3 GetStartPosition()
    {
        Vector3 newPosition = new(
            Random.Range(_spawner.transform.position.x - _shiftPositionX, _spawner.transform.position.x + _shiftPositionX),
            _spawner.transform.position.y,
            Random.Range(_spawner.transform.position.z - _shiftPositionZ, _spawner.transform.position.z + _shiftPositionZ));

        return newPosition;
    }

    private IEnumerator CreateCubes()
    {
        while (true)
        {
            yield return _wait;

            _pool.Get();
        }
    }

    private void ReturnToPool(Cube cube)
    {
        _pool.Release(cube);
    }
}