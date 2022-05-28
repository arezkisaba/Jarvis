﻿using Jarvis.Configuration.SecureAppSettings.Services.Contracts;

namespace Jarvis.Configuration.SecureAppSettings;

public class SecureAppSettingsConfigurationSource : IConfigurationSource
{
    private readonly ISecureAppSettingsService _secureAppSettingsService;

    public SecureAppSettingsConfigurationSource(
        ISecureAppSettingsService secureAppSettingsService)
    {
        _secureAppSettingsService = secureAppSettingsService;
    }

    public IConfigurationProvider Build(
        IConfigurationBuilder builder)
    {
        return new SecureAppSettingsConfigurationProvider(_secureAppSettingsService);
    }
}