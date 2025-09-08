using System;
using Unity.GraphToolkit.Editor;
namespace PG.DialougeGraphEditor
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

    [Serializable]
    public class EndNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddInputPort("in").WithConnectorUI(PortConnectorUI.Arrowhead).Build();
        }
    }

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
    }

    [Serializable]
    public class ChoiceNode : Node
    {
        private const string _OPTION_ID = "portCount";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddInputPort("in").WithConnectorUI(PortConnectorUI.Arrowhead).Build();

            context.AddInputPort<string>("Speaker").Build();
            context.AddInputPort<string>("Dialogue").Build();

            var option = GetNodeOptionByName(_OPTION_ID);
            option.TryGetValue(out int portCount);

            for (int i = 0; i < portCount; i++)
            {
                context.AddInputPort<string>($"Choice Text {i}").Build();
                context.AddOutputPort($"Choice {i}").WithConnectorUI(PortConnectorUI.Arrowhead).Build();
            }
        }
        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<int>(_OPTION_ID).WithDefaultValue(2).Delayed().Build();
        }
    }
}