using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KaizerWaldCode.V2.UI
{
    public class GenerateButton : MonoBehaviour
    {
        public string InputIsoSurface;

        public string InputBounds;

        public UI_InputsField test;
        public TextMeshProUGUI test23;
        public TMP_InputField test24;

        // Start is called before the first frame update
        void Start()
        {
            ReadInputs(InputIsoSurface, InputBounds);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void ReadInputs(params string[] inputs)
        {
            foreach (string s in inputs)
            {
                Debug.Log(s);
            }
        }

        public void ReadInput(string input)
        {
            InputIsoSurface = input;
            Debug.Log(InputIsoSurface);
        }
    }
}
