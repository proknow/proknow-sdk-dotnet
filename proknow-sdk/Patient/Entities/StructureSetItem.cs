using System;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents a structure set for a patient
    /// </summary>
    public class StructureSetItem : EntityItem
    {
        /// <summary>
        /// Downloads this entity as DICOM object(s) to the specified folder
        /// </summary>
        /// <param name="root">The full path to the destination root folder</param>
        /// <returns>The full path to the destination folder (root or a sub-folder) to which the file(s) were downloaded</returns>
        public override Task<string> Download(string root)
        {
            throw new NotImplementedException("StructureSetItem.Download()");
        }
    }
}
