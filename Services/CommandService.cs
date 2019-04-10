using OracleConfig.Entities.Exceptions;

namespace OracleConfig.Services
{
    class ClientJsonService{
        private ICommandService _commands;

        public ClientJsonService(ICommandService command)
        {
            if (command == null)
            {
                throw new DomainException("Command type undefined.");
            }

            _commands = command;
        }

        public void CreateConfig()
        {
            _commands.CreateConfig();
        }
    }
}