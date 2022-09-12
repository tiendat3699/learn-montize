using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textGold : MonoBehaviour
{
    private TextMeshProUGUI TextUI;
    private void Awake() {
        TextUI = GetComponent<TextMeshProUGUI>();
        CurrencyManager.Instance.OnUpdateGold.AddListener((gold)=> {
            string goldText = gold <= 99999? gold.ToString():"99999+"; 
            TextUI.text = goldText;
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        double gold = CurrencyManager.Instance.Gold;
        string goldText = gold <= 99999? gold.ToString():"99999+"; 
        TextUI.text = goldText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
