using System;
using System.Collections;
using System.Collections.Generic;
using Playmode.Ennemy.Strategies;
using Playmode.Util.Collections;
using UnityEngine;
using Random = System.Random;

namespace Playmode.Pickable
{
    public class PickableSpawner : MonoBehaviour
    {
        
        private Camera cam;
        private float maxGameWidth;
        private float camViewWidth;
        private float camViewHeight;
        private static readonly PickableBehaviour[] defaultPickableBehaviours =
        {
            PickableBehaviour.Medicalkit,
            PickableBehaviour.Shotgun,
            PickableBehaviour.Uzi
        };
        
        [SerializeField] private GameObject pickablePrefab;
        [SerializeField] private int numberOfPickable = 4;
        private void Awake()
        {
            camViewHeight = cam.orthographicSize * 2f;
            camViewWidth = camViewHeight * cam.aspect;
            ValidateSerialisedFields();
        }
        
        private void ValidateSerialisedFields()
        {
            if (pickablePrefab == null)
                throw new ArgumentException("Can't spawn null ennemy prefab.");
        }
        
        private void Start()
        {
            SpawnPickables();
        }
        private void SpawnPickables()
        {
            var pickableStragegyProvider = new LoopingEnumerator<PickableBehaviour>(defaultPickableBehaviours);
            for (int i = 0; i < numberOfPickable; ++i)
            {
                SpawnPickable(
                        CreateRandomCoordonate(),
                    pickableStragegyProvider.Next()
                );
            }
            
                
        }

        private Vector2 CreateRandomCoordonate()
        {
            return new Vector2(UnityEngine.Random.Range(0, camViewWidth),
                UnityEngine.Random.Range(0, camViewHeight));
        }
        private void SpawnPickable(Vector3 position, PickableBehaviour strategy)
        {
            Instantiate(pickablePrefab, position, Quaternion.identity)
                .GetComponentInChildren<PickableController>()
                .Configure(strategy);
        }
        
    }
}

