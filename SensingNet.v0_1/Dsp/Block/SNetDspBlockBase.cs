﻿using CToolkit.v0_1;
using CToolkit.v0_1.Timing;
using SensingNet.v0_1.Dsp.TimeSignal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensingNet.v0_1.Dsp.Block
{
    public class SNetDspBlockBase : ISNetDspBlock, IDisposable
    {
        public bool IsEnalbed = true;
        public int PurgeSeconds = 60;
        public Object PrevTime;//存放結構時:CtkTimeSecond, 仍可為null, 因此本身是物件形態
        protected String _identifier = Guid.NewGuid().ToString();
        ~SNetDspBlockBase() { this.Dispose(false); }


        public string SNetDspIdentifier { get { return this._identifier; } set { this._identifier = value; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tSignal">來源資料, 集合會被修改</param>
        /// <param name="time"></param>
        /// <param name="newDatas"></param>
        protected virtual void DoDataChange(SNetDspTimeSignalSetSecond tSignal, CtkTimeSecond time, IEnumerable<double> newDatas)
        {
            var ea = new SNetDspBlockTimeSignalSetSecondEventArg();
            ea.Time = time;
            ea.Sender = this;
            ea.TSignal = tSignal;
            ea.PrevTime = (CtkTimeSecond?)this.PrevTime;

            ea.NewTSignal.AddByKey(time, newDatas);
            tSignal.AddByKey(time, newDatas);
            this.OnDataChange(ea);

            this.PurgeSignal();

            this.PrevTime = time;

        }
        protected virtual void PurgeSignal()
        {
            throw new NotImplementedException();
        }
        protected virtual void PurgeSignalByCount(SNetDspTimeSignalSetSecond tSignal, int Count)
        {
            var query = tSignal.Signals.Take(tSignal.Signals.Count - Count).ToList();
            foreach (var ok in query)
                tSignal.Signals.Remove(ok.Key);
        }
        protected virtual void PurgeSignalByTime(SNetDspTimeSignalSetSecond tSignal, CtkTimeSecond time)
        {
            var now = DateTime.Now;
            var query = tSignal.Signals.Where(x => x.Key < time).ToList();
            foreach (var ok in query)
                tSignal.Signals.Remove(ok.Key);
        }
        protected virtual void PurgeSignalByTime(SNetDspTimeSignalSetMilliSecond tSignal, CtkTimeMilliSecond time)
        {
            var now = DateTime.Now;
            var query = tSignal.Signals.Where(x => x.Key < time).ToList();
            foreach (var ok in query)
                tSignal.Signals.Remove(ok.Key);
        }
        #region Event

        public event EventHandler<SNetDspBlockTimeSignalEventArg> evtDataChange;
        protected void OnDataChange(SNetDspBlockTimeSignalEventArg ea)
        {
            if (this.evtDataChange == null) return;
            this.evtDataChange(this, ea);
        }

        #endregion


        #region IDisposable
        // Flag: Has Dispose already been called?
        protected bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
                this.DisposeManaged();
            }

            // Free any unmanaged objects here.
            //
            this.DisposeUnmanaged();
            this.DisposeSelf();
            disposed = true;
        }



        protected virtual void DisposeManaged()
        {
        }
        protected virtual void DisposeSelf()
        {
            CtkEventUtil.RemoveEventHandlersFromOwningByFilter(this, (dlgt) => true);
        }

        protected virtual void DisposeUnmanaged()
        {

        }
        #endregion
    }
}
