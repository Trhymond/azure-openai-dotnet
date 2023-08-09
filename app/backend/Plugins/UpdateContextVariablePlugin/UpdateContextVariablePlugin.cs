
namespace Rhymond.OpenAI.Plugins;

public sealed class UpdateContextVariablePlugin
{
    [SKFunction, Description("Update or Append knowledge variable"), SKName("UpdateKnowledgeVariable")]
    public void UpdateKnowledgeVariable(string variableValue, SKContext context)
    {
        AddOrAppend("knowledge", variableValue, context);

        //if (context.Variables.ContainsKey("knowledge"))
        //{
        //    context.Variables["knowledge"] = $"{ context.Variables["knowledge"]}\r{variableValue}";
        //} else
        //{
        //    context.Variables["knowledge"] = variableValue;
        //}
    }

    private static void AddOrAppend(string variableName, string variableValue, SKContext context)
    {
        if (context.Variables.ContainsKey(variableName))
        {
            context.Variables[variableName] = $"{context.Variables[variableName]}\r{variableValue}";
        }
        else
        {
            context.Variables[variableName] = variableValue;
        }
    }
}


