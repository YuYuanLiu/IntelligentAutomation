﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using CToolkit.Secs;
using System.Net;
using SensingNet.SecsMgr;
using CToolkit.Net;
using System.Text;

namespace SensingNet.MyTest
{
    [TestClass]
    public class UtPassConn
    {
        [TestMethod]
        public void SecsTest()
        {

            using (var signalmgr = new SignalMgr.SignalMgrExecer())
            {


                signalmgr.evtSignalCapture += delegate (object sender, SignalEventArgs e)
                {

                };
                CToolkit.CtkUtil.RunWorkerAsyn(delegate (object sen, DoWorkEventArgs dwea)
                {
                    signalmgr.CfInit();
                    signalmgr.CfLoad();
                    signalmgr.CfRun();
                    signalmgr.CfUnload();
                    signalmgr.CfFree();
                });





                CToolkit.Net.Test_CtkTcpClient_Asyn.Test(
                    "127.0.0.1",
                    5001,
                    delegate (Test_CtkTcpClient_Asyn obj)
                    {
                        return 0;
                    },
                    delegate (Test_CtkTcpClient_Asyn obj, byte[] buffer, int length)
                    {
                        var cmd = Encoding.UTF8.GetString(buffer, 0 , length);
                        System.Diagnostics.Debug.WriteLine(cmd);

                        return 0;
                    },
                    delegate (Test_CtkTcpClient_Asyn obj)
                    {
                        if (obj.client == null || !obj.client.Connected) return 0;
                        var stream = obj.client.GetStream();
                        var buff = Encoding.UTF8.GetBytes("Hello Server\r\n");
                        stream.Write(buff, 0, buff.Length);
                        return 0;
                    }
                );




                //signalmgr.isExec = false;
                System.Diagnostics.Debug.WriteLine("");

            }


        }




    }
}
