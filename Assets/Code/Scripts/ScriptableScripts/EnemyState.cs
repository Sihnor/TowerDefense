using UnityEngine;

namespace Code.Scripts.ScriptableScripts
{
    [CreateAssetMenu(fileName = "EnemyState", menuName = "ScriptableObject/EnemyState", order = 0)]
    public class EnemyState : ScriptableObject
    {
        [SerializeField] private int BaseHealth = 100;
        [SerializeField] private int BaseSpeed = 10;
        [SerializeField] private int BaseDamage = 10;
        [SerializeField] private int BaseGold = 10;
        
        public int GetBaseHealth()
        {
            return this.BaseHealth;
        }
        
        public void AddBaseHealth(int health)
        {
            this.BaseHealth += health;
        }
        
        public int GetBaseSpeed()
        {
            return this.BaseSpeed;
        }
        
        public int GetBaseDamage()
        {
            return this.BaseDamage;
        }
        
        public void AddBaseDamage(int damage)
        {
            this.BaseDamage += damage;
        }
        
        public int GetBaseGold()
        {
            return this.BaseGold;
        }
    }
}