using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace ABCRetail.Services
{
    public class QueueService
    {
        private readonly QueueClient _queueClient;

        public QueueService(string connectionString, string queueName)
        {
            var queueServiceClient = new QueueServiceClient(connectionString);
            _queueClient = queueServiceClient.GetQueueClient(queueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessageAsync(string message)
        {
            await _queueClient.SendMessageAsync(message);
        }


        public async Task<List<string>> GetMessagesAsync(int maxMessages = 10)
        {
            var messages = new List<string>();
            var retrievedMessages = await _queueClient.ReceiveMessagesAsync(maxMessages);

            foreach (QueueMessage message in retrievedMessages.Value)
            {
                messages.Add(message.MessageText);
            }

            return messages;
        }
    }
}