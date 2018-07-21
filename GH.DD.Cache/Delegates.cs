using System;

namespace GH.DD.Cache
{
    public delegate Action BeforeDeleteDelegate(ICacheEntry entry);
    public delegate Action AfterDeleteDelegate(ICacheEntry entry);
}