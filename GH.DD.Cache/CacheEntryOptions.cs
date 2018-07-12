using System;

namespace GH.DD.Cache
{
    public class CacheEntryOptions
    {
        public TimeSpan? Ttl { set; get; } = null;
        public bool IsAutoDeleted { set; get; } = true;
        public BeforeDeleteDelegate BeforeDeleteCallback { set; get; } = null;
        public AfterDeleteDelegate AfterDeleteCallback { set; get; } = null;
    }
}