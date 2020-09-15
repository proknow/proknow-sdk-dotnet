using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.Json;

namespace ProKnow.Role.Test
{
    [TestClass]
    public class RoleItemJsonConverterTest
    {
        private readonly RoleItemJsonConverter _roleItemJsonConverter = new RoleItemJsonConverter();

        [TestMethod]
        public void ReadTest()
        {
            var jsonString = "{\"id\":\"5f5f56fbbf408bd8f14802c88075572b\",\"name\":\"SDK-UsersTest-Public\",\"private\":true,\"user\":null,\"create_api_keys\":true,\"manage_access\":true,\"manage_custom_metrics\":true,\"manage_template_metric_sets\":true,\"manage_renaming_rules\":true,\"manage_template_checklists\":true,\"organization_collaborator\":true,\"organization_read_patients\":true,\"organization_read_collections\":true,\"organization_view_phi\":true,\"organization_download_dicom\":true,\"organization_write_collections\":true,\"organization_write_patients\":true,\"organization_contour_patients\":true,\"organization_delete_collections\":true,\"organization_delete_patients\":true,\"created_at\":\"2020-09-14T11:41:47.753Z\",\"workspaces\":[]}";
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_roleItemJsonConverter);
            var roleItem = JsonSerializer.Deserialize<RoleItem>(jsonString, jsonSerializerOptions);
            Assert.AreEqual("5f5f56fbbf408bd8f14802c88075572b", roleItem.Id);
            Assert.AreEqual("SDK-UsersTest-Public", roleItem.Name);
            Assert.IsTrue(roleItem.IsPrivate);
            Assert.IsTrue(roleItem.Permissions.CanCreateApiKeys);
            Assert.IsTrue(roleItem.Permissions.CanManageAccess);
            Assert.IsTrue(roleItem.Permissions.CanManageCustomMetrics);
            Assert.IsTrue(roleItem.Permissions.CanManageScorecardTemplates);
            Assert.IsTrue(roleItem.Permissions.CanManageRenamingRules);
            Assert.IsTrue(roleItem.Permissions.CanManageChecklistTemplates);
            Assert.IsTrue(roleItem.Permissions.IsCollaborator);
            Assert.IsTrue(roleItem.Permissions.CanReadPatients);
            Assert.IsTrue(roleItem.Permissions.CanReadCollections);
            Assert.IsTrue(roleItem.Permissions.CanViewPhi);
            Assert.IsTrue(roleItem.Permissions.CanDownloadDicom);
            Assert.IsTrue(roleItem.Permissions.CanWriteCollections);
            Assert.IsTrue(roleItem.Permissions.CanWritePatients);
            Assert.IsTrue(roleItem.Permissions.CanContourPatients);
            Assert.IsTrue(roleItem.Permissions.CanDeleteCollections);
            Assert.IsTrue(roleItem.Permissions.CanDeletePatients);
            Assert.AreEqual(0, roleItem.Permissions.Workspaces.Count);
            Assert.IsNull(roleItem.ExtensionData["user"]);
            Assert.AreEqual("2020-09-14T11:41:47.753Z", ((JsonElement)roleItem.ExtensionData["created_at"]).GetString());
        }

        [TestMethod]
        public void WriteTest_PublicRole()
        {
            var roleItem = new RoleItem();
            roleItem.Id = "5f5f56fbbf408bd8f14802c88075572b";
            roleItem.Name = "SDK-UsersTest-Public";
            roleItem.IsPrivate = false;
            roleItem.Permissions.CanCreateApiKeys = true;
            roleItem.Permissions.CanManageAccess = true;
            roleItem.Permissions.CanManageCustomMetrics = true;
            roleItem.Permissions.CanManageScorecardTemplates = true;
            roleItem.Permissions.CanManageRenamingRules = true;
            roleItem.Permissions.CanManageChecklistTemplates = true;
            roleItem.Permissions.IsCollaborator = true;
            roleItem.Permissions.CanReadPatients = true;
            roleItem.Permissions.CanReadCollections = true;
            roleItem.Permissions.CanViewPhi = true;
            roleItem.Permissions.CanDownloadDicom = true;
            roleItem.Permissions.CanWriteCollections = true;
            roleItem.Permissions.CanWritePatients = true;
            roleItem.Permissions.CanContourPatients = true;
            roleItem.Permissions.CanDeleteCollections = true;
            roleItem.Permissions.CanDeletePatients = true;
            roleItem.ExtensionData = new Dictionary<string, object>();
            roleItem.ExtensionData.Add("user", null);
            roleItem.ExtensionData.Add("created_at", "2020-09-14T11:41:47.753Z");
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_roleItemJsonConverter);
            var jsonString = JsonSerializer.Serialize(roleItem, jsonSerializerOptions);
            Assert.AreEqual("{\"create_api_keys\":true,\"manage_access\":true,\"manage_custom_metrics\":true,\"manage_template_metric_sets\":true,\"manage_renaming_rules\":true,\"manage_template_checklists\":true,\"organization_collaborator\":true,\"organization_read_patients\":true,\"organization_read_collections\":true,\"organization_view_phi\":true,\"organization_download_dicom\":true,\"organization_write_collections\":true,\"organization_write_patients\":true,\"organization_contour_patients\":true,\"organization_delete_collections\":true,\"organization_delete_patients\":true,\"workspaces\":[],\"name\":\"SDK-UsersTest-Public\"}",
                jsonString);
        }

        [TestMethod]
        public void WriteTest_PrivateRole()
        {
            var roleItem = new RoleItem();
            roleItem.Id = "5f5f56fbbf408bd8f14802c88075572b";
            roleItem.Name = "SDK-UsersTest-Private";
            roleItem.IsPrivate = true;
            roleItem.Permissions.CanCreateApiKeys = true;
            roleItem.Permissions.CanManageAccess = true;
            roleItem.Permissions.CanManageCustomMetrics = true;
            roleItem.Permissions.CanManageScorecardTemplates = true;
            roleItem.Permissions.CanManageRenamingRules = true;
            roleItem.Permissions.CanManageChecklistTemplates = true;
            roleItem.Permissions.IsCollaborator = true;
            roleItem.Permissions.CanReadPatients = true;
            roleItem.Permissions.CanReadCollections = true;
            roleItem.Permissions.CanViewPhi = true;
            roleItem.Permissions.CanDownloadDicom = true;
            roleItem.Permissions.CanWriteCollections = true;
            roleItem.Permissions.CanWritePatients = true;
            roleItem.Permissions.CanContourPatients = true;
            roleItem.Permissions.CanDeleteCollections = true;
            roleItem.Permissions.CanDeletePatients = true;
            roleItem.ExtensionData = new Dictionary<string, object>();
            roleItem.ExtensionData.Add("user", null);
            roleItem.ExtensionData.Add("created_at", "2020-09-14T11:41:47.753Z");
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_roleItemJsonConverter);
            var jsonString = JsonSerializer.Serialize(roleItem, jsonSerializerOptions);
            Assert.AreEqual("{\"create_api_keys\":true,\"manage_access\":true,\"manage_custom_metrics\":true,\"manage_template_metric_sets\":true,\"manage_renaming_rules\":true,\"manage_template_checklists\":true,\"organization_collaborator\":true,\"organization_read_patients\":true,\"organization_read_collections\":true,\"organization_view_phi\":true,\"organization_download_dicom\":true,\"organization_write_collections\":true,\"organization_write_patients\":true,\"organization_contour_patients\":true,\"organization_delete_collections\":true,\"organization_delete_patients\":true,\"workspaces\":[]}",
                jsonString);
        }
    }
}
