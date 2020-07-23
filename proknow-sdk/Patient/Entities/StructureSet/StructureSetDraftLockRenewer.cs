using System;
using System.Text.Json;
using System.Threading;
using ProKnow.Exceptions;

namespace ProKnow.Patient.Entities.StructureSet
{
    /// <summary>
    /// Keeps a structure set lock from expiring
    /// </summary>
    public class StructureSetDraftLockRenewer
    {
        private readonly ProKnowApi _proKnow;
        private readonly StructureSetItem _structureSet;
        private Timer _timer;
        private bool _hasStarted;
        private TimeSpan _lockRenewalBuffer;

        /// <summary>
        /// Creates a StructureSetDraftLockRenewer
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="structureSet">The structure set</param>
        public StructureSetDraftLockRenewer(ProKnowApi proKnow, StructureSetItem structureSet)
        {
            _proKnow = proKnow;
            _structureSet = structureSet;
            _timer = null;
            _hasStarted = false;
            _lockRenewalBuffer = new TimeSpan(0, 0, proKnow.LockRenewalBuffer);
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            if (!_hasStarted)
            {
                TimeSpan expiresIn = new TimeSpan(0, 0, 0, 0, _structureSet.DraftLock.ExpiresIn);
                TimeSpan period;
                if (_lockRenewalBuffer < expiresIn)
                {
                    period = expiresIn - _lockRenewalBuffer;
                }
                else
                {
                    period = new TimeSpan(0);
                }
                _timer = new Timer(Run, null, new TimeSpan(0), period);
                _hasStarted = true;
            }
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            _timer.Dispose();
            _timer = null;
           _hasStarted = false;
        }

        /// <summary>
        /// The TimerCallback delegate that renews the draft lock
        /// </summary>
        private async void Run(Object notUsed)
        {
            if (_timer != null)
            {
                var json = await _proKnow.Requestor.PutAsync($"/workspaces/{_structureSet.WorkspaceId}/structuresets/{_structureSet.Id}/draft/lock/{_structureSet.DraftLock.Id}");
                _structureSet.DraftLock = JsonSerializer.Deserialize<StructureSetDraftLock>(json);
            }
        }
    }
}
