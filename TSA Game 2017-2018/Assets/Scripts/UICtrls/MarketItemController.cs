using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketItemController : MonoBehaviour {

    public GameObject playerObj;
    public string itemName; //This item's name; Displayed at the top of the market ui when this is selected
    [TextArea]
    public string itemDescription; //This item's description; This will display on the left side of the market ui when this is selected
    public int itemCost; //How much is costs to buy this item; This is displayed at the top right of the market ui when this is selected

    public int itemEffect; //0 = custom, 1 = max health boost, 2 = max block boost, 3 = damage boost
    public int itemIntensity; //How much of the effect above to apply

    public void OnPurchase() //What to do when this item is bought
    {
        switch(itemEffect)
        {
            case 1:
                playerObj.GetComponent<PlayerController>().maxHealth += itemIntensity;
                playerObj.GetComponent<PlayerController>().gameControllerScript.UpdateHealthSlider(playerObj.GetComponent<PlayerController>().minHealth, playerObj.GetComponent<PlayerController>().maxHealth, playerObj.GetComponent<PlayerController>().health);
                break;
            case 2:
                playerObj.GetComponent<PlayerController>().maxBlock += itemIntensity;
                playerObj.GetComponent<PlayerController>().gameControllerScript.UpdateHealthSlider(playerObj.GetComponent<PlayerController>().minBlock, playerObj.GetComponent<PlayerController>().maxBlock, (int) playerObj.GetComponent<PlayerController>().blockMeter);
                break;
            case 3:
                playerObj.GetComponent<PlayerController>().standardPunchDamage += itemIntensity;
                break;
        }
    }
}
