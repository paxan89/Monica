using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MonicaPlatform.IdentityServer4.Models
{
    /// <summary>
    /// Основной класс конфигурации для сериализации из XML
    /// </summary>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "configuration")]
    public class Configuration
    {
        private ConfigurationOptions _optionsField;

        public ConfigurationOptions Options
        {
            get => _optionsField;
            set => _optionsField = value;
        }
    }
}