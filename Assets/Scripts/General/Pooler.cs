namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Pooler : UpdateAbstract
    {
        [SerializeField]
        private Entity _entity;
    
        public List<Pool> pools;
        public Dictionary<string, GameObject[]> poolDict;
    
        public int[] queue;
        public bool makeSpawnedObjectsChild;
    
        void Start()
        {
            poolDict = new Dictionary<string, GameObject[]>();
            foreach (var pool in pools)
            {
                GameObject[] objPool = new GameObject[pool.size];
                for (int i = 0; i < objPool.Length; i++)
                {
                    var obj = Instantiate(pool.prefab);
    
                    if (obj.TryGetComponent(out PauseAudioOnFreeze freeze))
                    {
                        if (freeze.entity == null)
                            freeze.entity = _entity;
                    }
                    if (obj.TryGetComponent(out AudioSource audio))
                    {
                        audio.outputAudioMixerGroup = FighterAssistList.instance.effectMixerGroup;
                    }
                    if (obj.TryGetComponent(out Entity entity))
                    { }
                    else
                        obj.SetActive(false);
    
                    objPool[i] = obj;
                }
                poolDict.Add(pool.tag, objPool);
            }
            queue = new int[pools.Count];
        }
    
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Vector3 size)
        {
            if (!poolDict.ContainsKey(tag))
            {
                Debug.Log("Pool tag" + tag + "doesn't exist");
                return null;
            }
            var count = 0;
    
            for (int i = 0; i < pools.Count; i++)
            {
                Pool pool = pools[i];
                if (pool.tag == tag)
                {
                    count = queue[i];
                    queue[i] = count + 1;
                    if (queue[i] >= pool.size)
                        queue[i] = 0;
                }
            }
    
            var objToSpawn = poolDict[tag][count];
    
            if (makeSpawnedObjectsChild)
                objToSpawn.transform.SetParent(transform);
            objToSpawn.SetActive(true);
            objToSpawn.transform.position = position;
            objToSpawn.transform.rotation = rotation;
            objToSpawn.transform.localScale = size;
            if (objToSpawn.TryGetComponent(out Animator anim))
                anim.PlayInFixedTime(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
            if (objToSpawn.TryGetComponent(out AudioSource audio))
                audio.Play();
    
            return objToSpawn;
        }
        public Entity SpawnEntityFromPool(string tag, Vector3 position, Quaternion rotation, Vector3 size, Vector2 speed = default(Vector2))
        {
            if (!poolDict.ContainsKey(tag))
            {
                Debug.Log("Pool tag" + tag + "doesn't exist");
                return null;
            }
            var count = 0;
    
            for (int i = 0; i < pools.Count; i++)
            {
                Pool pool = pools[i];
                if (pool.tag == tag)
                {
                    count = queue[i];
                    queue[i] = count + 1;
                    if (queue[i] >= pool.size)
                        queue[i] = 0;
                }
            }
            if (!poolDict[tag][count].TryGetComponent(out Entity objToSpawn))
            {
                Debug.Log("Pool tag" + tag + " doesn't have entity");
                return null;
            }
            if (_entity != null)
                objToSpawn.stateVars.givenTime = _entity.stateVars.givenTime;
    
            if (objToSpawn.TryGetComponent(out BaseProjectileBehaviour proj) && _entity != null)
            {
                objToSpawn.stateVars.givenTime = 0;
                if (proj.preventRespawnWhileActive && proj.entity.stateVars.enabled)
                    return null;
                if (proj.preventRespawnWhileLifetimeNotExpired && proj.LifeTime < proj.MaxLifeTime)
                    return null;
            }
    
            if (makeSpawnedObjectsChild)
                objToSpawn.transform.SetParent(transform);
            objToSpawn.SkipCurrentAnimToFrame(0);
            objToSpawn.SetEntityActive(true);
            objToSpawn.transform.position = position;
            objToSpawn.transform.rotation = rotation;
            objToSpawn.transform.localScale = size;
            objToSpawn.stateVars.indieSpd = speed;
            objToSpawn.OnSpawn.Invoke();
    
            if (proj != null && _entity != null)
                proj.OnShoot(_entity);
            if (objToSpawn.TryGetComponent(out AudioSource audio))
                audio.Play();
    
            return objToSpawn;
        }
        public void SpawnEntityFromPool()
        {
            if (pools[0] == null)
            {
                Debug.Log("Nothing in pool!");
                return;
            }
            var count = 0;
    
            for (int i = 0; i < pools.Count; i++)
            {
                Pool pool = pools[i];
                count = queue[i];
                queue[i] = count + 1;
                if (queue[i] >= pool.size)
                    queue[i] = 0;
            }
            if (!poolDict[pools[0].tag][count].TryGetComponent(out Entity objToSpawn))
            {
                Debug.Log("Pool tag" + tag + " doesn't have entity");
                return;
            }
            if (_entity != null)
                objToSpawn.stateVars.givenTime = _entity.stateVars.givenTime;
    
            if (objToSpawn.TryGetComponent(out BaseProjectileBehaviour proj) && _entity != null)
            {
                objToSpawn.stateVars.givenTime = 0;
                if (proj.preventRespawnWhileActive && proj.entity.stateVars.enabled)
                    return;
            }
    
            if (makeSpawnedObjectsChild)
                objToSpawn.transform.SetParent(transform);
            objToSpawn.SkipCurrentAnimToFrame(0);
            objToSpawn.SetEntityActive(true);
            objToSpawn.transform.position = transform.position;
            objToSpawn.transform.rotation = transform.rotation;
            objToSpawn.transform.localScale = transform.lossyScale.y * pools[0].prefab.transform.lossyScale;
            objToSpawn.stateVars.indieSpd = Vector2.zero;
            objToSpawn.OnSpawn.Invoke();
    
            if (proj != null && _entity != null)
                proj.OnShoot(_entity);
            if (objToSpawn.TryGetComponent(out AudioSource audio))
                audio.Play();
        }
    
        public GameObject GetOGPrefabInfo(string tag)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                Pool pool = pools[i];
                if (pool.tag == tag)
                {
                    return pool.prefab;
                }
            }
            Debug.Log("Prefab Not Found!");
            return null;
        }
    
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }
    }
}
