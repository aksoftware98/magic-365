﻿using Magic365.Core.Interfaces;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Magic365.Core.Services
{
	public class TokenProvider : IAccessTokenProvider
	{
		private readonly Func<Task<string>> _getTokenDelegate;
        
        public TokenProvider(Func<Task<string>> getTokenDelegate)
        {
            _getTokenDelegate = getTokenDelegate;
        }

        public AllowedHostsValidator AllowedHostsValidator { get; }

		public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object> additionalAuthenticationContext = default,
			CancellationToken cancellationToken = default)
		{
			return await _getTokenDelegate();
		}
		
	}
}
