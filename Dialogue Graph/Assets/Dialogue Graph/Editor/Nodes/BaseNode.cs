using System;
using Unity.GraphToolkit.Editor;
namespace PG.DialogueGraphEditor
{
    [Serializable]
    public class BaseNode : Node
    {
        public virtual string GetJsonData()
        {
            return "";
        }
    }
}