using System.Threading.Tasks;
using com.cyborgAssets.inspectorButtonPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnObject : MonoBehaviour
{
    public Vector2 DelayTime = new Vector2(0.01f,0.4f);
    public GameObject ObjectSpawn;
    public Transform SpaceSpawner;
    public int count;

    [ProPlayButton]
    public async void SPAWN()
    {
        if(count == 0 || !ObjectSpawn || !AgentMovementManager.Instance) return;
        for (int i = 0; i < count; i++)
        {
            await Task.Delay((int)(Random.Range(DelayTime.x, DelayTime.y) * 1000));
            int random = Random.Range(0, SpaceSpawner.childCount);
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(0f, 1f);
            float z = Random.Range(-5f, 5f);
            Transform tf = Instantiate(ObjectSpawn, SpaceSpawner.GetChild(random).position + new Vector3(x,y,z),quaternion.identity).transform;
            AgentMovementManager.Instance.AddTf(tf);
        }
    }


}