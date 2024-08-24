using CommandTerminal;
using UnityEngine;
using System.Threading.Tasks;
using System.Text;

public static class EncodeCommand
{
    [RegisterCommand(Help = "Encodes a string using the Encoder", MinArgCount = 1)]
    public static async void CommandEncode(CommandArg[] args)
    {
        // Reassemble the arguments into a single string
        StringBuilder inputStringBuilder = new StringBuilder();
        for (int i = 0; i < args.Length; i++)
        {
            if (i > 0)
            {
                inputStringBuilder.Append(" ");
            }
            inputStringBuilder.Append(args[i].String);
        }
        string inputString = inputStringBuilder.ToString();

        // Add debug log to print the reassembled input string
        Debug.Log($"Reassembled Input String: {inputString}");

        if (Terminal.IssuedError) return;

        string result = await Encoder.Instance.Encode(inputString);
        if (!string.IsNullOrEmpty(result))
        {
            Debug.Log($"Encoded Result: {result}");
        }
        else
        {
            Debug.LogError("Failed to encode the input string.");
        }
    }
}
