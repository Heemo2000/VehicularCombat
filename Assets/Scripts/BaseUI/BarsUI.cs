using UnityEngine;
using UnityEngine.UI;

namespace Game.BaseUI
{
    [RequireComponent(typeof(Image))]
    public class BarsUI : MonoBehaviour
    {
        [Min(0.01f)]
        [SerializeField] private float fillSpeed = 10.0f;
        private Image bar;
        private float currentAmount;
        public void SetAmount(float currentAmount)
        {
            this.currentAmount = currentAmount;
        }
        public void SetAmount(float currentAmount, float maxAmount)
        {
            currentAmount = Mathf.Clamp(currentAmount, 0.0f, maxAmount);
            this.currentAmount = currentAmount/maxAmount;
        }

        private void HandleFillPercent()
        {
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, this.currentAmount, fillSpeed * Time.deltaTime);
        }

        private void Awake() {
            bar = GetComponent<Image>();
        }
        
        void Start()
        {
            bar.type = Image.Type.Filled;
            bar.fillMethod = Image.FillMethod.Horizontal;
        }

        private void LateUpdate()
        {
            HandleFillPercent();
        }
    }
}
