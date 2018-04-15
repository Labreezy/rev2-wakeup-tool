using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GGXrdWakeupDPUtil
{
    public class ScriptInjector : IDisposable
    {
        private Frida.Script script;
        private Frida.DeviceManager deviceManager;
        private Frida.Device device;
        private Frida.Session session;
        private int scriptOffset = 0xb825e6;
        private Dispatcher dispatcher;
        private int pid;

        public ScriptInjector(Dispatcher dispatcher, int pid)
        {
            this.dispatcher = dispatcher;
            this.pid = pid;
        }
        public void Post(string value)
        {
            CreateScript(dispatcher, pid);

            script.Post(value);
        }

        private void CreateScript(Dispatcher dispatcher, int pid)
        {
            if (script == null)
            {
                deviceManager = new Frida.DeviceManager(dispatcher);
                device = deviceManager.EnumerateDevices().FirstOrDefault(x => x.Type == Frida.DeviceType.Local);



                if (device == null)
                {
                    throw new Exception("Local device not found.This application will now close.");
                }

                session = device.Attach((uint)pid);


                var src = @"var xrdbase = Module.findBaseAddress('GuiltyGearXrd.exe');
            var hookaddr = xrdbase.add(" + "0x" + scriptOffset.ToString("x") + @");
            var playingback = false;
            var running = true;
            Interceptor.attach(hookaddr, function(args){
            	if(playingback && this.context.edi.equals(ptr('3'))){
                	playingback = false;
                	this.context.ebp = ptr(Memory.readU32(this.context.edx).toString());
                  }
                });
            var quit = recv('quit', function (value) {
               Interceptor.detachAll();
               running = false;
            });
setTimeout( function () {
    while (running){        
        var op = recv('playback', function (value) {
        playingback=true;
            });
        op.wait();
    }
    }, 0);";

                script = session.CreateScript(src);
                script.Load();


            }
        }

        public void Dispose()
        {
            Disable();




            script?.Dispose();
            deviceManager?.Dispose();
            device?.Dispose();
            session?.Dispose();
        }

        public void Disable()
        {
            script.Post("{\"type\": \"quit\"}");
            script.Post("{\"type\": \"playback\"}");
            script.Unload();
            session.Detach();


            script = null;
        }
    }
}
