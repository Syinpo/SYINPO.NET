using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using AutoMapper.Configuration;

namespace Syinpo.Core.Mapper {
    public static class BaseMapperConfiguration {
        private static MapperConfigurationExpression s_mapperConfigurationExpression;
        private static IMapper s_mapper;
        private static readonly object s_mapperLockObject = new object();

        public static MapperConfigurationExpression MapperConfigurationExpression => s_mapperConfigurationExpression ??
                                                                                     ( s_mapperConfigurationExpression = new MapperConfigurationExpression() );

        public static IMapper Mapper {
            get {
                if( s_mapper == null ) {
                    lock( s_mapperLockObject ) {
                        if( s_mapper == null ) {
                            var mapperConfiguration = new MapperConfiguration( MapperConfigurationExpression );

                            s_mapper = mapperConfiguration.CreateMapper();
                        }
                    }
                }

                return s_mapper;
            }
        }

        public static TDestination MapTo<TSource, TDestination>( this TSource source ) {
            return Mapper.Map<TSource, TDestination>( source );
        }

        public static TDestination MapTo<TSource, TDestination>( this TSource source, TDestination destination ) {
            return Mapper.Map( source, destination );
        }

        public static T MapTo<T>( this object obj ) {
            if (obj == null)
                return default(T);

            return Mapper.Map<T>( obj );
        }

        public static TDestination MapTo<TDestination>( this object source, TDestination destination ) {
            return Mapper.Map( source, destination );
        }

        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TDestination>( this IEnumerable source ) {
            return Mapper.Map<List<TDestination>>( source );
        }
        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TSource, TDestination>( this IEnumerable<TSource> source ) {
            return Mapper.Map<List<TDestination>>( source );
        }
    }
}
