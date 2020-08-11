using ProKnow.Exceptions;
using System;
using System.Text.Json;
using System.Threading;

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
        private readonly TimeSpan _timerDisposalTimeout;

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
            _timerDisposalTimeout = TimeSpan.FromSeconds(10);
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
            // Wait for the timer to be disposed so that all Run callbacks, which are queued on another thread, have completed
            using (var waitHandle = new ManualResetEvent(false))
            {
                // If this is the first time we've tried to dispose the timer, wait a few seconds for it to be disposed (it must wait for queued Run callbacks to finish)
                if (_timer.Dispose(waitHandle) && !waitHandle.WaitOne(_timerDisposalTimeout))
                {
                    throw new TimeoutException("Timeout waiting for structure set draft lock renewer timer to stop.");
                }
            }
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
                try
                {
                    var json = await _proKnow.Requestor.PutAsync($"/workspaces/{_structureSet.WorkspaceId}/structuresets/{_structureSet.Id}/draft/lock/{_structureSet.DraftLock.Id}");
                    _structureSet.DraftLock = JsonSerializer.Deserialize<StructureSetDraftLock>(json);
                }
                catch (ProKnowException ex)
                {
                    var workspace = await _proKnow.Workspaces.ResolveByIdAsync(_structureSet.WorkspaceId);
                    var patientSummary = await _proKnow.Patients.FindAsync(_structureSet.WorkspaceId, p => p.Id == _structureSet.PatientId);
                    throw new ProKnowException($"Error renewing draft lock for workspace '{workspace.Name}' patient '{patientSummary.Mrn}'.  Inner exception:  {ex.Message}.");
                }
            }
        }
    }
}
