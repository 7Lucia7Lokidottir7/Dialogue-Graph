using Unity.GraphToolkit.Editor;
using UnityEditor;
using System;

namespace PG.DialogueGraphEditor
{
    [Serializable]
    [Graph(ASSET_EXTENSION)]
    public class DialogueGraph : Graph
    {
        public const string ASSET_EXTENSION = "Dialoguegraph";
        [MenuItem("Assets/Create/PG/Dialogue Graph")]
        private static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogueGraph>();
        }
    }
}
