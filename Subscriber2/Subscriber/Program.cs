
using Subscriber.DTOs;
using System.Net.Http.Json;

Console.WriteLine("Press ESC to stop");
do
{
    HttpClient client = new HttpClient();
    Console.WriteLine("Listening...");
    while (!Console.KeyAvailable)
    {
        List<int> ackIds = await GetMessagesAsync(client);

        Thread.Sleep(2000);

        if (ackIds.Count > 0)
        {
            await AckMessageAsync(client, ackIds);
        }
    }

} while (Console.ReadKey(true).Key != ConsoleKey.Escape);

static async Task<List<int>> GetMessagesAsync(HttpClient httpClinet)
{
    List<int> ackIds = new List<int>();
    List<MessageReadDto>? newMessages = new List<MessageReadDto>();

    try
    {
        newMessages = await httpClinet.GetFromJsonAsync<List<MessageReadDto>>("http://localhost:5196/api/subscriptions/6/messages");

    }
    catch
    {
        return ackIds;
    }

    foreach (MessageReadDto msg in newMessages!)
    {
        Console.WriteLine($"{msg.Id} - {msg.TopicMessage} - {msg.MessageStatus}");
        ackIds.Add(msg.Id);
    }

    return ackIds;

}

static async Task AckMessageAsync(HttpClient httpClinet ,List<int> ackIds)
{
    var response = await httpClinet.PostAsJsonAsync("http://localhost:5196/api/subscriptions/6/messages", ackIds);
    var returnMessage = await response.Content.ReadAsStringAsync();

    Console.WriteLine(returnMessage);
}