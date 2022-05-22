using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Component
{
    public class UserTokenComponent : IComponent, IComponentOwner
    {
        public IEntity mOwner { get; set; }
    }
}
