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
    public bool isDialougeFinished = false;
 
    public float typingSpeed;
 
    private Animator animator;
 
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
 
       animator.Play("Show");
 
        lines.Clear();
 
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }
 
        DisplayNextDialogueLine();
    }
 
    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }
 
        DialogueLine currentLine = lines.Dequeue();
        isDialougeFinished = false;
        characterIcon.GetComponent<Image>().sprite = currentLine.character.image;
        characterIcon.GetComponent<Animator>().runtimeAnimatorController = currentLine.character.animator;
        characterName.GetComponent<TextMeshProUGUI>().text = currentLine.character.CharacterName;
        characterIcon.GetComponent<Animator>().Play(currentLine.animation);
 
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
    }
 
    void EndDialogue()
    {
        isDialogueActive = false;
        animator.Play("Hide");
    }
}