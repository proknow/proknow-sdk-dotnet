using System.Collections.Generic;
using System.Threading.Tasks;
using ProKnow.Tools;

namespace ProKnow.Patient
{
    /// <summary>
    /// Provides a summary of a patient in a ProKnow workspace
    /// </summary>
    public class PatientSummary
    {
        /// <summary>
        /// The parent Patients object
        /// </summary>
        internal Patients Patients { get; set; }

        /// <summary>
        /// The patient ProKnow ID
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// The patient workspace ID
        /// </summary>
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The patient medical record number (MRN) or ID
        /// </summary>
        public string Mrn { get; internal set; }

        /// <summary>
        /// The patient name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The patient birth date
        /// </summary>
        public string BirthDate { get; internal set; }

        /// <summary>
        /// The patient sex
        /// </summary>
        public string Sex { get; internal set; }

        /// <summary>
        /// All patient summary attributes
        /// </summary>
        public Dictionary<string, object> Data { get; internal set; }

        /// <summary>
        /// Constructs a patient summary
        /// </summary>
        /// <param name="patients">The parent Patients object</param>
        /// <param name="workspaceId">ID of the workspace containing the patient</param>
        /// <param name="data">The patient summary attributes</param>
        internal PatientSummary(Patients patients, string workspaceId, Dictionary<string, object> data)
        {
            Patients = patients;
            Id = JsonTools.DeserializeString(data, "id");
            WorkspaceId = workspaceId;
            Mrn = JsonTools.DeserializeString(data, "mrn");
            Name = JsonTools.DeserializeString(data, "name");
            BirthDate = JsonTools.DeserializeString(data, "birth_date");
            Sex = JsonTools.DeserializeString(data, "sex");
            Data = data;
        }

        /// <summary>
        /// Asynchronously gets the corresponding patient item
        /// </summary>
        /// <returns>The corresponding patient item</returns>
        public Task<PatientItem> GetAsync()
        {
            return Patients.GetAsync(WorkspaceId, Id);
        }
    }
}
