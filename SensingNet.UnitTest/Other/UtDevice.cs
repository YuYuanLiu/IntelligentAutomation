﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using SensingNet.v0_0;

namespace SensingNet.MyTest
{
    [TestClass]
    public class UtDevice
    {
        [TestMethod]
        public void TestMethod1()
        {


            using (var simCmd = new SimulateCmdProto("127.0.0.1", 5000))
            using (var execer = new v0_0.Signal.SignalMgrExecer())
            {
                simCmd.StartConnect();




                var bgWorker2 = new BackgroundWorker();
                bgWorker2.WorkerSupportsCancellation = true;
                execer.evtSignalCapture += delegate (object sender, SignalEventArgs e)
                {
                    System.Diagnostics.Debug.WriteLine("{0}, {1}", e.ToolId,
                         String.Join(",", e.calibrateData));
                };
                bgWorker2.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    execer.CfInit();
                    execer.CfLoad();
                    execer.CfRun();
                    execer.CfUnLoad();
                    execer.CfFree();
                };
                bgWorker2.RunWorkerAsync();


                try
                {
                    var flag = true;
                    while (flag)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }

                }
                finally
                {
                    bgWorker2.CancelAsync();
                }




            }








        }


    }
}
