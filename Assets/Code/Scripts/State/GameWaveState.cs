using Code.Scripts.ScriptableScripts;
using UnityEngine;

namespace Code.Scripts.State
{
    public class GameWaveState : GameState
    {
        [SerializeField] private EnemyState EnemyState;

        protected override void GameStateLogic(int WaveNumber)
        {
            this.EnemyState.AddBaseHealth(WaveNumber * 10);
            this.EnemyState.AddBaseDamage(WaveNumber * 10);
            
            GameStateEnd();
        }

        protected override void GameStateEnd()
        {
        }
    }
}