using System;

namespace GH.DD.Cache
{
    /// <summary>
    /// Delegate for execute before remove Cache entry
    /// </summary>
    /// <param name="entry">Cache entry <see cref="ICacheEntry"/></param>
    public delegate Action BeforeDeleteDelegate(ICacheEntry entry);
    
    /// <summary>
    /// Delegate for execute after remove Cache entry
    /// </summary>
    /// <param name="entry">Cache entry <see cref="ICacheEntry"/></param>
    public delegate Action AfterDeleteDelegate(ICacheEntry entry);
}