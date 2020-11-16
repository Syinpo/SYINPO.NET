using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Syinpo.Core.Extensions {
    public static class EnumExtension {
        public static string GetDisplayName( this Enum value ) {
            //Type type = value.GetType();
            //string name = Enum.GetName( type, value );
            //if( name != null ) {
            //    FieldInfo field = type.GetField( name );
            //    if( field != null ) {
            //        DisplayAttribute attr = Attribute.GetCustomAttribute( field,
            //            typeof( DisplayAttribute ) ) as DisplayAttribute;
            //        if( attr != null ) {
            //            return attr.GetName();
            //        }
            //    }
            //}
            //return null;



            var result = null as string;
            var memberInfos = value.GetType()
                .GetMember( value.ToString() );
            if( memberInfos == null || memberInfos.Count() == 0 )
                return "未知";

            var memberInfo = memberInfos.First();
            if( memberInfo == null )
                return "未知";

            var display = memberInfo
                .GetCustomAttributes( false )
                .OfType<DisplayAttribute>()
                .LastOrDefault();

            if( display != null ) {
                result = display.Name;
            }

            return result ?? value.ToString();
        }

        public static string GetDisplayDescription( this Enum value ) {

            var result = null as string;

            var display = value.GetType()
                .GetMember( value.ToString() ).First()
                .GetCustomAttributes( false )
                .OfType<DisplayAttribute>()
                .LastOrDefault();

            if( display != null ) {
                result = display.GetDescription();
            }

            return result ?? value.ToString();
        }

        public static bool GetBool( this Enum value ) {
            var result = null as bool?;

            var display = value.GetType()
                .GetMember( value.ToString() ).First()
                .GetCustomAttributes( false )
                .OfType<DisplayAttribute>()
                .LastOrDefault();

            if( display != null ) {
                result = display.GetAutoGenerateField();
            }

            return result ?? false;
        }

        public static string GetDisplayGroupName( this Enum value ) {

            var result = null as string;

            var display = value.GetType()
                .GetMember( value.ToString() ).First()
                .GetCustomAttributes( false )
                .OfType<DisplayAttribute>()
                .LastOrDefault();

            if( display != null ) {
                result = display.GetGroupName();
            }

            return result ?? value.ToString();
        }

        public static string GetDisplayPrompt( this Enum value ) {

            var result = null as string;

            var display = value.GetType()
                .GetMember( value.ToString() ).First()
                .GetCustomAttributes( false )
                .OfType<DisplayAttribute>()
                .LastOrDefault();

            if( display != null ) {
                result = display.GetPrompt();
            }

            return result ?? value.ToString();
        }

        public static Type GetModelType( this Enum value ) {

            Type result = default( Type );

            var display = value.GetType()
                .GetMember( value.ToString() ).First()
                .GetCustomAttributes( false )
                .OfType<DisplayAttribute>()
                .LastOrDefault();

            if( display != null ) {
                result = display.ResourceType;
            }

            return result;
        }

        public static Dictionary<string, int> GetDropdownList( this Enum value, bool useDisplayName = true ) {
            Dictionary<string, int> list = new Dictionary<string, int>();

            var type = value.GetType();


            string[] names = Enum.GetNames( type );
            var values = Enum.GetValues( type );

            int count = names.Length;
            for( int i = 0; i < count; i++ ) {
                if( !useDisplayName )
                    list.Add( names[ i ], Convert.ToInt32( values.GetValue( i ) ) );
                else
                    list.Add(( values.GetValue( i ) as Enum).GetDisplayName(), Convert.ToInt32( values.GetValue( i ) ) );
            }

            return list;
        }
    }
}
