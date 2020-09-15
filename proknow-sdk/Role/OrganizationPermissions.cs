using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Role
{
    /// <summary>
    /// A container for the organization permissions for a role
    /// </summary>
    public class OrganizationPermissions
    {
        /// <summary>
        /// Flag indicating whether role allows creation of API keys
        /// </summary>
        [JsonPropertyName("create_api_keys")]
        public bool CanCreateApiKeys { get; set; }

        /// <summary>
        /// Flag indicating whether role allows managing users, roles, and workspaces
        /// </summary>
        [JsonPropertyName("manage_access")]
        public bool CanManageAccess { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creation, modification, and deletion of custom metrics
        /// </summary>
        [JsonPropertyName("manage_custom_metrics")]
        public bool CanManageCustomMetrics { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creation, modification, and deletion of scorecard templates
        /// </summary>
        [JsonPropertyName("manage_template_metric_sets")]
        public bool CanManageScorecardTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creation, modification, deletion, and execution of renaming rules
        /// </summary>
        [JsonPropertyName("manage_renaming_rules")]
        public bool CanManageRenamingRules { get; set; }

        /// <summary>
        /// Flag indicating whether role allows managing of checklist templates
        /// </summary>
        [JsonPropertyName("manage_template_checklists")]
        public bool CanManageChecklistTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether this is a collaborator role, i.e., that the other permissions only apply to
        /// patients across the organization for which a user has been explicitly granted access
        /// </summary>
        [JsonPropertyName("organization_collaborator")]
        public bool IsCollaborator { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading of patient data across the organization
        /// </summary>
        [JsonPropertyName("organization_read_patients")]
        public bool CanReadPatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading of collection data across the organization
        /// </summary>
        [JsonPropertyName("organization_read_collections")]
        public bool CanReadCollections { get; set; }

        /// <summary>
        /// Flag indicating whether role allows viewing of protected health information (PHI) across the organization
        /// </summary>
        [JsonPropertyName("organization_view_phi")]
        public bool CanViewPhi { get; set; }

        /// <summary>
        /// Flag indicating whether role allows downloading of DICOM files across the organization
        /// </summary>
        [JsonPropertyName("organization_download_dicom")]
        public bool CanDownloadDicom { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creation and modification of collection data across the organization
        /// </summary>
        [JsonPropertyName("organization_write_collections")]
        public bool CanWriteCollections { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creation and modification of patient data across the organization
        /// </summary>
        [JsonPropertyName("organization_write_patients")]
        public bool CanWritePatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creation and modification of patient contour data across the organization
        /// </summary>
        [JsonPropertyName("organization_contour_patients")]
        public bool CanContourPatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deletion of workspace collections across the organization
        /// </summary>
        [JsonPropertyName("organization_delete_collections")]
        public bool CanDeleteCollections { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deletion of patients and patient entities across the organization
        /// </summary>
        [JsonPropertyName("organization_delete_patients")]
        public bool CanDeletePatients { get; set; }

        /// <summary>
        /// The collection of workspace permissions for the role
        /// </summary>
        [JsonPropertyName("workspaces")]
        public IList<WorkspacePermissions> Workspaces { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Used by deserialization to create an OrganizationPermissions object
        /// </summary>
        public OrganizationPermissions()
        {
            Workspaces = new List<WorkspacePermissions>();
        }

        /// <summary>
        /// Constructs an OrganizationPermissions object
        /// </summary>
        /// <param name="canCreateApiKeys">Flag indicating whether role allows creation of API keys</param>
        /// <param name="canManageAccess">Flag indicating whether role allows managing users, roles, and workspaces</param>
        /// <param name="canManageCustomMetrics">Flag indicating whether role allows creation, modification, and deletion of custom metrics</param>
        /// <param name="canManageScorecardTemplates">Flag indicating whether role allows creation, modification, and deletion of scorecard templates</param>
        /// <param name="canManageRenamingRules">Flag indicating whether role allows creation, modification, deletion, and execution of renaming rules</param>
        /// <param name="canManageChecklistTemplates">Flag indicating whether role allows managing of checklist templates</param>
        /// <param name="isCollaborator">Flag indicating whether this is a collaborator role, i.e., that the other permissions only apply to
        /// patients across the organization for which a user has been explicitly granted access</param>
        /// <param name="canReadPatients">Flag indicating whether role allows reading of patient data across the organization</param>
        /// <param name="canReadCollections">Flag indicating whether role allows reading of collection data across the organization</param>
        /// <param name="canViewPhi">Flag indicating whether role allows viewing of protected health information (PHI) across the organization</param>
        /// <param name="canDownloadDicom">Flag indicating whether role allows downloading of DICOM files across the organization</param>
        /// <param name="canWriteCollections">Flag indicating whether role allows creation and modification of collection data across the organization</param>
        /// <param name="canWritePatients">Flag indicating whether role allows creation and modification of patient data across the organization</param>
        /// <param name="canContourPatients">Flag indicating whether role allows creation and modification of patient contour data across the organization</param>
        /// <param name="canDeleteCollections">Flag indicating whether role allows deletion of workspace collections across the organization</param>
        /// <param name="canDeletePatients">Flag indicating whether role allows deletion of patients and patient entities across the organization</param>
        /// <param name="workspaces">The collection of workspace permissions for the role</param>
        public OrganizationPermissions(bool canCreateApiKeys = false, bool canManageAccess = false,
            bool canManageCustomMetrics = false, bool canManageScorecardTemplates = false, bool canManageRenamingRules = false,
            bool canManageChecklistTemplates = false, bool isCollaborator = false, bool canReadPatients = false,
            bool canReadCollections = false, bool canViewPhi = false, bool canDownloadDicom = false,
            bool canWriteCollections = false, bool canWritePatients = false, bool canContourPatients = false,
            bool canDeleteCollections = false, bool canDeletePatients = false, IList<WorkspacePermissions> workspaces = null)
        {
            CanCreateApiKeys = canCreateApiKeys;
            CanManageAccess = canManageAccess;
            CanManageCustomMetrics = canManageCustomMetrics;
            CanManageScorecardTemplates = canManageScorecardTemplates;
            CanManageRenamingRules = canManageRenamingRules;
            CanManageChecklistTemplates = canManageChecklistTemplates;
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
            if (workspaces != null)
            {
                Workspaces = new List<WorkspacePermissions>(workspaces);
            }
            else
            {
                Workspaces = new List<WorkspacePermissions>();
            }
        }
    }
}
