using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
namespace PG.DialogueGraph
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] protected RuntimeDialogueGraph _graph;
        [SerializeField] protected InputActionProperty _nextDialogueProperty;
        protected InputAction _nextDialogueAction;

        [SerializeField] protected bool _startDialogueOnStart;

        [Header("UI")]
        [SerializeField] protected GameObject _dialoguePanel;
        [SerializeField] protected TMP_Text _speakerNameObject;
        [SerializeField] protected TMP_Text _dialogueTextObject;

        [Header("Choice Button UI")]
        [SerializeField] protected Button _choiceButtonPrefab;
        [SerializeField] protected Transform _choiceButtonContainer;

        protected Dictionary<string, RuntimeDialogueNode> _nodeLookup = new Dictionary<string, RuntimeDialogueNode>();
        public RuntimeDialogueNode currentDialogueNode { get; protected set; }

        public event System.Action<RuntimeDialogueGraph> dialogueStarted;
        public event System.Action<RuntimeDialogueNode> dialogueChanged;
        public event System.Action<RuntimeDialogueNode> previousDialogueChanged;
        public event System.Action dialogueEnded;

        #region Invoke event Actions
        protected void InvokePreviousDialogueChanged(RuntimeDialogueNode runtimeDialogueNode) => previousDialogueChanged?.Invoke(runtimeDialogueNode);
        protected void InvokeDialogueChanged(RuntimeDialogueNode runtimeDialogueNode) => dialogueChanged?.Invoke(runtimeDialogueNode);
        protected void InvokeDialogueStarted(RuntimeDialogueGraph runtimeDialogueGraph) => dialogueStarted?.Invoke(runtimeDialogueGraph);
        protected void InvokeDialogueEnded() => dialogueEnded?.Invoke(); 
        #endregion



        private void Awake()
        {
            _nextDialogueAction = InputSystem.actions.FindAction(_nextDialogueProperty.reference.name);
        }
        private void Start()
        {
            if (_startDialogueOnStart)
            {
                StartDialogue(_graph);
            }
        }
        protected virtual void ShowNode(string nodeID)
        {
            if (!_nodeLookup.ContainsKey(nodeID))
            {
                return;
            }
            currentDialogueNode = _nodeLookup[nodeID];


            dialogueChanged?.Invoke(_nodeLookup[currentDialogueNode.nodeID]);

            if (currentDialogueNode != null)
            {
                _speakerNameObject.SetText(currentDialogueNode.speakerName);
                _dialogueTextObject.SetText(currentDialogueNode.dialogueText);
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

        protected void EndDialogue()
        {
            _speakerNameObject.SetText("");
            _dialogueTextObject.SetText("");

            _dialoguePanel.SetActive(false);
            foreach (Transform item in _choiceButtonContainer)
            {
                Destroy(item.gameObject);
            }


            currentDialogueNode = null;
            dialogueEnded?.Invoke();

            _nextDialogueAction.performed -= NextDialogue;
        }
        public void StartDialogue() => StartDialogue(_graph);
        public void StartDialogue(RuntimeDialogueGraph runtimeDialogueGraph)
        {
            if (currentDialogueNode != null)
            {
                EndDialogue();
            }

            _graph = runtimeDialogueGraph;
            dialogueStarted?.Invoke(_graph);
            foreach (var node in _graph.allNodes)
            {
                _nodeLookup[node.nodeID] = node;
            }

            if (!string.IsNullOrEmpty(_graph.entryNodeID))
            {
                ShowNode(_graph.entryNodeID);
            }
            else
            {
                EndDialogue();
            }

            _nextDialogueAction.performed += NextDialogue;
        }
        // Update is called once per frame
        void NextDialogue(InputAction.CallbackContext context)
        {
            if (currentDialogueNode != null && currentDialogueNode.choices.Count == 0)
            {
                if (!string.IsNullOrEmpty(currentDialogueNode.nextNodeID))
                {
                    InvokePreviousDialogueChanged(currentDialogueNode);
                    ShowNode(currentDialogueNode.nextNodeID);
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }
}
