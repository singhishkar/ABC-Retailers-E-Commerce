using Azure.Data.Tables;
using Azure;
using ABCRetail.Models;
using Org.BouncyCastle.Crypto.Generators;

namespace ABCRetail.Services
{

    public class UserService
    {
        private readonly TableClient _userTableClient;

        public UserService(string connectionString)
        {
            _userTableClient = new TableClient(connectionString, "Users");
            _userTableClient.CreateIfNotExists();
        }

        public async Task<List<User>> GetAllCustomersAsync()
        {
            var customers = new List<User>();
            await foreach (var customer in _userTableClient.QueryAsync<User>())
            {
                customers.Add(customer);
            }
            return customers;
        }

        public async Task<User?> GetCustomerAsync(string rowKey)
        {
            var query = _userTableClient.QueryAsync<User>(filter: $"RowKey eq '{rowKey}'");
            await foreach (var customer in query)
            {
                return customer;
            }
            return null;
        }

        public async Task<User?> FindCustomerByEmailAsync(string email)
        {
            var query = _userTableClient.QueryAsync<User>(filter: $"PartitionKey eq '{email}'");
            await foreach (var customer in query)
            {
                return customer;
            }
            return null;
        }

        public async Task<bool> AddCustomerAsync(User customer)
        {
            if (await FindCustomerByEmailAsync(customer.CustEmail) != null)
            {
                return false; // Customer already exists
            }

            customer.PartitionKey = customer.CustEmail;
            customer.RowKey = Guid.NewGuid().ToString();

            try
            {
                await _userTableClient.AddEntityAsync(customer);
                return true;
            }
            catch (RequestFailedException ex)
            {
                // Log exception or handle it as needed
                throw new InvalidOperationException("Error adding customer to Table Storage", ex);
            }
        }

        public async Task UpdateCustomerAsync(User customer)
        {
            try
            {
                await _userTableClient.UpdateEntityAsync(customer, ETag.All, TableUpdateMode.Replace);
            }
            catch (RequestFailedException ex)
            {
                // Log exception or handle it as needed
                throw new InvalidOperationException("Error updating customer in Table Storage", ex);
            }
        }

        public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
        {
            try
            {
                await _userTableClient.DeleteEntityAsync(partitionKey, rowKey);
            }
            catch (RequestFailedException ex)
            {
                // Log exception or handle it as needed
                throw new InvalidOperationException("Error deleting customer from Table Storage", ex);
            }
        }

        public async Task<bool> CustomerExistsAsync(string id)
        {
            var query = _userTableClient.QueryAsync<User>(filter: $"RowKey eq '{id}'");
            await foreach (var customer in query)
            {
                return true;
            }
            return false;
        }
    }
}