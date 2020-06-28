using System;
using System.Globalization;
using System.Text.Json;
using System.Threading;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Keeps a structure set lock from expiring
    /// </summary>
    public class StructureSetDraftLockRenewer
    {
        private ProKnowApi _proKnow;
        private StructureSetItem _structureSet;
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
                //DateTime expiresAt = DateTime.ParseExact(_structureSet.DraftLock.ExpiresAt, "yyyy-MM-dd HH:mm:ss,fff",
                //    CultureInfo.InvariantCulture);
                TimeSpan expiresIn = new TimeSpan(0, 0, 0, 0, _structureSet.DraftLock.ExpiresIn);
                //TimeSpan period = (expiresAt - DateTime.UtcNow) - _lockRenewalBuffer;
                TimeSpan period;
                if (_lockRenewalBuffer < expiresIn)
                {
                    period = expiresIn - _lockRenewalBuffer;
                }
                else
                {
                    period = new TimeSpan(0);
                }
                _timer = new Timer(RunAsync, null, new TimeSpan(0), period);
                _hasStarted = true;
            }
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            _timer.Dispose();
            _hasStarted = false;
        }

        /// <summary>
        /// The TimerCallback delegate that renews the draft lock
        /// </summary>
        private async void RunAsync(Object notUsed)
        {
            var json = await _proKnow.Requestor.PutAsync($"/workspaces/{_structureSet.WorkspaceId}/structuresets/{_structureSet.Id}/draft/lock/{_structureSet.DraftLock.Id}");
            _structureSet.DraftLock = JsonSerializer.Deserialize<StructureSetDraftLock>(json);
        }
    }
}
