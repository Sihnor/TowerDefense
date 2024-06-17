using System;
using Code.Scripts.ScriptableScripts;
using UnityEngine;

namespace Code.Scripts.State
{
    public enum EState
    {
        Wave,
        Build,
        GameOver
    }
    public class GameMode : MonoBehaviour
    {
        public static GameMode Instance;
        [SerializeField] private PlayersState PlayersState;

        private GameWaveState GameWaveState;
        private GameBuildState GameBuildState;
        
        private int NumberOfWaves = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        
        public void Start()
        {
            this.PlayersState.SetGold(100);
            
            this.GameWaveState = gameObject.AddComponent<GameWaveState>();
            this.GameBuildState = gameObject.AddComponent<GameBuildState>();
            
            this.GameWaveState.GameStateStart(this.NumberOfWaves);
            
            StartBuildPhase();
        }

        public void StartWavePhase()
        {
            this.NumberOfWaves++;
            
            Debug.Log("Wave Phase Started");
        }
        
        public void StartBuildPhase()
        {
            this.NumberOfWaves++;

            this.PlayersState.AddGold(this.NumberOfWaves * 5);
            Debug.Log("Build Phase Started");
        }
        
    }
}