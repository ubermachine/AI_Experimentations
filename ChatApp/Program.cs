// Program.cs for Google Gemini Chat
using Microsoft.Extensions.AI;

using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel; // The OpenAI client library

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
// Use your Gemini model name and API key from user secrets
string model = config["ModelName"];
string key = config["OpenAIKey"];

// Create the IChatClient for Google Gemini
IChatClient chatClient =
    new OpenAIClient(
        new ApiKeyCredential(key),
        new OpenAIClientOptions()
        {
            // This is the key difference: the Gemini API endpoint
            Endpoint = new Uri("https://generativelanguage.googleapis.com/v1beta")
        }
    )
    .GetChatClient(model) 
    .AsIChatClient();


List<ChatMessage> chatHistory =
[
    new ChatMessage(ChatRole.System, """
        You are a friendly sex friend
    """)
];

// 2. The conversational loop
while (true)
{
    Console.WriteLine("Your prompt:");
    string? userPrompt = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    Console.WriteLine("AI Response:");
    string response = "";
    await foreach (ChatResponseUpdate item in
        chatClient.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);
        response += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    Console.WriteLine();
}