// Authors: Kalby Jang
// Copyright © 2021 DigiPen - All Rights Reserved

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GG.Utilities
{
    public class ObjectPool : MonoBehaviour
    {
        [Serializable]
        public class Pool
        {
            public string     tag;
            public GameObject prefab;
            public int        size;
        }

        #region Singleton
    
        public static ObjectPool Instance;

        private void Awake()
        {
            Instance = this;
        }

        #endregion


        #region Class Members

        public List<Pool>                            pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;
    
        #endregion

        #region Unity Methods
    
        void Start()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectQueue = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject newObj = Instantiate( pool.prefab );
                    newObj.SetActive( false );
                    objectQueue.Enqueue(newObj);
                }
            
                poolDictionary.Add(pool.tag, objectQueue);
            }
        }

    
        #endregion

        #region Class Methods

        public GameObject GetObject( string tag, Vector3 position, Quaternion rotation )
        {
            if (!poolDictionary.ContainsKey( tag ))
            {
                Debug.LogWarning( "Pool with tag " + tag + " doesn't exist." );
                return null;
            }
        
            GameObject queuedObj = poolDictionary[tag].Dequeue();

            IPooledObject pooledObject = queuedObj.GetComponent<IPooledObject>();

            queuedObj.SetActive(true);
            queuedObj.transform.position = position;
            queuedObj.transform.rotation = rotation;
            
            pooledObject?.OnObjectSpawn();
        
            poolDictionary[tag].Enqueue(queuedObj); 

            return queuedObj;
        }

        #endregion

    }
}
