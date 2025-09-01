using UnityEngine;
using UnityEngine.UI;

namespace Game.Gameplay.AbilityManagement
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "Abilities/AbilityData")]
    public class AbilityData : ScriptableObject
    {
        public string abilityName;
        public Image abilityIcon;
        private Ability ability;
    }
}
