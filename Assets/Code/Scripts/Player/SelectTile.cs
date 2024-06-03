using System;
using Code.Scripts.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using Code.Scripts.Generation;

namespace Code.Scripts.Player
{
    public class SelectTile : MonoBehaviour
    {
        [SerializeField] private LayerMask LayerMask;
        [SerializeField] private GameObject TowerPrefab;
        
        private void Awake()
        {
            GetComponent<PlayerInput>().actions["Action"].performed += OnSelectTile;
        }
        
        private void OnSelectTile(InputAction.CallbackContext context)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, this.LayerMask)) return;
            
            Node tile = hit.collider.GetComponent<Node>();
            if (tile != null)
            {
                if (tile.GetTileType() != ENodeState.Open) return;
                if (!(tile as IStackable).CanBePlaced()) return;
                
                IStackable lastPlace = (tile as IStackable).GetLastPlace();
                
                Vector3 pos = (lastPlace).GetPosition();
                
                GameObject tower = Instantiate(this.TowerPrefab, pos, Quaternion.identity);

                var towerStack = tower.GetComponent<IStackable>();
                lastPlace.SetPlaceable(towerStack);
            }
        }
    }
}