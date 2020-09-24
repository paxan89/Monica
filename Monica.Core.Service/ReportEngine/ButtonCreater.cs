using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelDto.Report;
using Newtonsoft.Json.Linq;

namespace Monica.Core.Service.ReportEngine
{
    public class ButtonCreater : IButtonCreater
    {
        /// <summary>
        /// Получить список доступных действий для данного режима
        /// </summary>
        /// <param name="buttons">Список описанныз</param>
        /// <returns></returns>
        public async Task<JArray> GetButtonJson(IEnumerable<ButtonAccessDto> buttons)
        {
            var jArray = new JArray();
            foreach (var button in buttons)
            {
                var jButton = new JObject();
                jButton.Add("widget", new JValue("dxButton"));
                jButton.Add("location", new JValue(await GetLocation(button)));
                jButton.Add("sysName", new JValue(button.SysName));
                var jOptions = new JObject();
                jOptions.Add("icon", new JValue(button.IconName));
                jOptions.Add("hint", new JValue(button.ToolTip));
                jOptions.Add("text", new JValue(button.DisplayName));
                if (!string.IsNullOrWhiteSpace(button.ValidationGroup))
                    jOptions.Add("validationGroup", new JValue(button.ValidationGroup));
                jOptions.Add("type", new JValue(await GetTypeBtn(button)));
                jOptions.Add("stylingMode", new JValue(await GetStyleMode(button)));
                jButton.Add("onClick", new JValue("action"));
                jButton.Add("options", jOptions);
                jArray.Add(jButton);
            }

            return jArray;
        }

        private Task<string> GetLocation(ButtonAccessDto btn)
        {
            if (btn.Location == Location.After)
                return Task.FromResult("after");
            if (btn.Location == Location.Center)
                return Task.FromResult("center");
            if (btn.Location == Location.Before)
                return Task.FromResult("before");
            return Task.FromResult("after");
        }

        private Task<string> GetStyleMode(ButtonAccessDto btn)
        {
            if (btn.StylingMode == StylingMode.Contained)
                return Task.FromResult("contained");
            if (btn.StylingMode == StylingMode.Outlined)
                return Task.FromResult("outlined");
            return Task.FromResult("contained");
        }

        private Task<string> GetTypeBtn(ButtonAccessDto btn)
        {
            if (btn.TypeBtn == TypeBtn.Success)
                return Task.FromResult("success");
            if (btn.TypeBtn == TypeBtn.Danger)
                return Task.FromResult("danger");
            return Task.FromResult("normal");
        }
    }
 }
