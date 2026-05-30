using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;


// Safe way of retrieving the token from the config file.
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

// Assigning token
string myToken = configuration["BotToken"] ?? "";

// Checks to see if Null of empty token
if (string.IsNullOrEmpty(myToken))
{
    Console.WriteLine("Error: BotToken is missing from appsettings.json!");
    return;
}

// Gateway intents to give bot basic events, a setting in developer portal needs to be enabled to use Message contents, otherwise it would return an empty string.
var config = new DiscordSocketConfig 
{ 
    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
};

// Initialize the Discord client object, passing in a custom Gateway configuration.
var client = new DiscordSocketClient(config);

// Every connection step, warning, or API error will now be forwarded to 'LogAsync' method.
client.Log += LogAsync;

// This fires a lambda function asynchronously whenever a message is sent in a channel.
client.MessageReceived += async (message) =>
{
    if (message.Author.IsBot) return;
    // Typing !ping triggers this method
    if (message.Content.Equals("!ping", StringComparison.OrdinalIgnoreCase))
    {
        await message.Channel.SendMessageAsync("Pong!");
    }
};

// Authenticate the client with Discord's servers using the bot token.
await client.LoginAsync(TokenType.Bot, myToken);

// Establish a live WebSocket connection (the Gateway) to start receiving events.
await client.StartAsync();

// Block the main thread from exiting. Because this is a console app, it would close instantly without this.
// Passing '-1' tells the program to wait indefinitely, keeping the bot online forever.
await Task.Delay(-1);

// Logging method
Task LogAsync(LogMessage log)
{
    Console.WriteLine(log.ToString());
    return Task.CompletedTask;
}