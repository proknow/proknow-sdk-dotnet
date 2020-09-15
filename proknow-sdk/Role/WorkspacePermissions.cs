using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Role
{
    /// <summary>
    /// A container for workspace permissions for a role
    /// </summary>
    public class WorkspacePermissions
    {
        /// <summary>
        /// The ProKnow ID of the workspace
        /// </summary>
        [JsonPropertyName("id")]
        public string WorkspaceId { get; set; }

        /// <summary>
        /// Flag indicating whether this is a collaborator role, i.e., that the other permissions only apply to
        /// patients across the workspace for which a user has been explicitly granted access
        /// </summary>
        [JsonPropertyName("collaborator")]
        public bool IsCollaborator { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading of patient data across the workspace
        /// </summary>
        [JsonPropertyName("read_patients")]
        public bool CanReadPatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading of collection data across the workspace
        /// </summary>
        [JsonPropertyName("read_collections")]
        public bool CanReadCollections { get; set; }

        /// <summary>
        /// Flag indicating whether role allows viewing of protected health information (PHI) across the workspace
        /// </summary>
        [JsonPropertyName("view_phi")]
        public bool CanViewPhi { get; set; }

        /// <summary>
        /// Flag indicating whether role allows downloading of DICOM files across the workspace
        /// </summary>
        [JsonPropertyName("download_dicom")]
        public bool CanDownloadDicom { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creation and modification of collection data across the workspace
        /// </summary>
        [JsonPropertyName("write_collections")]
        public bool CanWriteCollections { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creation and modification of patient data across the workspace
        /// </summary>
        [JsonPropertyName("write_patients")]
        public bool CanWritePatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creation and modification of patient contour data across the workspace
        /// </summary>
        [JsonPropertyName("contour_patients")]
        public bool CanContourPatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deletion of workspace collections across the workspace
        /// </summary>
        [JsonPropertyName("delete_collections")]
        public bool CanDeleteCollections { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deletion of patients and patient entities across the workspace
        /// </summary>
        [JsonPropertyName("delete_patients")]
        public bool CanDeletePatients { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Used by deserialization to create a WorkspacePermissions object
        /// </summary>
        public WorkspacePermissions()
        {
        }

        /// <summary>
        /// Constructs a WorkspacePermissions object
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID of the workspace</param>
        /// <param name="isCollaborator">Flag indicating whether this is a collaborator role, i.e., that the other permissions only apply to
        /// patients across the workspace for which a user has been explicitly granted access</param>
        /// <param name="canReadPatients">Flag indicating whether role allows reading of patient data across the workspace</param>
        /// <param name="canReadCollections">Flag indicating whether role allows reading of collection data across the workspace</param>
        /// <param name="canViewPhi">Flag indicating whether role allows viewing of protected health information (PHI) across the workspace</param>
        /// <param name="canDownloadDicom">Flag indicating whether role allows downloading of DICOM files across the workspace</param>
        /// <param name="canWriteCollections">Flag indicating whether role allows creation and modification of collection data across the workspace</param>
        /// <param name="canWritePatients">Flag indicating whether role allows creation and modification of patient data across the workspace</param>
        /// <param name="canContourPatients">Flag indicating whether role allows creation and modification of patient contour data across the workspace</param>
        /// <param name="canDeleteCollections">Flag indicating whether role allows deletion of workspace collections across the workspace</param>
        /// <param name="canDeletePatients">Flag indicating whether role allows deletion of patients and patient entities across the workspace</param>
        public WorkspacePermissions(string workspaceId, bool isCollaborator = false, bool canReadPatients = false,
            bool canReadCollections = false, bool canViewPhi = false, bool canDownloadDicom = false,
            bool canWriteCollections = false, bool canWritePatients = false, bool canContourPatients = false,
            bool canDeleteCollections = false, bool canDeletePatients = false)
        {
            WorkspaceId = workspaceId;
            IsCollaborator = isCollaborator;
            CanReadPatients = canReadPatients;
            CanReadCollections = canReadCollections;
            CanViewPhi = canViewPhi;
            CanDownloadDicom = canDownloadDicom;
            CanWriteCollections = canWriteCollections;
            CanWritePatients = canWritePatients;
            CanContourPatients = canContourPatients;
            CanDeleteCollections = canDeleteCollections;
            CanDeletePatients = canDeletePatients;
        }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return WorkspaceId;
        }
    }
}
