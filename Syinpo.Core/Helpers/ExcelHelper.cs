using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace Syinpo.Core.Helpers {
    /// <summary>
    /// 导入导出
    /// </summary>
    public static class ExcelHelper {
        /// <summary>
        /// excel目录拼接当前日期的目录路径
        /// </summary>
        /// <returns></returns>
        public static string ExcelPath {
            get {
                return $"excel/{DateTime.Now.ToString( "yyyyMM" )}/{DateTime.Now.ToString( "dd" )}/";
            }
        }

        /// <summary>
        /// 在服务器中生成Excel文件
        /// </summary>
        /// <typeparam name="T">Descritionp描述的T</typeparam>
        /// <param name="path">/wwwroot下面的日期目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="list">数据源</param>
        public static void MakeExcelFile<T>( string path, string fileName, IList<T> list ) {
            string root = "/wwwroot/";
            string sFileName = string.Empty;
            string sWebRootFolder = Directory.GetCurrentDirectory() + root + path;
            if( fileName.Contains( ".xlsx" ) || fileName.Contains( ".xlx" ) ) {
                sFileName = fileName;
                fileName = fileName.Substring( 0, fileName.LastIndexOf( "." ) + 1 );
            }
            else {
                sFileName = $"{fileName}.xlsx";
            }

            //创建文件目录
            if( !Directory.Exists( sWebRootFolder ) ) {
                Directory.CreateDirectory( sWebRootFolder );
            }

            FileInfo fileInfo = new FileInfo( Path.Combine( sWebRootFolder, sFileName ) );
            using( ExcelPackage package = new ExcelPackage( fileInfo ) ) {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add( fileName )
                    .BindValues( list );
                package.Save();
            }
        }

        private static ExcelWorksheet BindValues<T>( this ExcelWorksheet worksheet, IList<T> list ) {
            int index = 0;
            PropertyInfo[] propertyInfos = typeof( T ).GetProperties();
            foreach( var propertyInfo in propertyInfos ) {
                int col = index + 1;  //Cell的col下表是从1开始
                object[] objs = propertyInfo.GetCustomAttributes( typeof( DescriptionAttribute ), false );
                if( objs == null || objs.Length == 0 )
                    continue; //无DescriptionAttribute的字段不写入ExcelWorkSheet的标题

                //worksheet.Cells[1, col].Value = string.Empty;
                DescriptionAttribute descriptionAttribute = objs[ 0 ] as DescriptionAttribute;
                worksheet.Cells[ 1, col ].Value = descriptionAttribute.Description;

                index++;
            }

            int row = 2;
            foreach( var item in list ) {
                int num = 0;
                foreach( var propertyInfo in propertyInfos ) {
                    if( !propertyInfo.IsDefined( typeof( DescriptionAttribute ), false ) )
                        continue;

                    int col = num + 1;  //Cell的col下表是从1开始
                    var val = propertyInfo.GetValue( item );

                    if( propertyInfo.PropertyType == typeof( DateTime ) || propertyInfo.PropertyType == typeof( DateTime? ) ) {
                        worksheet.Cells[ row, col ].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                    }

                    worksheet.Cells[ row, col ].Value = val;

                    num++;
                }
                row++;
            }
            return worksheet;
        }


        public static Dictionary<string,int> GetColumns( ExcelWorksheet workSheet )
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            for( var column = workSheet.Dimension.Start.Column; column <= workSheet.Dimension.End.Column; column++ ) {
                if( workSheet.Cells[ 1, column ].Value is null )
                    throw new Exception( $"没有值 {column}." );

                var columnName = workSheet.Cells[ 1, column ].Value.ToString();

                result.Add(columnName, column);
            }

            return result;
        }
    }
}
