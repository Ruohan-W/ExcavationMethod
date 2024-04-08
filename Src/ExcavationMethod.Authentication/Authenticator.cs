using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;
using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcavationMethod.Authentication
{
    public static class Authenticator
    {
        private const string ClientId = AADConfig.ClientId;
        private static readonly string[] Scopes = { "User.Read" };
        private static readonly Lazy<Task<IPublicClientApplication>> Application = new(InitializeApplication);

        public static async Task<AuthenticationResult> AcquireTokenAsync(IntPtr windowHandle)
        {
            // 1. Get the initialized public client application instance.
            var application = await Application.Value;

            // 2. Find an account for silent login. Is there an account in the cache?
            var accountToLogin = (await application.GetAccountsAsync()).FirstOrDefault();
            if (accountToLogin == null)
            {
                // 3. No account in the cache; try to log in with the OS account.
                accountToLogin = PublicClientApplication.OperatingSystemAccount;
            }

            try
            {
                // 4. Silent authentication 
                var authResult = await application.AcquireTokenSilent(Scopes, accountToLogin).ExecuteAsync();
                return authResult;
            }
            // Cannot log in silently - most likely Azure AD would show a consent dialog or the user needs to re-enter credentials.
            catch (MsalUiRequiredException)
            {
                // 5. Interactive authentication
                var authResult = await application.AcquireTokenInteractive(Scopes)
                    .WithAccount(accountToLogin)
                    // This is mandatory so that WAM is correctly parented to your app; read on for more guidance.
                    .WithParentActivityOrWindow(windowHandle)
                    .ExecuteAsync();

                // Consider allowing the user to re-authenticate with a different account, by calling AcquireTokenInteractive again.
                return authResult;
            }

        }

        private static async Task<IPublicClientApplication> InitializeApplication()
        {
            var pluginDirectory = Path.GetDirectoryName(typeof(Authenticator).Assembly.Location);
            var storageProperties = new StorageCreationPropertiesBuilder("msalcache.bin3", pluginDirectory)
                .Build();
            var application = PublicClientApplicationBuilder.Create(ClientId)
                .WithBroker(new BrokerOptions(BrokerOptions.OperatingSystems.Windows))
                .Build();

            // Add a token cache.
            // https://learn.microsoft.com/azure/active-directory/develop/msal-net-token-cache-serialization?tabs=desktop

            var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
            cacheHelper.VerifyPersistence();
            cacheHelper.RegisterCache(application.UserTokenCache);

            return application;
        }
    }
}
