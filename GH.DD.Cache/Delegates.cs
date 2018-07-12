using System;

namespace GH.DD.Cache
{
    public delegate Action BeforeDeleteDelegate(string key, object value);
    public delegate Action AfterDeleteDelegate(string key, object value);
}