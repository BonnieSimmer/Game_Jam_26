using System.Collections;
using UnityEngine;

public class BedLogic : InteractableLogic
{
    private GameManager gameManager;
    private DayCycle dayCycle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        dayCycle = GameObject.Find("Directional Light").GetComponent<DayCycle>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(gameManager == null)
        {
            Debug.LogError("GameManager not found in BedLogic");
        }
        if(dayCycle == null)
        {
            Debug.LogError("DayCycle not found in BedLogic, ensure gameObject named (Directional Light) exists with DayCycle script");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        base.Interact();
        if (dayCycle.GetDayStatus() ||dayCycle.GetDayProgress()>=gameManager.bedTimeThreshold)
        {
            gameManager.GoingToSleep();
        }
    }

    
}

