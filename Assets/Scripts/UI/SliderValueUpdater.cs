using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderValueUpdater : MonoBehaviour
    {
        public TextMeshProUGUI valueLabel;

        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(UpdateLabel);
            UpdateLabel(_slider.value);
        }

        private void UpdateLabel(float value)
        {
            valueLabel.text = Mathf.FloorToInt(value).ToString();
        }
    }
}
