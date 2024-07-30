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
    [Property] public String system_prompt { get; set; } = "You are a helpful AI assistant.";

    public AIAssistant assistant = new AIAssistant();
    public String completion;
    public async Task SendInput( string prompt, string system_prompt )
    {
        Log.Info("sending prompt: " + prompt);
        completion = await assistant.Complete( prompt, system_prompt );
        assistant.history.Add(completion);
    }
}

public class AIAssistant
{
    // This is the history of prompts and completions. This is used to give the AI context for the current prompt.
    public List<string> history;
    public string system_prompt;
    private string Url = "https://drakefruit-mixtral-46-7b-fastapi.hf.space/generate/";

    // Constructor
    public AIAssistant()
    {
        history = new List<string>();
    }

    public async Task<string> Complete( string prompt, string system_prompt )
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