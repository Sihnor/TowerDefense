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
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, this.LayerMask)) return;
            
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                if (tile.GetTileType() != ENodeState.Open) return;
                if (((IStackable)tile).CanBePlaced() == false) return;
                
                IStackable lastPlace = ((IStackable)tile).GetLastPlace();
                
                Vector3 pos = (lastPlace).GetPositionForPlacement();
                
                GameObject tower = Instantiate(this.TowerPrefab, pos, Quaternion.identity);

                IStackable towerStack = tower.GetComponent<IStackable>();
                lastPlace.SetPlaceable(towerStack);
            }
        }
    }
}