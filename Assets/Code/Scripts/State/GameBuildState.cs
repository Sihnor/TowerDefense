using Code.Scripts.ScriptableScripts;
using UnityEngine;

namespace Code.Scripts.State
{
    public class GameBuildState : GameState
    {
        [SerializeField] private PlayersState PlayersState;
        [SerializeField] private int GoldPerWave = 5;

        protected override void GameStateLogic(int WaveNumber)
        {
            this.PlayersState.AddGold(WaveNumber * this.GoldPerWave);
            
            GameStateEnd();
        }

        protected override void GameStateEnd()
        {
        }
    }
}