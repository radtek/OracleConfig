using System;

namespace OracleConfig.Entities.Exceptions
{
    class DomainException : ApplicationException
    {
        public DomainException(string menssage) : base(menssage){}
    }
}