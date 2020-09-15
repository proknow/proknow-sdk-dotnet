using ProKnow.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow.Role
{
    /// <summary>
    /// Converts a RoleItem to and from its JSON representation.  This converter is necessary because the ProKnow API
    /// has the organization permissions at the root level rather than as a separate property
    /// </summary>
    public class RoleItemJsonConverter : JsonConverter<RoleItem>
    {
        /// <summary>
        /// Reads a RoleItem from its JSON representation
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">The serializer options</param>
        /// <returns>The role</returns>
        public override RoleItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new ProKnowException($"Unexpected token parsing role.  Expected StartObject, got {reader.TokenType}.");
            }

            var roleItem = new RoleItem();

            // Deserialize the role as OrganizationPermissions, since they're at the root level
            var permissions = JsonSerializer.Deserialize<OrganizationPermissions>(ref reader, options);

            // Move the role ID, name, and private flag from the OrganizationPermissions ExtensionData to the role and add the permissions
            roleItem.Id = ((JsonElement)permissions.ExtensionData["id"]).GetString();
            permissions.ExtensionData.Remove("id");
            roleItem.Name = ((JsonElement)permissions.ExtensionData["name"]).GetString();
            permissions.ExtensionData.Remove("name");
            roleItem.IsPrivate = ((JsonElement)permissions.ExtensionData["private"]).GetBoolean();
            permissions.ExtensionData.Remove("private");
            roleItem.Permissions = permissions;

            // Move what remains of the OrganizationPermissions ExtensionData to the role
            roleItem.ExtensionData = permissions.ExtensionData;
            permissions.ExtensionData = null;

            return roleItem;
        }

        /// <summary>
        /// Writes a RoleItem as its JSON representation
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="value">The role to convert</param>
        /// <param name="options">The JSON serializer options</param>
        public override void Write(Utf8JsonWriter writer, RoleItem value, JsonSerializerOptions options)
        {
            // Convert permissions to Dictionary<string, object>
            var properties = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(value.Permissions));

            // Add role name to dictionary, if provided and role is not private
            if (!value.IsPrivate && value.Name != null)
            {
                properties["name"] = value.Name;
            }

            // Write out dictionary contents
            //writer.WriteStartObject();
            //foreach (KeyValuePair<string, object> kvp in properties)
            //{
            //    writer.WritePropertyName(kvp.Key.ToString());
            //    JsonSerializer.Serialize(writer, kvp.Value, options);
            //}
            //writer.WriteEndObject();
            JsonSerializer.Serialize<Dictionary<string, object>>(writer, properties, options);
        }
    }
}
