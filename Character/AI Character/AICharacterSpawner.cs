using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace RK
{
    /// <summary>
    /// aicharacterのspawner
    /// このコンポーネントを持つオブジェクトを配置するとstartした際にaicharacterもinstantiateされる
    /// </summary>
    public class AICharacterSpawner : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] GameObject characterGameObject;
        [SerializeField] GameObject instantiatedGameObject;
        private void Awake()
        {

        }
        private void Start()
        {
            WorldAIManager.instance.SpawnCharacter(this);
            gameObject.SetActive(false);
        }
        public void AttemptToSpawnCharacter()
        {
            if (characterGameObject != null)
            {
                instantiatedGameObject = Instantiate(characterGameObject);
                instantiatedGameObject.transform.position = transform.position;
                instantiatedGameObject.transform.rotation = transform.rotation;
                instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
                WorldAIManager.instance.AddCharacterToSpawnedCharactersList(instantiatedGameObject.GetComponent<AICharacterManager>());
            }
        }
        public void SetCharacter(GameObject characterObject)
        {
            characterGameObject = characterObject;
        }
    }
}
