using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Syinpo.Core.Helpers {
    public class CmdHelper {
        public static Tuple<string, string> StartProcess( string fileName, string arguments, string input ) {
            //  Create the process start info.
            var processStartInfo = new ProcessStartInfo( fileName, arguments );

            //  Set the options.
            processStartInfo.UseShellExecute = false;
            processStartInfo.ErrorDialog = false;
            processStartInfo.CreateNoWindow = true;

            //  Specify redirection.
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;

            //  Create the process.
            var process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo = processStartInfo;
            //process.Exited += currentProcess_Exited;

            //  Start the process.
            try {
                process.Start();
            }
            catch( Exception e ) {
                //  Trace the exception.
                Trace.WriteLine( "Failed to start process " + fileName + " with arguments '" + arguments + "'" );
                Trace.WriteLine( e.ToString() );
                return Tuple.Create( "", e.ToString() );
            }


            //  Create the readers and writers.
            var inputWriter = process.StandardInput;
            {
                inputWriter.WriteLine( input + "&exit" );
                inputWriter.AutoFlush = true;
                inputWriter.Flush();
            }

            var outputReader = process.StandardOutput.ReadToEnd();
            var errorReader = process.StandardError.ReadToEnd();



            //  Run the workers that read output and error.
            process.WaitForExit();//等待程序执行完退出进程
            process.Close();

            //AyscWriteConsoleOutput( outputReader, Color.White );
            //if( !string.IsNullOrEmpty( errorReader ) )
            //    AyscWriteConsoleOutput( outputReader, Color.Red );

            return Tuple.Create( outputReader, errorReader );
        }
    }
}
