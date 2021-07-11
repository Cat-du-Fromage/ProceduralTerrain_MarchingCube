using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace KaizerWaldCode.V2.UI
{
    public class UI_Slider : MonoBehaviour
    {
        // Start is called before the first frame update
        public TMP_InputField sliderTextInput;
        public Slider SliderValue;
        void Start()
        {
            
        }

        public void ChangeSliderValue()
        {
            int SliderVal = (int) SliderValue.value;
            sliderTextInput.text = SliderVal < 2 ? "": SliderVal.ToString();
        }
    }
}
