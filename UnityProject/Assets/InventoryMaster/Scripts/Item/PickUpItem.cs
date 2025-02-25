﻿using UnityEngine;
public class PickUpItem : MonoBehaviour
{
    public Item item;
    public AudioClip pickUpFX;

    private Inventory inventory;
    private GameObject player1;
    private GameObject player2;
    private GameController.PlayableScene ThisScene;

    // Use this for initialization
    void Start()
    {
        ThisScene = GameController.getActiveScene();
        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (Obj.name == "Astronaut")
                player1 = Obj;
            else if (Obj.name == "Astronaut_2")
                player2 = Obj;
        }
        if (player1 != null)
            inventory = player1.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory != null)
        {
            float distance1 = Vector3.Distance(this.gameObject.transform.position, player1.transform.position);
            float distance2 = Vector3.Distance(this.gameObject.transform.position, player2.transform.position);

            if (distance1 <= 2 || distance2 <= 2)
            {
                bool check = inventory.checkIfItemAllreadyExist(item.itemID, item.itemValue);
                if (check)
                    Destroy(this.gameObject);
                else if (inventory.ItemsInInventory.Count < (inventory.width * inventory.height))
                {
                    inventory.addItemToInventory(item.itemID, item.itemValue);
                    ThisScene.AddItemToPersistedInventory(item.itemID);
                    Destroy(this.gameObject);
                    if (pickUpFX != null)
                        AudioSource.PlayClipAtPoint(pickUpFX, transform.position);
                }
            }
        }
    }

}