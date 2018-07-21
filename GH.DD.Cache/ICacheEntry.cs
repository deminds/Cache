using System;

namespace GH.DD.Cache
{
    public interface ICacheEntry
    {
        object Key { get; }
        object Value { get; }
        
        bool IsAutoDeleted { get; }
        bool IsExpired();

        void UpdateValue(object value);

        void ExecuteBeforeDeleteCallback();
        void ExecuteAfterDeleteCallback();
    }
}