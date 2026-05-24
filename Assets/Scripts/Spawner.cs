using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform _spawner;
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private Platform[] _platforms;
    [SerializeField] private float _repeatRate = 1f;
    [SerializeField] private int _poolCapacity = 20;
    [SerializeField] private int _poolMaxSize = 20;

    private float _delay = 0f;
    private float _shiftPositionX = 10f;
    private float _shiftPositionZ = 10f;

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
    }

    private void OnEnable()
    {
        foreach (var platform in _platforms) 
        {
            platform.CubeCrashed += TryReturnToPool;
        }
    }

    private void OnDisable()
    {
        foreach (var platform in _platforms)
        {
            platform.CubeCrashed -= TryReturnToPool;
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetCube), _delay, _repeatRate);
    }

    private void ActionOnGet(Cube obj)
    {   
        obj.transform.position = GetStartPosition();
        obj.gameObject.SetActive(true);
    }

    private void ActionOnRelease(Cube obj) 
    {
        obj.ResetState();
        obj.gameObject.SetActive(false);
    }

    private void GetCube()
    {
        _pool.Get();
    }

    private Vector3 GetStartPosition() 
    {
        Vector3 newPosition = new(
            Random.Range(_spawner.transform.position.x - _shiftPositionX, _spawner.transform.position.x + _shiftPositionX),
            _spawner.transform.position.y,
            Random.Range(_spawner.transform.position.z - _shiftPositionZ, _spawner.transform.position.z + _shiftPositionZ));

        return newPosition;
    }

    public void TryReturnToPool(Cube cube) 
    {
        if (cube.HaveDefaulColor)
        {
            cube.ChangeColor();
            float lifeTime = cube.GetLifeTime();
            StartCoroutine(ReturnToPool(cube, lifeTime));
        }
    }

    private IEnumerator ReturnToPool(Cube cube, float delay) 
    {
        yield return new WaitForSecondsRealtime(delay);
        
        _pool.Release(cube);
    }
}