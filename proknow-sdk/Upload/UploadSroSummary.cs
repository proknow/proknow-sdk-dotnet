using System.Linq;
using System.Threading.Tasks;
using ProKnow.Patient;

namespace ProKnow.Upload
{
    /// <summary>
    /// A summary view of a spatial registration object in an upload response
    /// </summary>
    public class UploadSroSummary
    {
        /// <summary>
        /// The root object for interfacing with the ProKnow API
        /// </summary>
        protected ProKnowApi _proKnow;

        /// <summary>
        /// The workspace ID
        /// </summary>
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The patient ProKnow ID
        /// </summary>
        public string PatientId { get; internal set; }

        /// <summary>
        /// The study ProKnow ID
        /// </summary>
        public string StudyId { get; set; }

        /// <summary>
        /// The SRO ProKnow ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The entity UID
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Used by de-serialization to create an UploadSroSummary
        /// </summary>
        public UploadSroSummary()
        {
        }

        /// <summary>
        /// Creates a summary view of a spatial registration object in an upload response
        /// </summary>
        /// <param name="proKnow">The root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="patientId">The patient ProKnow ID</param>
        /// <param name="studyId">The study ProKnow ID</param>
        /// <param name="uploadStatusResult">The upload status result</param>
        internal UploadSroSummary(ProKnowApi proKnow, string workspaceId, string patientId, string studyId,
            UploadStatusResultSro uploadStatusResult)
        {
            _proKnow = proKnow;
            WorkspaceId = workspaceId;
            PatientId = patientId;
            StudyId = studyId;
            Id = uploadStatusResult.Id;
            Uid = uploadStatusResult.Uid;
        }

        /// <summary>
        /// Gets the complete representation of the spatial registration object
        /// </summary>
        /// <returns>A complete representation of the spatial registration object</returns>
        public async Task<SroItem> GetAsync()
        {
            var patientItem = await _proKnow.Patients.GetAsync(WorkspaceId, PatientId);
            var studySummary = patientItem.Studies.Where(s => s.Id == StudyId).First();
            var sroSummary = studySummary.Sros.Where(sro => sro.Id == Id).First();
            return await sroSummary.GetAsync();
        }
    }
}
