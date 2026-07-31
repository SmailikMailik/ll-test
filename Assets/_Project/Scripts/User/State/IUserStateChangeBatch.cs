using System;

namespace LL.User.State
{
    internal interface IUserStateChangeBatch
    {
        TResult Execute<TResult>(Func<TResult> mutation);
    }
}