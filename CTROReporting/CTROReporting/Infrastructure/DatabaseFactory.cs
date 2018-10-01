using CTROReporting.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROReporting.Infrastructure
{
    public interface IDatabaseFactory : IDisposable
    {
        EntitiesInitial Get();
    }

    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private EntitiesInitial dataContext;
        public EntitiesInitial Get()
        {
            //if dataContext is null then dataContext = new EntitiesInitial()
            return dataContext ?? (dataContext = new EntitiesInitial());
        }

        protected override void DisposeCore()
        {
            if (dataContext != null)
                dataContext.Dispose();
        }

    }
}