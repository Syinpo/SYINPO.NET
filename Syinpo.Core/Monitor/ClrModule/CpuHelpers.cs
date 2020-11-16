using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Syinpo.Core.Monitor.PackModule;
using Microsoft.Extensions.Logging;
using OpenHardwareMonitor.Hardware;

namespace Syinpo.Core.Monitor.ClrModule {
    public static class CpuHelpers {
        private const int interval = 1000;
        private static double _usagePercent;
        private static double _totalCpuUsagePercent;
        private static double _totalRamUsagePercent;
        private static readonly Task _task;

        public static double UsagePercent => _usagePercent;
        public static double TotalCpuUsagePercent => _totalCpuUsagePercent;
        public static double TotalRamUsagePercent => _totalRamUsagePercent;

        static CpuHelpers()
        {
            return;

            info INFO = new info();
            INFO.init();

            var process = Process.GetCurrentProcess();
            _task = Task.Factory.StartNew( async () =>
            {
                var _prevCpuTime = process.TotalProcessorTime.TotalMilliseconds;
                while( true ) {
                    try
                    {
                        // server
                        var serverHard = INFO.GetData();
                        Interlocked.Exchange(ref _totalCpuUsagePercent, Convert.ToDouble(serverHard.cpu ?? 0));
                        Interlocked.Exchange(ref _totalRamUsagePercent, Convert.ToDouble(serverHard.ram ?? 0));


                        // 当前托管程序的cpu使用率
                        var prevCpuTime = _prevCpuTime;
                        var currentCpuTime = process.TotalProcessorTime;
                        var usagePercent = (currentCpuTime.TotalMilliseconds - prevCpuTime) / interval;
                        Interlocked.Exchange(ref _prevCpuTime, currentCpuTime.TotalMilliseconds);
                        Interlocked.Exchange(ref _usagePercent, usagePercent);
                    }
                    catch (Exception ex)
                    {
                        IoC.Resolve<ILogger<TimeData>>().LogError( ex, "CpuHelpers Error."  );
                    }

                    await Task.Delay( 1000 );
                }
            } );
        }
    }


    class info {
        public string SysInfo;
        UpdateVisitor updateVisitor;
        public Computer computer;
        public struct Source {
            public int num;
            public bool CPU;
            public int CPUindex;
            public int CPUNUM;
            public bool GPU;
            public int GPUindex;
            public int GPUNUM;
        }
        public Source source;
        public struct CPUinfo {
            public bool Tem;
            public int TemSensorNum;
            public int TemSensorindex;
            public bool Load;
            public int LoadSensorNum;
            public int LoadSensorindex;
            public bool Clock;
            public int ClockSensorNum;
            public int ClockSensorindex;
        }
        public CPUinfo cpuinfo;
        public info() {

        }
        public void init()//硬件初始化
        {
            source = new Source();
            source.CPU = false;
            source.GPU = false;
            cpuinfo = new CPUinfo();
            cpuinfo.Tem = false;
            cpuinfo.Load = false;
            cpuinfo.Clock = false;
            updateVisitor = new UpdateVisitor();
            computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;
            computer.GPUEnabled = true;
            computer.HDDEnabled = true;
            computer.RAMEnabled = true;
            computer.Accept( updateVisitor );
            source.num = computer.Hardware.Length;
            for( int i = 0; i < computer.Hardware.Length; i++ ) {
                if( computer.Hardware[ i ].HardwareType == HardwareType.CPU ) {
                    source.CPU = true;
                    source.CPUindex = i;
                }
                if( computer.Hardware[ i ].HardwareType == HardwareType.GpuAti ) {
                    source.GPU = true;
                    source.GPUindex = i;
                }
                if( computer.Hardware[ i ].HardwareType == HardwareType.GpuNvidia ) {
                    source.GPU = true;
                    source.GPUindex = i;
                }
            }
            //CPUINFO初始化
            int num = 0;
            //Load初始化
            for( int i = 0; i < computer.Hardware[ source.CPUindex ].Sensors.Length; i++ ) {
                if( computer.Hardware[ source.CPUindex ].Sensors[ i ].SensorType == SensorType.Load ) {
                    cpuinfo.Load = true;
                    cpuinfo.LoadSensorindex = i;
                    break;
                }
            }
            for( int i = 0; i < computer.Hardware[ source.CPUindex ].Sensors.Length; i++ ) {
                if( computer.Hardware[ source.CPUindex ].Sensors[ i ].SensorType == SensorType.Load ) {
                    num++;
                }
            }
            cpuinfo.LoadSensorNum = num;
            num = 0;
            computer.Close();
        }

        public (float? cpu, float? ram) GetData() {
            updateVisitor = new UpdateVisitor();
            computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;
            computer.GPUEnabled = true;
            computer.HDDEnabled = true;
            computer.RAMEnabled = true;
            computer.Accept( updateVisitor );

            var ram = computer.Hardware.First( x => x.HardwareType == HardwareType.RAM );
            var ramServer = ram.Sensors.FirstOrDefault( f => f.SensorType == SensorType.Load )?.Value;

            if( source.CPU && cpuinfo.Load ) {
                var cpuServer = computer.Hardware[ source.CPUindex ].Sensors[ cpuinfo.LoadSensorNum - 1 ].Value;
                return (cpuServer, ramServer);
            }

            return (null, ramServer);
        }
        public class UpdateVisitor : IVisitor {
            public void VisitComputer( IComputer computer ) {
                computer.Traverse( this );
            }
            public void VisitHardware( IHardware hardware ) {
                hardware.Update();
                foreach( IHardware subHardware in hardware.SubHardware )
                    subHardware.Accept( this );
            }
            public void VisitSensor( ISensor sensor ) {
            }
            public void VisitParameter( IParameter parameter ) {
            }
        }

    }

}
