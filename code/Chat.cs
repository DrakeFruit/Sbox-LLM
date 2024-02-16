using System;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

[Title( "Chat" )]
[Category( "LLM" )]
[Icon( "electrical_services" )]
public sealed class Chat : Component
{
    public AIAssistant assistant = new AIAssistant();
    public String completion;
    public async Task SendInput( string prompt )
    {
        Log.Info("sending prompt: " + prompt);
        completion = await assistant.Complete( prompt );
        Log.Info( completion );
    }
}

public class AIAssistant
{
    // This is the history of prompts and completions. This is used to give the AI context for the current prompt.
    public List<string> history;
    public string system_prompt = "You are a very powerful AI assistant, your responses are short and sweet.";
    private string Url = "https://iiced-mixtral-46-7b-fastapi.hf.space/generate/";

    // Constructor
    public AIAssistant()
    {
        history = new List<string>();
    }

    public async Task<string> Complete( string prompt )
    {
        // Create HTTP Content. This is the json object that is sent in the body of the request.
        var content = new StringContent( JsonSerializer.Serialize( new
        {
            prompt,
            history,
            system_prompt
        } ), Encoding.UTF8, "application/json" );
        try
        {
            // Make the request to the API
            var response = await Http.RequestAsync(Url, "POST", content);
            // TODO : Add both the prompt and the completion to the history and limit the history to X amount of entries. This will allow the following prompt to have context.

            // Return the response from the API. You probably want to parse this and only return the completion, as this is a json string.
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Log.Error(e);
            return "";
        }
    }
}