using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// A container for the image set entity data
    /// </summary>
    public class ImageSetData
    {
        /// <summary>
        /// The maximum scaled pixel value
        /// </summary>
        [JsonPropertyName("max_value")]
        public double MaxValue { get; set; }

        /// <summary>
        /// The minimum scaled pixel value
        /// </summary>
        [JsonPropertyName("min_value")]
        public double MinValue { get; set; }

        /// <summary>
        /// True if the image set is oblique with respect to the IEC patient coordinate axes
        /// </summary>
        [JsonPropertyName("oblique")]
        public bool IsOblique { get; set; }

        /// <summary>
        /// The position of the patient relative to the imaging equipment space
        /// </summary>
        [JsonPropertyName("patient_position")]
        public string PatientPosition { get; set; }

        /// <summary>
        /// The intended interpretation of the pixel data.  Standard values are MONOCHROME1, MONOCHROME2, RGB,
        /// PALETTECOLOR, YBR_FULL, YBR_FULL_422, YBR_RCT, and YBR_ICT
        /// </summary>
        [JsonPropertyName("photometric_interpretation")]
        public string PhotometricInterpretation { get; set; }

        /// <summary>
        /// The number of columns in each image
        /// </summary>
        [JsonPropertyName("resolution_u")]
        public ushort NumberOfColumns { get; set; }

        /// <summary>
        /// The number of rows in each image
        /// </summary>
        [JsonPropertyName("resolution_v")]
        public ushort NumberOfRows { get; set; }

        /// <summary>
        /// The number of images
        /// </summary>
        [JsonPropertyName("resolution_w")]
        public ushort NumberOfImages { get; set; }

        /// <summary>
        /// The image size in the column direction in mm
        /// </summary>
        [JsonPropertyName("size_u")]
        public double ColumnExtents { get; set; }

        /// <summary>
        /// The image size in the row direction in mm
        /// </summary>
        [JsonPropertyName("size_v")]
        public double RowExtents { get; set; }

        /// <summary>
        /// The image set size in the slice direction in mm
        /// </summary>
        [JsonPropertyName("size_w")]
        public double SliceExtents { get; set; }

        /// <summary>
        /// Distance between the center of each pixel in the column direction in mm
        /// </summary>
        [JsonPropertyName("spacing_u")]
        public double ColumnSpacing { get; set; }

        /// <summary>
        /// Distance between the center of each pixel in the row direction in mm
        /// </summary>
        [JsonPropertyName("spacing_v")]
        public double RowSpacing { get; set; }

        /// <summary>
        /// The slice spacing in mm.  In the case of non-uniform slice spacing, this is a nominal slice spacing
        /// </summary>
        [JsonPropertyName("spacing_w")]
        public double SliceSpacing { get; set; }

        /// <summary>
        /// True if the slice spacing is uniform
        /// </summary>
        [JsonPropertyName("uniform_w")]
        public bool HasUniformSliceSpacing { get; set; }

        /// <summary>
        /// The cosine of the angle between the first row of an image and IEC patient X axis
        /// </summary>
        [JsonPropertyName("u_x")]
        public double RowXDirectionCosine { get; set; }

        /// <summary>
        /// The cosine of the angle between the first row of an image and IEC patient Y axis
        /// </summary>
        [JsonPropertyName("u_y")]
        public double RowYDirectionCosine { get; set; }

        /// <summary>
        /// The cosine of the angle between the first row of an image and IEC patient Z axis
        /// </summary>
        [JsonPropertyName("u_z")]
        public double RowZDirectionCosine { get; set; }

        /// <summary>
        /// The cosine of the angle between the first column of an image and IEC patient X axis
        /// </summary>
        [JsonPropertyName("v_x")]
        public double ColumnXDirectionCosine { get; set; }

        /// <summary>
        /// The cosine of the angle between the first column of an image and IEC patient Y axis
        /// </summary>
        [JsonPropertyName("v_y")]
        public double ColumnYDirectionCosine { get; set; }

        /// <summary>
        /// The cosine of the angle between the first column of an image and IEC patient Z axis
        /// </summary>
        [JsonPropertyName("v_z")]
        public double ColumnZDirectionCosine { get; set; }

        /// <summary>
        /// The minimum IEC patient X coordinate in mm
        /// </summary>
        [JsonPropertyName("min_x")]
        public double MinX { get; set; }

        /// <summary>
        /// The minimum IEC patient Y coordinate in mm
        /// </summary>
        [JsonPropertyName("min_y")]
        public double MinY { get; set; }

        /// <summary>
        /// The minimum IEC patient Z coordinate in mm
        /// </summary>
        [JsonPropertyName("min_z")]
        public double MinZ { get; set; }

        /// <summary>
        /// The maximum IEC patient X coordinate in mm
        /// </summary>
        [JsonPropertyName("max_x")]
        public double MaxX { get; set; }

        /// <summary>
        /// The maximum IEC patient Y coordinate in mm
        /// </summary>
        [JsonPropertyName("max_y")]
        public double MaxY { get; set; }

        /// <summary>
        /// The maximum IEC patient Z coordinate in mm
        /// </summary>
        [JsonPropertyName("max_z")]
        public double MaxZ { get; set; }

        /// <summary>
        /// The images
        /// </summary>
        [JsonPropertyName("images")]
        public IList<Image> Images { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}
