using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textGem : MonoBehaviour
{
    private TextMeshProUGUI TextUI;
    private void Awake() {
        TextUI = GetComponent<TextMeshProUGUI>();
        CurrencyManager.Instance.OnUpdateCurrency.AddListener((gold,gem)=> {
            string gemText = gem <= 99999? gem.ToString():"99999+"; 
            TextUI.text = gemText;
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        double gem = CurrencyManager.Instance.Gem;
        string gemText = gem <= 99999? gem.ToString():"99999+"; 
        TextUI.text = gemText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
