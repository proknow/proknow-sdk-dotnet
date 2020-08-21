namespace ProKnow.Geometry
{
    /// <summary>
    /// Represents a 3D point in couch IEC coordinates
    /// </summary>
    public class Point3D
    {
        /// <summary>
        /// The couch IEC x-coordinate
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The couch IEC y-coordinate
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The couch IEC z-coordinate
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Constructs a Point3D
        /// </summary>
        /// <param name="x">The couch IEC x-coordinate</param>
        /// <param name="y">The couch IEC y-coordinate</param>
        /// <param name="z">The couch IEC z-coordinate</param>
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return $"{X.ToString("0.###")}, {Y.ToString("0.###")}, {Z.ToString("0.###")}";
        }
    }
}
