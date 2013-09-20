using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace UserVoice.Dialect
{
    public class DialectEnumConverter : EnumConverter
    {
        /// <summary>
        /// 列挙値からその表示名に変換します。
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context,
                                         CultureInfo culture,
                                         object value,
                                         Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                // 対象となる列挙値の名前を取得します。
                string valueName = Enum.GetName(EnumType, value);
                if (valueName != null)
                {
                    // 表示名を取得します。
                    FieldInfo field = EnumType.GetField(valueName);
                    string name = GetDisplayName(field, culture);
                    if (name != null)
                    {
                        return name;
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// 表示名からその列挙値を取得します。
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context,
                                           CultureInfo culture,
                                           object valueToConvert)
        {
            string value = valueToConvert as string;
            if (value != null)
            {
                foreach (FieldInfo field in EnumType.GetFields())
                {
                    string name = GetDisplayName(field, culture);
                    if (name == value)
                    {
                        return field.GetValue(null);
                    }
                }
            }

            return base.ConvertFrom(context, culture, valueToConvert);
        }

        /// <summary>
        /// 列挙値の表示名を取得します。
        /// </summary>
        private string GetDisplayName(FieldInfo field, CultureInfo culture)
        {
            if (field == null)
            {
                return null;
            }

            Attribute attribute =
                Attribute.GetCustomAttribute(field, typeof(DialectAttribute));
            DialectAttribute dialectAttribute =
                attribute as DialectAttribute;
            return (dialectAttribute == null ?
                null :
                string.Format("{0}({1})",
                    dialectAttribute.DisplayName,
                    dialectAttribute.TagName));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DialectEnumConverter(Type type)
            : base(type)
        {
        }
    }
}
