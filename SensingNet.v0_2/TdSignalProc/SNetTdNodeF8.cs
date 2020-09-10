﻿using CToolkit.v1_1;
using CToolkit.v1_1.Timing;
using SensingNet.v0_2.TimeSignal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SensingNet.v0_2.TdBase;

namespace SensingNet.v0_2.TdSignalProc
{

    /// <summary>
    /// 用在Double(F8)序列資料節點
    /// </summary>
    public class SNetTdNodeF8 : SNetTdNode
    {
        public CtkTimeSecond? PrevTime;

        public int PurgeSeconds = 60;

        ~SNetTdNodeF8() { this.Dispose(false); }
        /// <summary>
        /// 簡易處理方式, 多段時間同時輸入時, 請自行分段輸入.
        /// </summary>
        /// <param name="tSignal">原始訊號來源</param>
        /// <param name="newSignals">本次要新增的訊號</param>
        protected virtual void ProcAndPushData(SNetTSignalSetSecF8 tSignal, SNetTSignalSecF8 newSignals)
        {
            var ea = new SNetTdSignalSetSecF8EventArg();
            ea.Sender = this;
            var time = newSignals.Time.HasValue ? newSignals.Time.Value : DateTime.Now;
            ea.Time = time;
            ea.TSignalSource = tSignal;
            ea.PrevTime = this.PrevTime;


            ea.TSignalNew.Add(time, newSignals.Signals);
            tSignal.Add(time, newSignals.Signals);
            this.OnDataChange(ea);

            this.Purge();

            this.PrevTime = time;
        }

        protected virtual void Purge()
        {
            throw new NotImplementedException();
        }
        //protected virtual void ProcAndPushData(SNetTSignalSetSecF8 tSignal, SNetTSignalSetSecF8 newSignals)
        //{
        //    var ea = new SNetTdSignalSetSecF8EventArg();
        //    ea.Sender = this;
        //    ea.TSignalSource = tSignal;

        //    foreach (var s in newSignals.Signals)
        //    {
        //        var time = s.Key.DateTime;
        //        ea.TSignalNew.AddByKey(s.Key, s.Value);
        //        tSignal.AddByKey(s.Key, s.Value);


        //        ea.Time = time;
        //        this.PrevTime = ea.PrevTime = this.PrevTime;
        //    }

        //    this.OnDataChange(ea);
        //    this.Purge();

        //}




        #region Event

        public event EventHandler<SNetTdSignalEventArg> EhDataChange;
        protected void OnDataChange(SNetTdSignalEventArg ea)
        {
            if (this.EhDataChange == null) return;
            this.EhDataChange(this, ea);
        }

        #endregion



        #region IDisposable

        protected override void DisposeSelf()
        {
            base.DisposeSelf();
        }

        #endregion


        #region Static

        public static void PurgeSignalByCount(SNetTSignalSetSecF8 tSignal, int Count)
        {
            var query = tSignal.Signals.Take(tSignal.Signals.Count - Count).ToList();
            foreach (var ok in query)
                tSignal.Signals.Remove(ok.Key);
        }
        public static void PurgeSignalByTime(SNetTSignalSetSecF8 tSignal, CtkTimeSecond time)
        {
            var now = DateTime.Now;
            var query = tSignal.Signals.Where(x => x.Key < time).ToList();
            foreach (var row in query)
                tSignal.Signals.Remove(row.Key);
        }

        public static void PurgeSignalByTime(SNetTSignalSetSecF8 tSignal, CtkTimeSecond start, CtkTimeSecond end)
        {
            var now = DateTime.Now;
            var query = (from row in tSignal.Signals
                         where row.Key < start || row.Key > end
                         select row).ToList();
            foreach (var row in query)
                tSignal.Signals.Remove(row.Key);
        }

        #endregion

    }
}
