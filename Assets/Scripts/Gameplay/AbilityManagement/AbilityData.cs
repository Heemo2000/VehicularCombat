using UnityEngine;
using UnityEngine.UI;

namespace Game.Gameplay.AbilityManagement
{
    [System.Serializable]
    public class AbilityData
    {
        public string abilityName;
        public Image abilityIcon;
        [SerializeReference]
        public Ability ability;
    }
}
