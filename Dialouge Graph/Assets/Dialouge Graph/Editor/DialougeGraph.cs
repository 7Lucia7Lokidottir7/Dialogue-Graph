using Unity.GraphToolkit.Editor;
using UnityEditor;
using System;

namespace PG.DialougeGraphEditor
{
    [Serializable]
    [Graph(ASSET_EXTENSION)]
    public class DialougeGraph : Graph
    {
        public const string ASSET_EXTENSION = "dialougegraph";
        [MenuItem("Assets/Create/PG/Dialouge Graph")]
        private static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialougeGraph>();
        }
    }
}
