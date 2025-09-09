using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.Collections.Generic;
using System.Linq;

namespace PG.DialogueGraphEditor
{
    using PG.DialogueGraph;

    [ScriptedImporter(1, DialogueGraph.ASSET_EXTENSION)]
    public class DialogueGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            DialogueGraph editorGraph = GraphDatabase.LoadGraphForImporter<DialogueGraph>(ctx.assetPath);

            RuntimeDialogueGraph runtimeGraph = ScriptableObject.CreateInstance<RuntimeDialogueGraph>();

            var nodeIDMap = new Dictionary<INode, string>();
            foreach (var node in editorGraph.GetNodes())
            {
                nodeIDMap[node] = GUID.Generate().ToString();
            }

            var startNode = editorGraph.GetNodes().OfType<StartNode>().FirstOrDefault();
            if (startNode != null)
            {
                var entryPort = startNode.GetOutputPorts().FirstOrDefault()?.firstConnectedPort;
                if (entryPort != null)
                {
                    runtimeGraph.entryNodeID = nodeIDMap[entryPort.GetNode()];
                }
            }

            foreach (var iNode in editorGraph.GetNodes())
            {
                if (iNode is StartNode || iNode is EndNode) continue;

                var runtimeNode = new RuntimeDialogueNode { nodeID = nodeIDMap[iNode] };
                if (iNode is DialogueNode dialogueNode)
                {
                    ProcessDialogueNode(dialogueNode, runtimeNode, nodeIDMap);
                }
                else if (iNode is ChoiceNode choiceNode)
                {
                    ProcessChoiceNode(choiceNode, runtimeNode, nodeIDMap);
                }


                runtimeGraph.allNodes.Add(runtimeNode);
            }
            ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
        }
        private void ProcessDialogueNode(DialogueNode node, RuntimeDialogueNode runtimeNode, Dictionary<INode, string> nodeIDMap)
        {
            node.GetNodeOptionByName("Node Key").TryGetValue(out runtimeNode.nodeKey);
            runtimeNode.speakerName = GetPortValue<string>(node.GetInputPortByName("Speaker"));
            runtimeNode.dialogueText = GetPortValue<string>(node.GetInputPortByName("Dialogue"));

            var nextNodePort = node.GetOutputPortByName("out").firstConnectedPort;
            if (nextNodePort != null)
            {
                runtimeNode.nextNodeID = nodeIDMap[nextNodePort.GetNode()];
            }
        }
        private void ProcessChoiceNode(ChoiceNode node, RuntimeDialogueNode runtimeNode, Dictionary<INode, string> nodeIDMap)
        {
            node.GetNodeOptionByName("Node Key").TryGetValue(out runtimeNode.nodeKey);
            runtimeNode.speakerName = GetPortValue<string>(node.GetInputPortByName("Speaker"));
            runtimeNode.dialogueText = GetPortValue<string>(node.GetInputPortByName("Dialogue"));

            var choiceOutputPorts = node.GetOutputPorts().Where(p => p.name.StartsWith("Choice "));

            foreach (var outputPort in choiceOutputPorts)
            {
                var index = outputPort.name.Substring("Choice ".Length);
                var textPort = node.GetInputPortByName($"Choice Text {index}");

                var choiceData = new ChoiceData
                {
                    choiceText = GetPortValue<string>(textPort),
                    desinationNodeID = outputPort.firstConnectedPort != null
                        ? nodeIDMap[outputPort.firstConnectedPort.GetNode()]
                        : null
                };

                runtimeNode.choices.Add(choiceData);
            }

        }

        private T GetPortValue<T>(IPort port)
        {
            if (port == null) return default;

            if (port.isConnected)
            {
                if (port.firstConnectedPort.GetNode() is IVariableNode variableNode)
                {
                    variableNode.variable.TryGetDefaultValue(out T value);
                    return value;
                }
            }

            port.TryGetValue(out T fallbackValue);
            return fallbackValue;
        }


    }
}