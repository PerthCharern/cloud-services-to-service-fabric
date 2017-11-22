﻿namespace Tailspin.SurveyManagementService.Configuration
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Description;
    using Microsoft.WindowsAzure.Storage;

    public static class ServiceFabricConfiguration
    {
        public static string GetConfigurationSettingValue(string sectionName, string settingName, string defaultValue, string package = "Config")
        {
            // Defaulting to Config package though in theory different folders may be created for different settings collection
            var configurationPackage = FabricRuntime.GetActivationContext().GetConfigurationPackageObject(package);
            var settingValue = (configurationPackage.Settings.Sections[sectionName]?.Parameters[settingName]?.Value) ?? defaultValue;
            return settingValue;
        }

        public static CloudStorageAccount GetCloudStorageAccount()
        {
            var storageAccountConnectionString = GetConfigurationSettingValue(sectionName: "ConnectionStrings",
                settingName: "DataConnectionString", defaultValue: "UseDevelopmentStorage=true");

            return CloudStorageAccount.Parse(storageAccountConnectionString);
        }

        public static CloudStorageAccount GetCloudCosmosAccount()
        {
            var storageAccountConnectionString = GetConfigurationSettingValue(sectionName: "ConnectionStrings",
                settingName: "CosmosConnectionString", defaultValue: "UseDevelopmentStorage=true");

            return CloudStorageAccount.Parse(storageAccountConnectionString);
        }
    }
}
