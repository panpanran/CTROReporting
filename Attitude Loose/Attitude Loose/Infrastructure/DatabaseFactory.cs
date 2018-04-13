using Attitude_Loose.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Infrastructure
{
    public interface IDatabaseFactory
    {
        EntitiesInitial Get();
    }

    public class DatabaseFactory : IDatabaseFactory
    {
        private EntitiesInitial dataContext;
        public EntitiesInitial Get()
        {
            //if dataContext is null then dataContext = new EntitiesInitial()
            return dataContext ?? (dataContext = new EntitiesInitial());
        }
    }
}