using System.Collections.Generic;
using UnityEngine;
 
 
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
 
    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            TriggerDialogue();
            GetComponent<Collider2D>().enabled = false;
        }
    }
    public void Update(){
        if(Input.GetKeyDown(KeyCode.Space) && DialogueManager.Instance.isDialogueActive){
            DialogueManager.Instance.DisplayNextDialogueLine();
        }
    }
}


 
[System.Serializable]
public class DialogueLine
{
    public Character character;
    [TextArea(3, 10)]
    public string line;
    public string animation;
    public bool RightSide;
}
 
[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}