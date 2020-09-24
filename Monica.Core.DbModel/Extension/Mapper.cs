using System;
using System.Linq;
using System.Reflection;

namespace Monica.Core.DbModel.Extension
{
    public static class Mapper
    {
        public static TModel Map<TModel>(this object modelDto, TModel model = null) where TModel : class
        {
            model = model ?? Activator.CreateInstance<TModel>();
            var properties = modelDto.GetType().GetProperties().Where(f => f.CanWrite);
            foreach (PropertyInfo info in properties)
            {
                var propInfo = typeof(TModel).GetProperty(info.Name);
                if (propInfo == null)
                    continue;
                propInfo.SetValue(model, info.GetValue(modelDto));
            }

            return model;
        }
    }
}
