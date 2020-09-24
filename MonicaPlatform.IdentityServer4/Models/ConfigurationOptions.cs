using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MonicaPlatform.IdentityServer4.Models
{
    /// <summary>
    /// Опции конфигуратора
    /// </summary>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, TypeName = "configurationOptions")]
    public class ConfigurationOptions
    {
        private string _apiNameField;

        private string _authorityField;

        private ConfigurationOptionsClaims _claimsField;

        private bool _requireHttpsMetadataField;

        public string Authority
        {
            get => _authorityField;
            set => _authorityField = value;
        }

        public bool RequireHttpsMetadata
        {
            get => _requireHttpsMetadataField;
            set => _requireHttpsMetadataField = value;
        }

        public string ApiName
        {
            get => _apiNameField;
            set => _apiNameField = value;
        }

        public ConfigurationOptionsClaims Claims
        {
            get => _claimsField;
            set => _claimsField = value;
        }
    }
}