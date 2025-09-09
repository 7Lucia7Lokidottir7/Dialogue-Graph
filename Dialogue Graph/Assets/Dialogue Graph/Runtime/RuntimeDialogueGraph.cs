using System;
using System.Collections.Generic;
using UnityEngine;

namespace PG.DialogueGraph
{
    public class RuntimeDialogueGraph : ScriptableObject
    {
        public string entryNodeID;
        public List<RuntimeDialogueNode> allNodes = new List<RuntimeDialogueNode>();
    }

    [Serializable]
    public class RuntimeDialogueNode
    {
        public string nodeID;
        public string nodeKey;
        public string speakerName;
        public string dialogueText;
        public List<ChoiceData> choices = new List<ChoiceData>();
        public string nextNodeID;
    }

    [Serializable]
    public class ChoiceData
    {
        public string choiceText;
        public string desinationNodeID;
    }
}