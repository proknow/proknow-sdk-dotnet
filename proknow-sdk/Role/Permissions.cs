using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Role
{ 
    /// <summary>
    /// A container for the permissions for a role
    /// </summary>
    public class Permissions
    {
        /// <summary>
        /// Flag indicating whether role allows an orgainzation update
        /// </summary>
        [JsonPropertyName("organizations_update")]
        public bool CanUpdateOrganizations { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creation of API keys
        /// </summary>
        [JsonPropertyName("api_keys_create")]
        public bool CanCreateApiKeys { get; set; }

        /// <summary>
        /// Flag indicating whether role allows managing audit logs
        /// </summary>
        [JsonPropertyName("audit_logs_manage")]
        public bool CanManageAuditLogs { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating custom metrics
        /// </summary>
        [JsonPropertyName("custom_metrics_create")]
        public bool CanCreateCustomMetrics { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading custom metrics
        /// </summary>
        [JsonPropertyName("custom_metrics_read")]
        public bool CanReadCustomMetrics { get { return true; }  }

        /// <summary>
        /// Flag indicating whether role allows updating custom metrics
        /// </summary>
        [JsonPropertyName("custom_metrics_update")]
        public bool CanUpdateCustomMetrics { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting custom metrics
        /// </summary>
        [JsonPropertyName("custom_metrics_delete")]
        public bool CanDeleteCustomMetrics { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading renaming rules
        /// </summary>
        [JsonPropertyName("renaming_rules_read")]
        public bool CanReadRenamingRules { get { return true; } }

        /// <summary>
        /// Flag indicating whether role allows updating renaming rules
        /// </summary>
        [JsonPropertyName("renaming_rules_update")]
        public bool CanUpdateRenamingRules { get; set; }

        /// <summary>
        /// Flag indicating whether role allows searching renaming rules
        /// </summary>
        [JsonPropertyName("renaming_rules_search")]
        public bool CanSearchRenamingRules { get; set; }

        /// <summary>
        /// Flag indicating whether role allows executing renaming rules
        /// </summary>
        [JsonPropertyName("renaming_rules_execute")]
        public bool CanExecuteRenamingRules { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating checklist templates
        /// </summary>
        [JsonPropertyName("checklist_templates_create")]
        public bool CanCreateChecklistTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading checklist templates
        /// </summary>
        [JsonPropertyName("checklist_templates_read")]
        public bool CanReadChecklistTemplates { get { return true; } }

        /// <summary>
        /// Flag indicating whether role allows updating checklist templates
        /// </summary>
        [JsonPropertyName("checklist_templates_update")]
        public bool CanUpdateChecklistTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting checklist templates
        /// </summary>
        [JsonPropertyName("checklist_templates_delete")]
        public bool CanDeleteChecklistTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating structure set templates
        /// </summary>
        [JsonPropertyName("structure_set_templates_create")]
        public bool CanCreateStructureSetTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading structure set templates
        /// </summary>
        [JsonPropertyName("structure_set_templates_read")]
        public bool CanReadStructureSetTemplates { get { return true; } }

        /// <summary>
        /// Flag indicating whether role allows updating structure set templates
        /// </summary>
        [JsonPropertyName("structure_set_templates_update")]
        public bool CanUpdateStructureSetTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting structure set templates
        /// </summary>
        [JsonPropertyName("structure_set_templates_delete")]
        public bool CanDeleteStructureSetTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating scorecard templates
        /// </summary>
        [JsonPropertyName("scorecard_templates_create")]
        public bool CanCreateScorecardTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading scorecard templates
        /// </summary>
        [JsonPropertyName("scorecard_templates_read")]
        public bool CanReadScorecardTemplates { get { return true; } }

        /// <summary>
        /// Flag indicating whether role allows updating scorecard templates
        /// </summary>
        [JsonPropertyName("scorecard_templates_update")]
        public bool CanUpdateScorecardTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting scorecard templates
        /// </summary>
        [JsonPropertyName("scorecard_templates_delete")]
        public bool CanDeleteScorecardTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating objective templates
        /// </summary>
        [JsonPropertyName("objective_templates_create")]
        public bool CanCreateObjectiveTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading objective templates
        /// </summary>
        [JsonPropertyName("objective_templates_read")]
        public bool CanReadObjectiveTemplates { get { return true; } }

        /// <summary>
        /// Flag indicating whether role allows deleting objective templates
        /// </summary>
        [JsonPropertyName("objective_templates_delete")]
        public bool CanDeleteObjectiveTemplates { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating workspaces
        /// </summary>
        [JsonPropertyName("workspaces_create")]
        public bool CanCreateWorkspaces { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading workspaces
        /// </summary>
        [JsonPropertyName("workspaces_read")]
        public bool CanReadWorkspaces { get; set; }

        /// <summary>
        /// Flag indicating whether role allows updating workspaces
        /// </summary>
        [JsonPropertyName("workspaces_update")]
        public bool CanUpdateWorkspaces { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting workspaces
        /// </summary>
        [JsonPropertyName("workspaces_delete")]
        public bool CanDeleteWorkspaces { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating groups
        /// </summary>
        [JsonPropertyName("groups_create")]
        public bool CanCreateGroups { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading groups
        /// </summary>
        [JsonPropertyName("groups_read")]
        public bool CanReadGroups { get { return true; } }

        /// <summary>
        /// Flag indicating whether role allows updating groups
        /// </summary>
        [JsonPropertyName("groups_update")]
        public bool CanUpdateGroups { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting groups
        /// </summary>
        [JsonPropertyName("groups_delete")]
        public bool CanDeleteGroups { get; set; }

        /// <summary>
        /// Flag indicating whether role allows adding group members
        /// </summary>
        [JsonPropertyName("group_members_add")]
        public bool CanAddGroupMembers { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading group members
        /// </summary>
        [JsonPropertyName("group_members_list")]
        public bool CanListGroupMembers { get { return true; } }

        /// <summary>
        /// Flag indicating whether role allows removing group members
        /// </summary>
        [JsonPropertyName("group_members_remove")]
        public bool CanRemoveGroupMembers { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating roles
        /// </summary>
        [JsonPropertyName("roles_create")]
        public bool CanCreateRoles { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading roles
        /// </summary>
        [JsonPropertyName("roles_read")]
        public bool CanReadRoles { get { return true; } }

        /// <summary>
        /// Flag indicating whether role allows updating roles
        /// </summary>
        [JsonPropertyName("roles_update")]
        public bool CanUpdateRoles { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting roles
        /// </summary>
        [JsonPropertyName("roles_delete")]
        public bool CanDeleteRoles { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating users
        /// </summary>
        [JsonPropertyName("users_create")]
        public bool CanCreateUsers { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading users
        /// </summary>
        [JsonPropertyName("users_read")]
        public bool CanReadUsers { get { return true; } }

        /// <summary>
        /// Flag indicating whether role allows updating users
        /// </summary>
        [JsonPropertyName("users_update")]
        public bool CanUpdateUsers { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting users
        /// </summary>
        [JsonPropertyName("users_delete")]
        public bool CanDeleteUsers { get; set; }

        /// <summary>
        /// Flag indicating whether role allows adding resource assignments
        /// </summary>
        [JsonPropertyName("resource_assignments_add")]
        public bool CanAddResourceAssignments { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading resource assignments
        /// </summary>
        [JsonPropertyName("resource_assignments_list")]
        public bool CanListResourceAssignments { get; set; }

        /// <summary>
        /// Flag indicating whether role allows removing resource assignments
        /// </summary>
        [JsonPropertyName("resource_assignments_remove")]
        public bool CanRemoveResourceAssignments { get; set; }

        /// <summary>
        /// Flag indicating whether role allows resolving resource permissions
        /// </summary>
        [JsonPropertyName("resource_permissions_resolve")]
        public bool CanResolveResourcePermissions { get { return true; } }

        /// <summary>
        /// Flag indicating whether role allows creating patients
        /// </summary>
        [JsonPropertyName("patients_create")]
        public bool CanCreatePatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading patients
        /// </summary>
        [JsonPropertyName("patients_read")]
        public bool CanReadPatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows updating patients
        /// </summary>
        [JsonPropertyName("patients_update")]
        public bool CanUpdatePatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting patients
        /// </summary>
        [JsonPropertyName("patients_delete")]
        public bool CanDeletePatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows copying patients
        /// </summary>
        [JsonPropertyName("patients_copy")]
        public bool CanCopyPatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows moving patients
        /// </summary>
        [JsonPropertyName("patients_move")]
        public bool CanMovePatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows viewing patients PHI
        /// </summary>
        [JsonPropertyName("patients_phi")]
        public bool CanViewPatientsPHI { get; set; }

        /// <summary>
        /// Flag indicating whether role allows contouring patients
        /// </summary>
        [JsonPropertyName("patients_contour")]
        public bool CanContourPatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows uploading patient DICOM
        /// </summary>
        [JsonPropertyName("patient_dicom_upload")]
        public bool CanUploadPatientDICOM { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating patient checklists
        /// </summary>
        [JsonPropertyName("patient_checklists_create")]
        public bool CanCreatePatientChecklists { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading patient checklists
        /// </summary>
        [JsonPropertyName("patient_checklists_read")]
        public bool CanReadPatientChecklists { get; set; }

        /// <summary>
        /// Flag indicating whether role allows updating patient checklists
        /// </summary>
        [JsonPropertyName("patient_checklists_update")]
        public bool CanUpdatePatientChecklists { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting patient checklists
        /// </summary>
        [JsonPropertyName("patient_checklists_delete")]
        public bool CanDeletePatientChecklists { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating patient scorecards
        /// </summary>
        [JsonPropertyName("patient_scorecards_create")]
        public bool CanCreatePatientScorecards { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading patient scorecards
        /// </summary>
        [JsonPropertyName("patient_scorecards_read")]
        public bool CanReadPatientScorecards { get; set; }

        /// <summary>
        /// Flag indicating whether role allows updating patient scorecards
        /// </summary>
        [JsonPropertyName("patient_scorecards_update")]
        public bool CanUpdatePatientScorecards { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting patient scorecards
        /// </summary>
        [JsonPropertyName("patient_scorecards_delete")]
        public bool CanDeletePatientScorecards { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating patient documents
        /// </summary>
        [JsonPropertyName("patient_documents_create")]
        public bool CanCreatePatientDocuments { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading patient documents
        /// </summary>
        [JsonPropertyName("patient_documents_read")]
        public bool CanReadPatientDocuments { get; set; }

        /// <summary>
        /// Flag indicating whether role allows updating patient documents
        /// </summary>
        [JsonPropertyName("patient_documents_update")]
        public bool CanUpdatePatientDocuments { get; set; }


        /// <summary>
        /// Flag indicating whether role allows deleting patient documents
        /// </summary>
        [JsonPropertyName("patient_documents_delete")]
        public bool CanDeletePatientDocuments { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating patient notes
        /// </summary>
        [JsonPropertyName("patient_notes_create")]
        public bool CanCreatePatientNotes { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading patient notes
        /// </summary>
        [JsonPropertyName("patient_notes_read")]
        public bool CanReadPatientNotes { get; set; }

        /// <summary>
        /// Flag indicating whether role allows updating patient notes
        /// </summary>
        [JsonPropertyName("patient_notes_update")]
        public bool CanUpdatePatientNotes { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting patient notes
        /// </summary>
        [JsonPropertyName("patient_notes_delete")]
        public bool CanDeletePatientNotes { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating collections
        /// </summary>
        [JsonPropertyName("collections_create")]
        public bool CanCreateCollections { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading collections
        /// </summary>
        [JsonPropertyName("collections_read")]
        public bool CanReadCollections { get; set; }

        /// <summary>
        /// Flag indicating whether role allows updating collections
        /// </summary>
        [JsonPropertyName("collections_update")]
        public bool CanUpdateCollections { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting collections
        /// </summary>
        [JsonPropertyName("collections_delete")]
        public bool CanDeleteCollections { get; set; }

        /// <summary>
        /// Flag indicating whether role allows adding collection patients
        /// </summary>
        [JsonPropertyName("collection_patients_add")]
        public bool CanAddCollectionPatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows removing collection patients
        /// </summary>
        [JsonPropertyName("collection_patients_remove")]
        public bool CanRemoveCollectionPatients { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating collection scorecards
        /// </summary>
        [JsonPropertyName("collection_scorecards_create")]
        public bool CanCreateCollectionScorecards { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading collection scorecards
        /// </summary>
        [JsonPropertyName("collection_scorecards_read")]
        public bool CanReadCollectionScorecards { get; set; }

        /// <summary>
        /// Flag indicating whether role allows updating collection scorecards
        /// </summary>
        [JsonPropertyName("collection_scorecards_update")]
        public bool CanUpdateCollectionScorecards { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting collection scorecards
        /// </summary>
        [JsonPropertyName("collection_scorecards_delete")]
        public bool CanDeleteCollectionScorecards { get; set; }

        /// <summary>
        /// Flag indicating whether role allows creating collection bookmarks
        /// </summary>
        [JsonPropertyName("collection_bookmarks_create")]
        public bool CanCreateCollectionBookmarks { get; set; }

        /// <summary>
        /// Flag indicating whether role allows reading collection bookmarks
        /// </summary>
        [JsonPropertyName("collection_bookmarks_read")]
        public bool CanReadCollectionBookmarks { get; set; }

        /// <summary>
        /// Flag indicating whether role allows updating collection bookmarks
        /// </summary>
        [JsonPropertyName("collection_bookmarks_update")]
        public bool CanUpdateCollectionBookmarks { get; set; }

        /// <summary>
        /// Flag indicating whether role allows deleting collection bookmarks
        /// </summary>
        [JsonPropertyName("collection_bookmarks_delete")]
        public bool CanDeleteCollectionBookmarks { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Used by deserialization to create a Permissions object
        /// </summary>
        public Permissions()
        {
        }

        /// <summary>
        /// Constructs a Permissions object
        /// </summary>
        /// <param name="canUpdateOrganizations">Flag indicating whether role allows updating the orgainization</param>
        /// <param name="canCreateApiKeys">Flag indicating whether role allows creation of API keys</param>
        /// <param name="canManageAuditLogs">Flag indicating whether role allows managing audit logs</param>
        /// <param name="canCreateCustomMetrics">Flag indicating whether role allows creating custom metrics</param>
        /// <param name="canUpdateCustomMetrics">Flag indicating whether role allows updating custom metrics</param>
        /// <param name="canDeleteCustomMetrics">Flag indicating whether role allows deleting custom metrics</param>
        /// <param name="canUpdateRenamingRules">Flag indicating whether role allows updating renaming rules</param>
        /// <param name="canSearchRenamingRules">Flag indicating whether role allows searching renaming rules</param>
        /// <param name="canExecuteRenamingRules">Flag indicating whether role allows executing renaming rules</param>
        /// <param name="canCreateChecklistTemplates">Flag indicating whether role allows creating checklist templates</param>
        /// <param name="canUpdateChecklistTemplates">Flag indicating whether role allows updating checklist templates</param>
        /// <param name="canDeleteChecklistTemplates">Flag indicating whether role allows deleting checklist templates</param>
        /// <param name="canCreateStructureSetTemplates">Flag indicating whether role allows creating structure set templates</param>
        /// <param name="canUpdateStructureSetTemplates">Flag indicating whether role allows updating structure set templates</param>
        /// <param name="canDeleteStructureSetTemplates">Flag indicating whether role allows deleting structure set templates</param>
        /// <param name="canCreateScorecardTemplates">Flag indicating whether role allows creating scorecard templates</param>
        /// <param name="canUpdateScorecardTemplates">Flag indicating whether role allows updating scorecard templates</param>
        /// <param name="canDeleteScorecardTemplates">Flag indicating whether role allows deleting scorecard templates</param>
        /// <param name="canCreateObjectiveTemplates">Flag indicating whether role allows creating objective templates</param>
        /// <param name="canDeleteObjectiveTemplates">Flag indicating whether role allows deleting objective templates</param>
        /// <param name="canCreateWorkspaces">Flag indicating whether role allows creating workspaces</param>
        /// <param name="canReadWorkspaces">Flag indicating whether role allows reading workspaces</param>
        /// <param name="canUpdateWorkspaces">Flag indicating whether role allows updating workspaces</param>
        /// <param name="canDeleteWorkspaces">Flag indicating whether role allows deleting workspaces</param>
        /// <param name="canCreateGroups">Flag indicating whether role allows creating groups</param>
        /// <param name="canUpdateGroups">Flag indicating whether role allows updating groups</param>
        /// <param name="canDeleteGroups">Flag indicating whether role allows deleting groups</param>
        /// <param name="canAddGroupMembers">Flag indicating whether role allows adding group members</param>
        /// <param name="canRemoveGroupMembers">Flag indicating whether role allows removing group members</param>
        /// <param name="canCreateRoles">Flag indicating whether role allows creating roles</param>
        /// <param name="canUpdateRoles">Flag indicating whether role allows updating roles</param>
        /// <param name="canDeleteRoles">Flag indicating whether role allows deleting roles</param>
        /// <param name="canCreateUsers">Flag indicating whether role allows creating users</param>
        /// <param name="canUpdateUsers">Flag indicating whether role allows updating users</param>
        /// <param name="canDeleteUsers">Flag indicating whether role allows deleting users</param>
        /// <param name="canAddResourceAssignments">Flag indicating whether role allows adding resource assignments</param>
        /// <param name="canListResourceAssignments">Flag indicating whether role allows reading resource assignments</param>
        /// <param name="canRemoveResourceAssignments">Flag indicating whether role allows removing resource assignments</param>
        /// <param name="canCreatePatients">Flag indicating whether role allows creating patients</param>
        /// <param name="canReadPatients">Flag indicating whether role allows reading patients</param>
        /// <param name="canUpdatePatients">Flag indicating whether role allows updating patients</param>
        /// <param name="canDeletePatients">Flag indicating whether role allows deleting patients</param>
        /// <param name="canCopyPatients">Flag indicating whether role allows copying patients</param>
        /// <param name="canMovePatients">Flag indicating whether role allows moving patients</param>
        /// <param name="canViewPatientsPHI">Flag indicating whether role allows viewing patients PHI</param>
        /// <param name="canContourPatients">Flag indicating whether role allows contouring patients</param>
        /// <param name="canUploadPatientDICOM">Flag indicating whether role allows uploading patient DICOM</param>
        /// <param name="canCreatePatientChecklists">Flag indicating whether role allows creating patient checklists</param>
        /// <param name="canReadPatientChecklists">Flag indicating whether role allows reading patient checklists</param>
        /// <param name="canUpdatePatientChecklists">Flag indicating whether role allows updating patient checklists</param>
        /// <param name="canDeletePatientChecklists">Flag indicating whether role allows deleting patient checklists</param>
        /// <param name="canCreatePatientScorecards">Flag indicating whether role allows creating patient scorecards</param>
        /// <param name="canReadPatientScorecards">Flag indicating whether role allows reading patient scorecards</param>
        /// <param name="canUpdatePatientScorecards">Flag indicating whether role allows updating patient scorecards</param>
        /// <param name="canDeletePatientScorecards">Flag indicating whether role allows deleting patient scorecards</param>
        /// <param name="canCreatePatientDocuments">Flag indicating whether role allows creating patient documents</param>
        /// <param name="canReadPatientDocuments">Flag indicating whether role allows reading patient documents</param>
        /// <param name="canUpdatePatientDocuments">Flag indicating whether role allows updating patient documents</param>
        /// <param name="canDeletePatientDocuments">Flag indicating whether role allows deleting patient documents</param>
        /// <param name="canCreatePatientNotes">Flag indicating whether role allows creating patient notes</param>
        /// <param name="canReadPatientNotes">Flag indicating whether role allows reading patient notes</param>
        /// <param name="canUpdatePatientNotes">Flag indicating whether role allows updating patient notes</param>
        /// <param name="canDeletePatientNotes">Flag indicating whether role allows deleting patient notes</param>
        /// <param name="canCreateCollections">Flag indicating whether role allows creating collections</param>
        /// <param name="canReadCollections">Flag indicating whether role allows reading collections</param>
        /// <param name="canUpdateCollections">Flag indicating whether role allows updating collections</param>
        /// <param name="canDeleteCollections">Flag indicating whether role allows deleting collections</param>
        /// <param name="canAddCollectionPatients">Flag indicating whether role allows adding collection patients</param>
        /// <param name="canRemoveCollectionPatients">Flag indicating whether role allows removing collection patients</param>
        /// <param name="canCreateCollectionScorecards">Flag indicating whether role allows creating collection scorecards</param>
        /// <param name="canReadCollectionScorecards">Flag indicating whether role allows reading collection scorecards</param>
        /// <param name="canUpdateCollectionScorecards">Flag indicating whether role allows updating collection scorecards</param>
        /// <param name="canDeleteCollectionScorecards">Flag indicating whether role allows deleting collection scorecards</param>
        /// <param name="canCreateCollectionBookmarks">Flag indicating whether role allows creating collection bookmarks</param>
        /// <param name="canReadCollectionBookmarks">Flag indicating whether role allows reading collection bookmarks</param>
        /// <param name="canUpdateCollectionBookmarks">Flag indicating whether role allows updating collection bookmarks</param>
        /// <param name="canDeleteCollectionBookmarks">Flag indicating whether role allows deleting collection bookmarks</param>
        public Permissions(
            bool canUpdateOrganizations = false, bool canCreateApiKeys = false, bool canManageAuditLogs = false, 
            bool canCreateCustomMetrics = false, bool canUpdateCustomMetrics = false, bool canDeleteCustomMetrics = false, 
            bool canUpdateRenamingRules = false, bool canSearchRenamingRules = false, bool canExecuteRenamingRules = false, 
            bool canCreateChecklistTemplates = false, bool canUpdateChecklistTemplates = false, bool canDeleteChecklistTemplates = false, 
            bool canCreateStructureSetTemplates = false, bool canUpdateStructureSetTemplates = false, bool canDeleteStructureSetTemplates = false,
            bool canCreateScorecardTemplates = false, bool canUpdateScorecardTemplates = false, bool canDeleteScorecardTemplates = false,
            bool canCreateObjectiveTemplates = false, bool canDeleteObjectiveTemplates = false, bool canCreateWorkspaces = false,
            bool canReadWorkspaces = false, bool canUpdateWorkspaces = false, bool canDeleteWorkspaces = false,
            bool canCreateGroups = false, bool canUpdateGroups = false,
            bool canDeleteGroups = false, bool canAddGroupMembers = false, bool canRemoveGroupMembers = false,
            bool canCreateRoles = false, bool canUpdateRoles = false, bool canDeleteRoles = false, bool canCreateUsers = false,
            bool canUpdateUsers = false, bool canDeleteUsers = false, bool canAddResourceAssignments = false,
            bool canListResourceAssignments = false, bool canRemoveResourceAssignments = false,
            bool canCreatePatients = false, bool canReadPatients = false, bool canUpdatePatients = false,
            bool canDeletePatients = false, bool canCopyPatients = false, bool canMovePatients = false,
            bool canViewPatientsPHI = false, bool canContourPatients = false, bool canUploadPatientDICOM = false,
            bool canCreatePatientChecklists = false, bool canReadPatientChecklists = false,
            bool canUpdatePatientChecklists = false, bool canDeletePatientChecklists = false, bool canCreatePatientScorecards = false,
            bool canReadPatientScorecards = false, bool canUpdatePatientScorecards = false, bool canDeletePatientScorecards = false,
            bool canCreatePatientDocuments = false, bool canReadPatientDocuments = false, bool canUpdatePatientDocuments = false,
            bool canDeletePatientDocuments = false, bool canCreatePatientNotes = false, bool canReadPatientNotes = false,
            bool canUpdatePatientNotes = false, bool canDeletePatientNotes = false, bool canCreateCollections = false,
            bool canReadCollections = false, bool canUpdateCollections = false, bool canDeleteCollections = false, bool canAddCollectionPatients = false,
            bool canRemoveCollectionPatients = false, bool canCreateCollectionScorecards = false, bool canReadCollectionScorecards = false,
            bool canUpdateCollectionScorecards = false, bool canDeleteCollectionScorecards = false, bool canCreateCollectionBookmarks = false,
            bool canReadCollectionBookmarks = false, bool canUpdateCollectionBookmarks = false, bool canDeleteCollectionBookmarks = false)
        {
            CanUpdateOrganizations = canUpdateOrganizations;
            CanCreateApiKeys = canCreateApiKeys;
            CanManageAuditLogs = canManageAuditLogs;
            CanCreateCustomMetrics = canCreateCustomMetrics;
            CanUpdateCustomMetrics = canUpdateCustomMetrics;
            CanDeleteCustomMetrics = canDeleteCustomMetrics;
            CanUpdateRenamingRules = canUpdateRenamingRules;
            CanSearchRenamingRules = canSearchRenamingRules;
            CanExecuteRenamingRules = canExecuteRenamingRules;
            CanCreateChecklistTemplates = canCreateChecklistTemplates;
            CanUpdateChecklistTemplates = canUpdateChecklistTemplates;
            CanDeleteChecklistTemplates = canDeleteChecklistTemplates;
            CanCreateStructureSetTemplates = canCreateStructureSetTemplates;
            CanUpdateStructureSetTemplates = canUpdateStructureSetTemplates;
            CanDeleteStructureSetTemplates = canDeleteStructureSetTemplates;
            CanCreateScorecardTemplates = canCreateScorecardTemplates;
            CanUpdateScorecardTemplates = canUpdateScorecardTemplates;
            CanDeleteScorecardTemplates = canDeleteScorecardTemplates;
            CanCreateObjectiveTemplates = canCreateObjectiveTemplates;
            CanDeleteObjectiveTemplates = canDeleteObjectiveTemplates;
            CanCreateWorkspaces = canCreateWorkspaces;
            CanReadWorkspaces = canReadWorkspaces;
            CanUpdateWorkspaces = canUpdateWorkspaces;
            CanDeleteWorkspaces = canDeleteWorkspaces;
            CanCreateGroups = canCreateGroups;
            CanUpdateGroups = canUpdateGroups;
            CanDeleteGroups = canDeleteGroups;
            CanAddGroupMembers = canAddGroupMembers;
            CanRemoveGroupMembers = canRemoveGroupMembers;
            CanCreateRoles = canCreateRoles;
            CanUpdateRoles = canUpdateRoles;
            CanDeleteRoles = canDeleteRoles;
            CanCreateUsers = canCreateUsers;
            CanUpdateUsers = canUpdateUsers;
            CanDeleteUsers = canDeleteUsers;
            CanAddResourceAssignments = canAddResourceAssignments;
            CanListResourceAssignments = canListResourceAssignments;
            CanRemoveResourceAssignments = canRemoveResourceAssignments;
            CanCreatePatients = canCreatePatients;
            CanReadPatients = canReadPatients;
            CanUpdatePatients = canUpdatePatients;
            CanDeletePatients = canDeletePatients;
            CanCopyPatients = canCopyPatients;
            CanMovePatients = canMovePatients;
            CanViewPatientsPHI = canViewPatientsPHI;
            CanContourPatients = canContourPatients;
            CanUploadPatientDICOM = canUploadPatientDICOM;
            CanCreatePatientChecklists = canCreatePatientChecklists;
            CanReadPatientChecklists = canReadPatientChecklists;
            CanUpdatePatientChecklists = canUpdatePatientChecklists;
            CanDeletePatientChecklists = canDeletePatientChecklists;
            CanCreatePatientScorecards = canCreatePatientScorecards;
            CanReadPatientScorecards = canReadPatientScorecards;
            CanUpdatePatientScorecards = canUpdatePatientScorecards;
            CanDeletePatientScorecards = canDeletePatientScorecards;
            CanCreatePatientDocuments = canCreatePatientDocuments;
            CanReadPatientDocuments = canReadPatientDocuments;
            CanUpdatePatientDocuments = canUpdatePatientDocuments;
            CanDeletePatientDocuments = canDeletePatientDocuments;
            CanCreatePatientNotes = canCreatePatientNotes;
            CanReadPatientNotes = canReadPatientNotes;
            CanUpdatePatientNotes = canUpdatePatientNotes;
            CanDeletePatientNotes = canDeletePatientNotes;
            CanCreateCollections = canCreateCollections;
            CanReadCollections = canReadCollections;
            CanUpdateCollections = canUpdateCollections;
            CanDeleteCollections = canDeleteCollections;
            CanAddCollectionPatients = canAddCollectionPatients;
            CanRemoveCollectionPatients = canRemoveCollectionPatients;
            CanCreateCollectionScorecards = canCreateCollectionScorecards;
            CanReadCollectionScorecards = canReadCollectionScorecards;
            CanUpdateCollectionScorecards = canUpdateCollectionScorecards;
            CanDeleteCollectionScorecards = canDeleteCollectionScorecards;
            CanCreateCollectionBookmarks = canCreateCollectionBookmarks;
            CanReadCollectionBookmarks = canReadCollectionBookmarks;
            CanUpdateCollectionBookmarks = canUpdateCollectionBookmarks;
            CanDeleteCollectionBookmarks = canDeleteCollectionBookmarks;
        }
    }
}
