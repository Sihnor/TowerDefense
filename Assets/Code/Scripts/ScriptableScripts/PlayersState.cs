using UnityEngine;

namespace Code.Scripts.ScriptableScripts
{
    [CreateAssetMenu(fileName = "PlayerState", menuName = "ScriptableObject/PlayerState", order = 0)]
    public class PlayersState : ScriptableObject
    {
        private int Gold = 0;
        private int Lives = 0;
        
        
        public int GetGold()
        {
            return this.Gold;
        }
        
        public void SetGold(int gold)
        {
            this.Gold = gold;
        }
        
        public void AddGold(int gold)
        {
            this.Gold += gold;
        }
        
        public int GetLives()
        {
            return this.Lives;
        }
        
        public void SetLives(int lives)
        {
            this.Lives = lives;
        }
        
        
    }
}