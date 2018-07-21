namespace GH.DD.Cache
{
    public interface ICache
    {
        ICacheEntry Get(object key);
        void Set(object key, ICacheEntry entry);
    }
}