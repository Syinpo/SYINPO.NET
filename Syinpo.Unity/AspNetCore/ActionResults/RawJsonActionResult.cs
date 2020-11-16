using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace IFengKe.Unity.AspNetCore.ActionResults {
    public class RawJsonActionResult : IActionResult {
        private readonly string _jsonString;

        public RawJsonActionResult( object value ) {
            if( value != null ) {
                _jsonString = value.ToString();
            }
        }

        public async Task ExecuteResultAsync( ActionContext context ) {
            if( context == null )
                throw new ArgumentNullException( nameof( context ) );

            var response = context.HttpContext.Response;

            response.StatusCode = 200;
            response.ContentType = "application/json";

            //using( TextWriter writer = new HttpResponseStreamWriter( response.Body, Encoding.UTF8 ) ) {
            //    writer.Write( _jsonString );
            //}

            //return Task.CompletedTask;

            await JsonSerializer.SerializeAsync( response.Body, _jsonString );
            await response.Body.FlushAsync();
        }
    }
}
