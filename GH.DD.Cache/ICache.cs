namespace GH.DD.Cache
{
    public interface ICache
    {
        ICacheEntry Get(string key);
        void Set(string key, ICacheEntry entry);
    }
}