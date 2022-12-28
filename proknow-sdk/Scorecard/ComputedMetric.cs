using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Represents a computed metric
    /// </summary>
    [JsonConverter(typeof(ComputedMetricJsonConverter))]
    public class ComputedMetric
    {
        /// <summary>
        /// The metric type, one of:
        /// <para/>'DOSE_VOLUME_CC_ROI' - Dose (Gy) covering Arg1 (cc) of the RoiName
        /// <para/>'DOSE_VOLUME_MINUS_CC_ROI' - Dose (Gy) covering the total volume (cc) minus Arg1 (cc) of the RoiName
        /// <para/>'DOSE_VOLUME_PERCENT_ROI' - Dose (Gy) covering Arg1 (%) of the RoiName
        /// <para/>'VOLUME_CC_DOSE_ROI' - Volume (cc) of the RoiName covered by Arg1 (Gy)
        /// <para/>'VOLUME_PERCENT_DOSE_ROI' - Volume (%) of the RoiName covered by Arg1 (Gy)
        /// <para/>'VOLUME_CC_DOSE_RANGE_ROI' - Volume (cc) of the RoiName in range of Arg1 to Arg2 (Gy)
        /// <para/>'VOLUME_PERCENT_DOSE_RANGE_ROI' - Volume (%) of the RoiName in range of Arg1 to Arg2 (Gy)
        /// <para/>'MIN_DOSE_ROI' - Minimum dose (Gy) to the RoiName
        /// <para/>'MAX_DOSE_ROI' - Maximum dose (Gy) to the RoiName
        /// <para/>'MEAN_DOSE_ROI' - Mean dose (Gy) to the RoiName
        /// <para/>'INTEGRAL_DOSE_ROI' - Integral dose (Gy * cc) to the RoiName
        /// <para/>'MAX_DOSE' - Global maximum dose (Gy) over the enire dose grid
        /// <para/>'VOLUME_OF_REGRET' - Total volume (cc) covered by Arg1 (Gy) but outside of the RoiName
        /// <para/>'IRRADIATED_VOLUME' - Total volume (cc) covered by Arg1 (Gy)
        /// <para/>'CONFORMATION_NUMBER' - Conformation number of the RoiName at Arg1 (Gy)
        /// <para/>'CONFORMALITY_INDEX' - Conformality index of the RoiName at Arg1 (Gy)
        /// <para/>'HOMOGENEITY_INDEX' - Homogeneity index of the RoiName at Arg1 (Gy)
        /// <para/>'INHOMOGENEITY_INDEX' - Inhomogeneity index of the RoiName
        /// <para/>'CUMULATIVE_METERSET' - Cumulative meterset
        /// <para/>'ESTIMATED_BEAM_TIME' - Estimated beam time
        /// <para/>'VOLUME'- Volume (cc) of the RoiName
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The ROI name or null if not required
        /// </summary>
        [JsonPropertyName("roi_name")]
        public string RoiName { get; set; }

        /// <summary>
        /// The first argument or null if not required
        /// </summary>
        [JsonPropertyName("arg_1")]
        public double? Arg1 { get; set; }

        /// <summary>
        /// The second argument or null if not required
        /// </summary>
        [JsonPropertyName("arg_2")]
        public double? Arg2 { get; set; }

        /// <summary>
        /// The rx or null if not required
        /// </summary>
        [JsonPropertyName("rx")]
        public string Rx { get; set; }

        /// <summary>
        /// The rx_scale or null if not required
        /// </summary>
        [JsonPropertyName("rx_scale")]
        public double? RxScale { get; set; }

        /// <summary>
        /// The objectives or null if not specified
        /// </summary>
        [JsonPropertyName("objectives")]
        public IList<MetricBin> Objectives { get; set; }

        /// <summary>
        /// Used by deserialization to create a computed metric
        /// </summary>
        public ComputedMetric()
        {
        }

        /// <summary>
        /// Constructs a computed metric
        /// </summary>
        /// <param name="type">The metric type</param>
        /// <param name="roiName">The ROI name or null if not required</param>
        /// <param name="arg1">The first argument or null if not required</param>
        /// <param name="arg2">The second argument or null if not required</param>
        /// <param name="objectives">The objectives or null if not specified</param>
        /// <param name="rx">The rx value or null if not required</param>
        /// <param name="rxScale">The RX scale or null if not required</param>"
        /// <remarks>
        /// The allowable values for metric type are:
        /// <para/>'DOSE_VOLUME_CC_ROI' - Dose (Gy) covering Arg1 (cc) of the RoiName
        /// <para/>'DOSE_VOLUME_MINUS_CC_ROI' - Dose (Gy) covering the total volume (cc) minus Arg1 (cc) of the RoiName
        /// <para/>'DOSE_VOLUME_PERCENT_ROI' - Dose (Gy) covering Arg1 (%) of the RoiName
        /// <para/>'VOLUME_CC_DOSE_ROI' - Volume (cc) of the RoiName covered by Arg1 (Gy)
        /// <para/>'VOLUME_PERCENT_DOSE_ROI' - Volume (%) of the RoiName covered by Arg1 (Gy)
        /// <para/>'VOLUME_CC_DOSE_RANGE_ROI' - Volume (cc) of the RoiName in range of Arg1 to Arg2 (Gy)
        /// <para/>'VOLUME_PERCENT_DOSE_RANGE_ROI' - Volume (%) of the RoiName in range of Arg1 to Arg2 (Gy)
        /// <para/>'MIN_DOSE_ROI' - Minimum dose (Gy) to the RoiName
        /// <para/>'MAX_DOSE_ROI' - Maximum dose (Gy) to the RoiName
        /// <para/>'MEAN_DOSE_ROI' - Mean dose (Gy) to the RoiName
        /// <para/>'INTEGRAL_DOSE_ROI' - Integral dose (Gy * cc) to the RoiName
        /// <para/>'MAX_DOSE' - Global maximum dose (Gy) over the enire dose grid
        /// <para/>'VOLUME_OF_REGRET' - Total volume (cc) covered by Arg1 (Gy) but outside of the RoiName
        /// <para/>'IRRADIATED_VOLUME' - Total volume (cc) covered by Arg1 (Gy)
        /// <para/>'CONFORMATION_NUMBER' - Conformation number of the RoiName at Arg1 (Gy)
        /// <para/>'CONFORMALITY_INDEX' - Conformality index of the RoiName at Arg1 (Gy)
        /// <para/>'HOMOGENEITY_INDEX' - Homogeneity index of the RoiName at Arg1 (Gy)
        /// <para/>'INHOMOGENEITY_INDEX' - Inhomogeneity index of the RoiName
        /// <para/>'CUMULATIVE_METERSET' - Cumulative meterset
        /// <para/>'ESTIMATED_BEAM_TIME' - Estimated beam time
        /// <para/>'VOLUME'- Volume (cc) of the RoiName
        /// </remarks>
        public ComputedMetric(string type, string roiName = null, double? arg1 = null, double? arg2 = null,
            IList<MetricBin> objectives = null, string rx = null, double? rxScale = null)
        {
            Type = type;
            RoiName = roiName;
            Arg1 = arg1;
            Arg2 = arg2;
            Rx = rx;
            RxScale = rxScale;
            Objectives = objectives;
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            var roiName = RoiName != null ? $" | {RoiName}" : "";
            var arg1 = Arg1 != null ? $" | {Arg1}" : "";
            var arg2 = Arg2 != null ? $" | {Arg2}" : "";
            var rx = Rx != null ? $" | {Rx}" : "";
            var rxScale = RxScale != null ? $" | {RxScale}" : "";
            return $"{Type}{roiName}{arg1}{arg2}{rx}{rxScale}";
        }
    }
}
