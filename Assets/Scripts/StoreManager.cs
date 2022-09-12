using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.Events;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance {get; private set;}

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance =  this;
            DontDestroyOnLoad(Instance);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBuyComplete(Product product) {
        var payout = product.definition.payout;
        if(payout.type == PayoutType.Currency) {
            CurrencyManager.Instance.updateCurrency(payout.quantity, payout.subtype);
        }
    }
}
