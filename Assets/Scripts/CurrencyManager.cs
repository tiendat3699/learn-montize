    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public double Gold {get; private set;} = 0;
    public double Gem {get; private set;} = 0;
    public UnityEvent<double, double> OnUpdateCurrency;

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

    public void updateCurrency(double quantity, string subtype) {
        if(subtype == "Gold") {
            Gold += quantity;
        }

        if(subtype == "Gem") {
            Gem += quantity;
        }
        OnUpdateCurrency?.Invoke(Gold, Gem);

    }
}
