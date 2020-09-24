using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MonicaPlatform.IdentityServer4.Models
{
    /// <summary>
    /// Опции для Claims
    /// </summary>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, TypeName = "configurationOptionsClaims")]
    public class ConfigurationOptionsClaims
    {
        private string _nameClaimTypeField;

        private string _roleClaimTypeField;

        public string NameClaimType
        {
            get => _nameClaimTypeField;
            set => _nameClaimTypeField = value;
        }

        public string RoleClaimType
        {
            get => _roleClaimTypeField;
            set => _roleClaimTypeField = value;
        }
    }
}