using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace KaizerWaldCode.V2.UI
{
    public class UI_HideShow : MonoBehaviour
    {
        //[SerializeField] private UI_InputsField inputField;
        // Start is called before the first frame update

        public void HideShow()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

    }
}
