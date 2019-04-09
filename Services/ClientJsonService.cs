using OracleConfig.Entities.Exceptions;

namespace OracleConfig.Services
{
    class ClientJsonService{
        private string _client;

        public ClientJsonService(string client)
        {
            if (string.IsNullOrWhiteSpace(client))
            {
                throw new DomainException("Client name undefined.");
            }

            _client = client;
        }

        public IClientJsonService CreateClientJsonService()
        {
            return new ClientOracleService(_client);
        }
    }
}