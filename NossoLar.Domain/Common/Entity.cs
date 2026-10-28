using System;
using System.Collections.Generic;
using System.Text;

namespace NossoLar.Domain.Common
{
    public class Entity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
    }
}
