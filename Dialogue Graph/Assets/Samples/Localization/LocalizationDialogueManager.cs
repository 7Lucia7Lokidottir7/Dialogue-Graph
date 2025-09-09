using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace PG.DialogueGraph
{
    using LocalizationSystem;
    public class LocalizationDialogueManager : DialogueManager
    {
        protected override void ShowNode(string nodeID)
        {
            if (!_nodeLookup.ContainsKey(nodeID))
            {
                return;
            }
            currentDialogueNode = _nodeLookup[nodeID];


            InvokeDialogueChanged(_nodeLookup[currentDialogueNode.nodeID]);

            if (currentDialogueNode != null)
            {
                _speakerNameObject.SetText(LocalizationSystem.instance.GetLocalizedValue(currentDialogueNode.speakerName));
                _dialogueTextObject.SetText(LocalizationSystem.instance.GetLocalizedValue(currentDialogueNode.dialogueText));
                _dialoguePanel.SetActive(true);


                foreach (Transform child in _choiceButtonContainer)
                {
                    Destroy(child.gameObject);
                }

                if (currentDialogueNode.choices.Count > 0)
                {
                    foreach (var choice in currentDialogueNode.choices)
                    {
                        Button button = Instantiate(_choiceButtonPrefab, _choiceButtonContainer);

                        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                        if (buttonText != null)
                        {
                            buttonText.text = choice.choiceText;
                        }

                        if (button != null)
                        {
                            button.onClick.AddListener(() =>
                            {
                                if (!string.IsNullOrEmpty(choice.desinationNodeID))
                                {
                                    ShowNode(choice.desinationNodeID);
                                }
                                else
                                {
                                    EndDialogue();
                                }
                            });
                        }
                    }
                }

            }
        }

    }
}
