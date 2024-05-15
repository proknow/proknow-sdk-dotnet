using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// A container for the dose entity item data
    /// </summary>
    public class DoseData
    {
        /// <summary>
        /// Dicom blob ids
        /// </summary>
        [JsonPropertyName("dicom")]
        public string[] Dicom { get; set; }

        /// <summary>
        /// JSON web token (JWT) for dicom blob ids
        /// </summary>
        [JsonPropertyName("dicom_token")]
        public string DicomToken { get; set; }

        /// <summary>
        /// The process id for the dose in RTV
        /// </summary>
        [JsonPropertyName("processed_id")]
        public string ProcessedId { get; set; }

        /// <summary>
        /// The minimum IEC couch x position in mm
        /// </summary>
        [JsonPropertyName("min_x")]
        public double MinX { get; set; }

        /// <summary>
        /// The minimum IEC couch y position in mm
        /// </summary>
        [JsonPropertyName("min_y")]
        public double MinY { get; set; }

        /// <summary>
        /// The minimum IEC couch z position in mm
        /// </summary>
        [JsonPropertyName("min_z")]
        public double MinZ { get; set; }

        /// <summary>
        /// The maximum IEC couch x position in mm
        /// </summary>
        [JsonPropertyName("max_x")]
        public double MaxX { get; set; }

        /// <summary>
        /// The maximum IEC couch y position in mm
        /// </summary>
        [JsonPropertyName("max_y")]
        public double MaxY { get; set; }

        /// <summary>
        /// The maximum IEC couch z position in mm
        /// </summary>
        [JsonPropertyName("max_z")]
        public double MaxZ { get; set; }

        /// <summary>
        /// The number of voxels in the IEC x direction
        /// </summary>
        [JsonPropertyName("resolution_x")]
        public int ResolutionX { get; set; }

        /// <summary>
        /// The number of voxels in the IEC y direction
        /// </summary>
        [JsonPropertyName("resolution_y")]
        public int ResolutionY { get; set; }

        /// <summary>
        /// The number of voxels in the IEC z direction
        /// </summary>
        [JsonPropertyName("resolution_z")]
        public int ResolutionZ { get; set; }

        /// <summary>
        /// The voxel spacing in the IEC x direction in mm
        /// </summary>
        [JsonPropertyName("spacing_x")]
        public double SpacingX { get; set; }

        /// <summary>
        /// The (average) voxel spacing in the IEC y direction in mm
        /// </summary>
        [JsonPropertyName("spacing_y")]
        public double SpacingY { get; set; }

        /// <summary>
        /// The voxel spacing in the IEC z direction in mm
        /// </summary>
        [JsonPropertyName("spacing_z")]
        public double SpacingZ { get; set; }

        /// <summary>
        /// The size of the dose volume in the IEC x direction in mm
        /// </summary>
        [JsonPropertyName("size_x")]
        public double SizeX { get; set; }

        /// <summary>
        /// The size of the dose volume in the IEC y direction in mm
        /// </summary>
        [JsonPropertyName("size_y")]
        public double SizeY { get; set; }

        /// <summary>
        /// The size of the dose volume in the IEC z direction in mm
        /// </summary>
        [JsonPropertyName("size_z")]
        public double SizeZ { get; set; }

        /// <summary>
        /// True if the dose slabs in the IEC y direction are uniformly spaced
        /// </summary>
        [JsonPropertyName("uniform_y")]
        public bool IsUniformY { get; set; }

        /// <summary>
        /// The intercept of the linear transformation from stored voxel value to output units
        /// </summary>
        [JsonPropertyName("pixel_intercept")]
        public double PixelIntercept { get; set; }

        /// <summary>
        /// The slope of the linear transformation from stored voxel value to output units
        /// </summary>
        [JsonPropertyName("pixel_slope")]
        public double PixelSlope { get; set; }

        /// <summary>
        /// The minimum dose value
        /// </summary>
        [JsonPropertyName("min_value")]
        public double MinValue { get; set; }

        /// <summary>
        /// The maximum dose value
        /// </summary>
        [JsonPropertyName("max_value")]
        public double MaxValue { get; set; }

        /// <summary>
        /// The summation type ("PLAN", "MULTI_PLAN", "FRACTION", "BEAM", "BRACHY", "FRACTION_SESSION", "BEAM_SESSION",
        /// "BRACHY_SESSION", "CONTROL_POINT", or "RECORD")
        /// </summary>
        [JsonPropertyName("summation_type")]
        public string SummationType { get; set; }

        /// <summary>
        /// The dose type ("PHYSICAL", "EFFECTIVE", or "ERROR")
        /// </summary>
        [JsonPropertyName("type")]
        public string DoseType { get; set; }

        /// <summary>
        /// The dose units ("GY" or "RELATIVE")
        /// </summary>
        [JsonPropertyName("units")]
        public string DoseUnits { get; set; }

        /// <summary>
        /// The dose slices in the IEC y direction
        /// </summary>
        [JsonPropertyName("slices")]
        public IList<DoseSlice> Slices { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}
