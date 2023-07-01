using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Magic365.WinUI;
public static class TokenCacheHelper
{
    // Define the path of the cache file
    public static readonly string CacheFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location + ".msalcache.bin3";

    // Define a lock object to prevent concurrent access to the file
    private static readonly object FileLock = new object();

    // Enable the serialization of the TokenCache
    public static void EnableSerialization(ITokenCache tokenCache)
    {
        tokenCache.SetBeforeAccess(BeforeAccessNotification);
        tokenCache.SetAfterAccess(AfterAccessNotification);
    }

    // Read the cache file before accessing the cache
    private static void BeforeAccessNotification(TokenCacheNotificationArgs args)
    {
        lock (FileLock)
        {
            args.TokenCache.DeserializeMsalV3(File.Exists(CacheFilePath)
                ? File.ReadAllBytes(CacheFilePath)
                : null);
        }
    }

    // Write the cache file after accessing the cache
    private static void AfterAccessNotification(TokenCacheNotificationArgs args)
    {
        // if the access operation resulted in a cache update
        if (args.HasStateChanged)
        {
            lock (FileLock)
            {
                // reflect changes in the persistent store
                File.WriteAllBytes(CacheFilePath, args.TokenCache.SerializeMsalV3());
            }
        }
    }
}
