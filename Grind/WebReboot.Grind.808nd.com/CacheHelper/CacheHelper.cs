using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Interfaces;

namespace WebReboot.Grind._808nd.com.CacheHelper
{

    public static class CacheHelpers
    {
        public static void RefreshCacheCollection<T>(IEnumerable<T> collection, string cacheItemName)
        {
            HttpRuntime.Cache.Insert(
                    cacheItemName,
                      collection,
                     null,
                     /* absoluteExpiration */ Cache.NoAbsoluteExpiration,
                     /* slidingExpiration */  Cache.NoSlidingExpiration,
                     /* priority */           CacheItemPriority.NotRemovable,
                     /* onRemoveCallback */   null);
        }

        public static void UpdateCardCache<T>(T newITem, string cacheCollectionName) where T : IPrimaryKeyable
        {
            var cards = HttpRuntime.Cache.Get(cacheCollectionName) as List<T>;
            var cardToUpdate = cards.FirstOrDefault(x => x.PrimaryKey == newITem.PrimaryKey);
            cards.Remove(cardToUpdate);
            cards.Add(newITem);

            HttpRuntime.Cache.Remove(cacheCollectionName);
            HttpRuntime.Cache.Insert(
                     cacheCollectionName,
                      cards.OrderByDescending(x => x.PrimaryKey).ToList(),
                     null,
                     /* absoluteExpiration */ Cache.NoAbsoluteExpiration,
                     /* slidingExpiration */  Cache.NoSlidingExpiration,
                     /* priority */           CacheItemPriority.NotRemovable,
                     /* onRemoveCallback */   null);
        }

        public static void AddCardCache<T>(T newITem, string cacheCollectionName) where T : IPrimaryKeyable
        {
            var cards = HttpRuntime.Cache.Get(cacheCollectionName) as List<T>;
            cards.Add(newITem);

            HttpRuntime.Cache.Insert(
                     cacheCollectionName,
                       cards.OrderByDescending(x => x.PrimaryKey).ToList(),
                     null,
                     /* absoluteExpiration */ Cache.NoAbsoluteExpiration,
                     /* slidingExpiration */  Cache.NoSlidingExpiration,
                     /* priority */           CacheItemPriority.NotRemovable,
                     /* onRemoveCallback */   null);
        }

    }


}