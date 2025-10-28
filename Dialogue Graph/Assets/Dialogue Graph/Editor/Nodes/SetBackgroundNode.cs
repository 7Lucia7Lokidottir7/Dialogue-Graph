using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;
namespace PG.DialogueGraphEditor
{
    [Serializable]
    public class SetBackgroundNode : BaseNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddOutputPort("out").WithConnectorUI(PortConnectorUI.Arrowhead).Build();
            context.AddInputPort("in").WithConnectorUI(PortConnectorUI.Arrowhead).Build();

            context.AddInputPort<Sprite>("Background").Build();
        }
        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>("Node Key").Delayed().Build();
            base.OnDefineOptions(context);
        }
    }
}