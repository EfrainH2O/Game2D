using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
 
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
 
    public GameObject characterIcon;
    public GameObject characterName;
    public GameObject dialogueArea;
 
    private Queue<DialogueLine> lines;
    
    public bool isDialogueActive = false;
    public bool isDialougeFinished = true;
 
    public float typingSpeed;
 
    private Animator animator;
    private DialogueLine currentLine;
 
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
 
        lines = new Queue<DialogueLine>();
    }
    public void Start(){
        animator = GetComponent<Animator>();
    }
    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        Player.Instance.Freeze();
        animator.Play("Show");
        lines.Clear();
 
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }
        isDialougeFinished = true;
        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {

        if(!isDialougeFinished){
            StopAllCoroutines();
            dialogueArea.GetComponent<TextMeshProUGUI>().text = currentLine.line;
            isDialougeFinished = true;
            return;
        }
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }
        
 
        currentLine = lines.Dequeue();
        isDialougeFinished = false;
        characterIcon.GetComponent<Image>().sprite = currentLine.character.image;
        characterIcon.GetComponent<Animator>().runtimeAnimatorController = currentLine.character.animator;
        characterName.GetComponent<TextMeshProUGUI>().text = currentLine.character.CharacterName;
        characterIcon.GetComponent<Animator>().Play(currentLine.animation);
        if(currentLine.RightSide){
            characterIcon.transform.localPosition = new Vector2(548.7f, characterIcon.transform.localPosition.y);
            characterName.transform.localPosition = new Vector2(691f, characterName.transform.localPosition.y);
            dialogueArea.transform.localPosition  = new Vector2(186f, dialogueArea.transform.localPosition.y);
        }else{
            characterIcon.transform.localPosition = new Vector2(-643.6f, characterIcon.transform.localPosition.y);
            characterName.transform.localPosition = new Vector2(-160, characterName.transform.localPosition.y);
            dialogueArea.transform.localPosition  = new Vector2(406f, dialogueArea.transform.localPosition.y);
        }
 
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
        
    }
 
    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.GetComponent<TextMeshProUGUI>().text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.GetComponent<TextMeshProUGUI>().text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isDialougeFinished = true;
    }
 
    void EndDialogue()
    {
        isDialogueActive = false;
        animator.Play("Hide");
        Player.Instance.UnFreeze();
    }
}