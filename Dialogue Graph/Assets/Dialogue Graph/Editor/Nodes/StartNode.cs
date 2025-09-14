using System;
using Unity.GraphToolkit.Editor;
namespace PG.DialogueGraphEditor
{
    [Serializable]
    public class StartNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddOutputPort("out").WithConnectorUI(PortConnectorUI.Arrowhead).Build();
        }
    }
}