using System;
using Unity.GraphToolkit.Editor;
namespace PG.DialogueGraphEditor
{
    [Serializable]
    public class DialogueNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddOutputPort("out").WithConnectorUI(PortConnectorUI.Arrowhead).Build();
            context.AddInputPort("in").WithConnectorUI(PortConnectorUI.Arrowhead).Build();

            context.AddInputPort<string>("Speaker").Build();
            context.AddInputPort<string>("Dialogue").Build();
        }
        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>("Node Key").Delayed().Build();
        }
    }
}