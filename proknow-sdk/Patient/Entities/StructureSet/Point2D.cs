namespace ProKnow.Patient.Entities.StructureSet
{
    /// <summary>
    /// Represents a 2D point in couch IEC coordinates
    /// </summary>
    public class Point2D
    {
        /// <summary>
        /// The couch IEC x-coordinate
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The couch IEC z-coordinate
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Constructs a Point2D
        /// </summary>
        /// <param name="x">The couch IEC x-coordinate</param>
        /// <param name="z">The couch IEC z-coordinate</param>
        public Point2D(double x, double z)
        {
            X = x;
            Z = z;
        }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return $"{X.ToString("0.###")}, {Z.ToString("0.###")}";
        }
    }
}
