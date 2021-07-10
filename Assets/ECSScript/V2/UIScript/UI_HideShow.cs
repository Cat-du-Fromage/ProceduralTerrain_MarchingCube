using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace KaizerWaldCode
{
    public class UI_HideShow : MonoBehaviour
    {
        [SerializeField] private UI_InputsField inputField;
        // Start is called before the first frame update
        void Start()
        {
            //transform.Find("UI_MapSettings").GetComponent<Button>().onClick = HideShow(inputField.gameObject);
        }

        void HideShow(GameObject gameObject)
        {
            if (gameObject.activeSelf)
            {
                inputField.Hide();
            }
            else
            {
                inputField.Show();
            }
        }

    }
}
