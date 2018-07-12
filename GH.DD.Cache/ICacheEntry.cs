using System;

namespace GH.DD.Cache
{
    public interface ICacheEntry
    {
        string Key { get; }
        object Value { get; }
        
        bool IsAutoDeleted { get; }
        bool IsExpired();

        void ExecuteBeforeDeleteCallback();
        void ExecuteAfterDeleteCallback();
    }
}