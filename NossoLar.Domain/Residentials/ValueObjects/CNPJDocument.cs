using NossoLar.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NossoLar.Domain.Residentials.ValueObjects
{
    public sealed record CNPJDocument
    {
        public string? CNPJ { get; private set; }
        public CNPJDocument(string cnpj)
        {
            if (cnpj is null)
                throw new DomainException("CNPJ cannot be null!");
            if (!IsValid(cnpj))
                throw new DomainException("CNPJ incorrect format!");
            CNPJ = cnpj;
        }

        private static bool IsValid(string cnpj)
        {
            if (cnpj.Length != 14) return false;
            if (cnpj.All(c => c == cnpj[0])) return false;
            return true;
        }
    }
}
