using UnityEngine;
using UnityEngine.UI;

namespace Game.BaseUI
{
    [RequireComponent(typeof(Image))]
    public class BarsUI : MonoBehaviour
    {
        private Image bar;

        public void SetAmount(float currentAmount)
        {
            bar.fillAmount = currentAmount;
        }
        public void SetAmount(float currentAmount, float maxAmount)
        {
            currentAmount = Mathf.Clamp(currentAmount, 0.0f, maxAmount);
            bar.fillAmount = currentAmount/maxAmount;
        }

        private void Awake() {
            bar = GetComponent<Image>();
        }
        
        void Start()
        {
            bar.fillMethod = Image.FillMethod.Horizontal;
        }
    }
}
