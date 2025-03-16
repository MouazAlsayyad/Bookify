﻿using Microsoft.AspNetCore.Authorization;

namespace Bookify.Infrastructure.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(permission)
    {
    }
}
