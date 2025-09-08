using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
namespace PG.DialougeGraph
{
    public class DialougeManager : MonoBehaviour
    {
        [SerializeField] private RuntimeDialougeGraph _graph;
        [SerializeField] private InputActionProperty _nextDialogueProperty;
        private InputAction _nextDialogueAction;

        [SerializeField] private bool _startDialogueOnStart;

        [Header("UI")]
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private TMP_Text _speakerNameObject;
        [SerializeField] private TMP_Text _dialogueTextObject;

        [Header("Choice Button UI")]
        [SerializeField] private Button _choiceButtonPrefab;
        [SerializeField] private Transform _choiceButtonContainer;

        private Dictionary<string, RuntimeDialogueNode> _nodeLookup;
        private RuntimeDialogueNode _currentDialogueNode;

        public event System.Action<RuntimeDialougeGraph> dialogueStarted;
        public event System.Action<RuntimeDialogueNode> dialogueChanged;
        public event System.Action dialogueEnded;
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
        void ShowNode(string nodeID)
        {
            if (!_nodeLookup.ContainsKey(nodeID))
            {
                return;
            }
            _currentDialogueNode = _nodeLookup[nodeID];


            dialogueChanged?.Invoke(_nodeLookup[_currentDialogueNode.nodeID]);

            if (_currentDialogueNode != null)
            {
                _speakerNameObject.SetText(_currentDialogueNode.speakerName);
                _dialogueTextObject.SetText(_currentDialogueNode.dialogueText);
                _dialoguePanel.SetActive(true);


                foreach (Transform child in _choiceButtonContainer)
                {
                    Destroy(child.gameObject);
                }

                if (_currentDialogueNode.choices.Count > 0)
                {
                    foreach (var choice in _currentDialogueNode.choices)
                    {
                        Button button = Instantiate(_choiceButtonPrefab, _choiceButtonContainer);

                        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
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

        void EndDialogue()
        {
            _speakerNameObject.SetText("");
            _dialogueTextObject.SetText("");

            _dialoguePanel.SetActive(false);
            foreach (Transform item in _choiceButtonContainer)
            {
                Destroy(item.gameObject);
            }


            _currentDialogueNode = null;
            dialogueEnded?.Invoke();

            _nextDialogueAction.performed -= NextDialogue;
        }

        void StartDialogue(RuntimeDialougeGraph runtimeDialougeGraph)
        {
            if (_currentDialogueNode != null)
            {
                EndDialogue();
            }

            _graph = runtimeDialougeGraph;
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
            if (_currentDialogueNode != null && _currentDialogueNode.choices.Count == 0)
            {
                if (string.IsNullOrEmpty(_currentDialogueNode.nextNodeID))
                {
                    ShowNode(_currentDialogueNode.nextNodeID);
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }
}
