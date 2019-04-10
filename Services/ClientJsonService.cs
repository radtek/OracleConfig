using OracleConfig.Entities.Exceptions;

namespace OracleConfig.Services
{
    class ClientJsonService{
        private IClientJsonService _commandCli;

        public ClientJsonService(IClientJsonService client)
        {
            if (client == null)
            {
                throw new DomainException("Client name undefined.");
            }

            _commandCli = client;
        }

        public void CreateConfig()
        {
            _commandCli.CreateConfig();
        }
    }
}