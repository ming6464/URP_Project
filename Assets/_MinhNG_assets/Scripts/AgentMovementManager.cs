using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

class AgentMovementManager : Singleton<AgentMovementManager>
{

    
    [SerializeField,Range(0,100000)] private int _length;
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _direct = new Vector3(0,0,-1);
    private int indexMax = -1;
    private Transform[] _transforms;
    TransformAccessArray _accessArray;
    private List<Transform> _listAdd;
    
    private JobHandle jobHandle;

    public override void Awake()
    {
        _transforms = new Transform[_length];
        _accessArray = new TransformAccessArray(_transforms);
        _listAdd = new List<Transform>();
    }

    void OnDestroy()
    {
        _accessArray.Dispose();
    }

    void Update()
    {
        jobHandle.Complete();

        foreach (Transform tfChild in _listAdd)
        {
            if(!tfChild) continue;
            indexMax++;
            _transforms[indexMax] = tfChild;
        }
        
        if(indexMax < 0) return;

        if (_listAdd.Count > 0)
        {
            _listAdd.Clear();
            _accessArray.Dispose();
            _accessArray = new TransformAccessArray(_transforms);
        }
        
        
        // Create the job
        var job = new VelocityJob
        {
            deltaTime = Time.deltaTime,
            speed = _speed,
            direct = _direct,
        };

        // Schedule the parallel-for-transform job
        jobHandle = job.Schedule(_accessArray);
    }


    #region pulic methods

    public void AddTf(Transform tf)
    {
        _listAdd.Add(tf);
    }

    public void RemoveTf()
    {
        
    }

    #endregion
    
    [BurstCompatible]
    public struct VelocityJob : IJobParallelForTransform
    {
        [ReadOnly] public Vector3 direct;
        [ReadOnly]public float deltaTime;
        [ReadOnly]public float speed;
        
        // Execute method runs for each transform
        public void Execute(int index, TransformAccess transform)
        {
            Debug.Log("hello");
            // Move the transforms based on velocity and delta time
            var pos = transform.position;
            pos += direct * (deltaTime * speed);
            transform.position = pos;
        }
    }
}