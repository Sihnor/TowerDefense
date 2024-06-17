using UnityEngine;

namespace Code.Scripts.State
{
    public abstract class GameState : MonoBehaviour
    {
        public  void GameStateStart(int WaveNumber)
        {
            GameStateLogic(WaveNumber);
        }

        protected abstract void GameStateLogic(int WaveNumber);

        protected abstract void GameStateEnd();
    }
}