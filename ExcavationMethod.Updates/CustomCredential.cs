using ExcavationMethod.Authentication;
using Azure.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExcavationMethod.Updates
{
    internal class CustomCredential : TokenCredential
    {
        private readonly IntPtr _windowHandle;

        public CustomCredential(IntPtr windowHandle) => _windowHandle = windowHandle;

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            var result = await Authenticator.AcquireTokenAsync(_windowHandle).ConfigureAwait(false);
            return new AccessToken(result.AccessToken, result.ExpiresOn);
        }
    }
}
