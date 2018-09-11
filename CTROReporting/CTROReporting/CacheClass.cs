using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
//using System.Runtime.Caching;

namespace CTRPReporting
{
    public class CacheClass : Controller
    {
        #region 页面缓存
        [OutputCache(Duration = 100)]
        public ActionResult A()
        {
            return View("~/Views/Cache/Index.cshtml");
        }

        [OutputCache(Duration = 100, VaryByParam = "id")]
        public ActionResult B(string id)
        {
            return View("~/Views/Cache/Index.cshtml");
        }

        [OutputCache(Duration = 100, VaryByParam = "id;name")]
        public ActionResult C(string id, string name)
        {
            return View("~/Views/Cache/Index.cshtml");
        }
        #endregion

        #region 二级缓存
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private object GetCache(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        /// <summary>
        /// 绝对过期
        /// </summary>
        /// <returns></returns>
        public ActionResult CacheAbsolute(string key)
        {
            object obj = GetCache(key);
            if (obj == null)
            {
                object list = new { Name = "张三", Age = 22, Time = DateTime.Now };
                HttpRuntime.Cache.Add(key, list, null, DateTime.Now.AddSeconds(10), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                obj = list;
            }
            ViewBag.CacheText = obj;
            return View("~/Views/Cache/Index.cshtml");
        }

        /// <summary>
        /// 相对过期
        /// </summary>
        /// <returns></returns>
        public ActionResult CacheRelative(string key)
        {
            object obj = GetCache(key);
            if (obj == null)
            {
                object list = new { Name = "张三", Age = 22, Time = DateTime.Now };
                HttpRuntime.Cache.Add(key, list, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, 10), CacheItemPriority.Default, null);
                obj = list;
            }
            ViewBag.CacheText = obj;
            return View("~/Views/Cache/Index.cshtml");
        }

        /// <summary>
        /// 数据库依赖
        /// </summary>
        /// <returns></returns>
        public ActionResult CacheSQL(string key)
        {
            object obj = GetCache(key);
            if (obj == null)
            {
                object list = new { Name = "张三", Age = 22, Time = DateTime.Now };
                HttpRuntime.Cache.Add(key, list, new SqlCacheDependency("CTROReportingEntities", "AspNetUsers"), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                obj = list;
            }
            ViewBag.CacheText = obj;
            return View("~/Views/Cache/Index.cshtml");
        }

        /// <summary>
        /// 文件依赖
        /// </summary>
        /// <returns></returns>
        public ActionResult CacheFile(string key)
        {
            object obj = GetCache(key);
            if (obj == null)
            {
                object list = new { Name = "张三", Age = 22, Time = DateTime.Now };
                HttpRuntime.Cache.Add(key, list, new CacheDependency(Request.MapPath("/App_Data/Config.xml")), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                obj = list;
            }
            ViewBag.CacheText = obj;
            return View("~/Views/Cache/Index.cshtml");
        }
        #endregion
    }


    //public class InMemoryCache : ICacheService
    //{
    //    public T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class
    //    {
    //        T item = MemoryCache.Default.Get(cacheKey) as T;
    //        if (item == null)
    //        {
    //            item = getItemCallback();
    //            MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(10));
    //        }
    //        return item;
    //    }
    //}

    //interface ICacheService
    //{
    //    T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class;
    //}
}