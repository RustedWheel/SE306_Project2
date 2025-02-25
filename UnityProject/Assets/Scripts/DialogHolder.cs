﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class DialogHolder : MonoBehaviour
{
    public List<GameObject> dBoxes;
    public int boxIndex = 0;
   
    public string dialogue;
    private DialogueManager dMan;

    public string[] textLines;
    // public Item item to check
    public bool autoDialog;
    private bool showDecisionBox;

    //needs to link to inventory 

    // Use this for initialization
    void Start()
    {
        dMan = FindObjectOfType<DialogueManager>();

        textLines = dBoxes[boxIndex].GetComponent<TextHolder>().getTextLines();
    }

    // Update is called once per frame
    void Update()
    {
        if (dMan != null && this != null && dMan.activeNPC == this.gameObject) // only update dialogue if the current active NPC holds this dialogue
        {
            
            // check if should move on to next dialogue box
            if (dMan.currentLine >= textLines.Length)
            {    // end of this box's file asset
                if (boxIndex < dBoxes.Count - 1)
                {
                    boxIndex++;

                    setAndShowDialogue(dBoxes[boxIndex]);
                }

            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
      
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("exit");
        if (other.gameObject.CompareTag("Player"))
        {
            dMan.setActiveNPC(null);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {



            dMan.setActiveNPC(this.gameObject);
            if (autoDialog)
            {
                //dMan.showBox(this.gameObject.name , dialogue);
                // show dialogue
                if (!dMan.diaglogActive)
                {
                    setAndShowDialogue(dBoxes[boxIndex]);
                    //dMan.currentLine = 0;
                }
                autoDialog = false;
            }
        }
    }

    public void setAndShowDialogue(GameObject box)
    {
        if (dMan.diaglogActive)
        {
            dMan.closeDialogue();
        }

        textLines = box.GetComponent<TextHolder>().getTextLines();

        dMan.dBox = box;

        dMan.dialogLines = textLines;

        dMan.currentLine = 0;

        dMan.showDialogue(this.gameObject.name);

        if (box.tag.Equals("FinalStatement") || (box.name.Equals("DecisionCorrectAnswer"))) {
            dMan.setAllDone();
        }

    }




}
