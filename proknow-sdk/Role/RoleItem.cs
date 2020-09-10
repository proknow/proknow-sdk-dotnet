using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Role
{
    /// <summary>
    /// Represents a role for a ProKnow organization
    /// </summary>
    public class RoleItem : OrganizationPermissions
    {
        /// <summary>
        /// The ProKnow ID of the role
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the role
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows creation of API keys
        ///// </summary>
        //[JsonPropertyName("create_api_keys")]
        //public bool CanCreateApiKeys { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows managing users, roles, and workspaces
        ///// </summary>
        //[JsonPropertyName("manage_access")]
        //public bool CanManageAccess { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows creation, modification, and deletion of custom metrics
        ///// </summary>
        //[JsonPropertyName("manage_custom_metrics")]
        //public bool CanManageCustomMetrics { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows creation, modification, and deletion of scorecard templates
        ///// </summary>
        //[JsonPropertyName("manage_template_metric_sets")]
        //public bool CanManageScorecardTemplates { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows creation, modification, deletion, and execution of renaming rules
        ///// </summary>
        //[JsonPropertyName("manage_renaming_rules")]
        //public bool CanManageRenamingRules { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows managing of checklist templates
        ///// </summary>
        //[JsonPropertyName("manage_template_checklists")]
        //public bool CanManageChecklistTemplates { get; set; }

        ///// <summary>
        ///// Flag indicating whether this is a collaborator role, i.e., that the other permissions only apply to
        ///// patients across the organization for which a user has been explicitly granted access
        ///// </summary>
        //[JsonPropertyName("organization_collaborator")]
        //public bool IsCollaborator { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows reading of patient data across the organization
        ///// </summary>
        //[JsonPropertyName("organization_read_patients")]
        //public bool CanReadPatients { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows reading of collection data across the organization
        ///// </summary>
        //[JsonPropertyName("organization_read_collections")]
        //public bool CanReadCollections { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows viewing of protected health information (PHI) across the organization
        ///// </summary>
        //[JsonPropertyName("organization_view_phi")]
        //public bool CanViewPhi { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows downloading of DICOM files across the organization
        ///// </summary>
        //[JsonPropertyName("organization_download_dicom")]
        //public bool CanDownloadDicom { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows creation and modification of collection data across the organization
        ///// </summary>
        //[JsonPropertyName("organization_write_collections")]
        //public bool CanWriteCollections { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows creation and modification of patient data across the organization
        ///// </summary>
        //[JsonPropertyName("organization_write_patients")]
        //public bool CanWritePatients { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows creation and modification of patient contour data across the organization
        ///// </summary>
        //[JsonPropertyName("organization_contour_patients")]
        //public bool CanContourPatients { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows deletion of workspace collections across the organization
        ///// </summary>
        //[JsonPropertyName("organization_delete_collections")]
        //public bool CanDeleteCollections { get; set; }

        ///// <summary>
        ///// Flag indicating whether role allows deletion of patients and patient entities across the organization
        ///// </summary>
        //[JsonPropertyName("organization_delete_patients")]
        //public bool CanDeletePatients { get; set; }

        ///// <summary>
        ///// The collection of workspace permissions for the role
        ///// </summary>
        //[JsonPropertyName("workspaces")]
        //public IList<WorkspacePermissions> Workspaces { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
