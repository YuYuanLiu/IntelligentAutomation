﻿using CToolkit.v1_1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SensingNet.v0_2.DvcSensor;
using SensingNet.v0_2.DvcSensor.Protocol;
using SensingNet.v0_2.DvcSensor.SignalTrans;
using SensingNet.v0_2.Framework.Storage;

namespace SensingNet.TestMy
{
    [TestClass]
    public class UtVibrationPhys
    {
        SNetFileStorage fs = new SNetFileStorage(@"signals/vibration");

        [TestMethod]
        public void TestMethod()
        {

            CtkLog.RegisterEveryLogWrite((ss, ee) =>
            {
                System.Diagnostics.Debug.WriteLine(ee.Message);
            });

            var deviceHdl = new SNetDvcSensorHandler();
            deviceHdl.Config = new SNetDvcSensorCfg()
            {
                RemoteUri = "tcp://192.168.123.201:5000",
                TxInterval = 0,
                TimeoutResponse = 5000,
                ProtoConnect = SNetEnumProtoConnect.Tcp,
                ProtoFormat = SNetEnumProtoFormat.SNetCmd,
                ProtoSession = SNetEnumProtoSession.SNetCmd,
                SignalTran = SNetEnumSignalTrans.SNetCmd,
            };
            deviceHdl.Config.SignalCfgList.Add(new SNetSignalTransCfg()
            {
                Svid = 0,
            });
            deviceHdl.EhSignalCapture += (sender, ea) =>
            {
                fs.Write(ea);
            };





            using (deviceHdl)
            {
                deviceHdl.CfInit();
                deviceHdl.CfLoad();
                deviceHdl.CfRunLoop();
                deviceHdl.CfUnLoad();
                deviceHdl.CfFree();
                System.Threading.Thread.Sleep(100);
            }

        }


    }
}
