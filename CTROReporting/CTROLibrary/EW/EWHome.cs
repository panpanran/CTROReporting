using System.Collections.Generic;

namespace CTROLibrary.EW
{
    public interface IEWHome<T> where T : class
    {
        void Update(T tablename);
        T GetById(string id);
        IEnumerable<string> GetIDList(string where);
    }
}
