﻿using CToolkit.v1_0;
using CToolkit.v1_0.Timing;
using SensingNet.v0_1.TimeSignal;
using SensingNet.v0_1.TriggerDiagram.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensingNet.v0_1.TriggerDiagram
{

    /// <summary>
    /// 用在Double(F8)序列資料節點
    /// </summary>
    public class SNetTdNodeF8 : SNetTdNode
    {

        public CtkTimeSecond? PrevTime;
        public int PurgeSeconds = 60;


        protected virtual void Purge()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 多段時間同時輸入時, 請自行分段輸入, 
        /// </summary>
        protected virtual void ProcDataInput(SNetTSignalsSecF8 tSignal, SNetTSignalSecF8 newSignals)
        {
            var ea = new SNetTdSignalsSecF8EventArg();
            ea.Sender = this;
            var time = newSignals.Time.HasValue ? newSignals.Time.Value : DateTime.Now;
            ea.Time = time;
            ea.TSignalSource = tSignal;
            ea.PrevTime = this.PrevTime;


            ea.TSignalNew.AddByKey(time, newSignals.Signals);
            tSignal.AddByKey(time, newSignals.Signals);
            this.OnDataChange(ea);

            this.Purge();

            this.PrevTime = time;
        }
  



        #region Event

        public event EventHandler<SNetTdSignalEventArg> evtDataChange;
        protected void OnDataChange(SNetTdSignalEventArg ea)
        {
            if (this.evtDataChange == null) return;
            this.evtDataChange(this, ea);
        }

        #endregion



        #region IDisposable

        protected override void DisposeSelf()
        {
            base.DisposeSelf();
        }

        #endregion


        #region Static

        public static void PurgeSignalByCount(SNetTSignalsSecF8 tSignal, int Count)
        {
            var query = tSignal.Signals.Take(tSignal.Signals.Count - Count).ToList();
            foreach (var ok in query)
                tSignal.Signals.Remove(ok.Key);
        }
        public static void PurgeSignalByTime(SNetTSignalsSecF8 tSignal, CtkTimeSecond time)
        {
            var now = DateTime.Now;
            var query = tSignal.Signals.Where(x => x.Key < time).ToList();
            foreach (var ok in query)
                tSignal.Signals.Remove(ok.Key);
        }

        #endregion

    }
}
