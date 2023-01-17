using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpLib.EuropeanDataFormat
{
    public class File : IDisposable
    {
        public Header Header { get; set; }
        public Signal[] Signals { get; set; }
        public IList<AnnotationSignal> AnnotationSignals { get; set; }

        private Reader iReader;

        public File() { AnnotationSignals = new List<AnnotationSignal>(); }
        public File(string edfFilePath, IFile file)
        {
            ReadAll(edfFilePath, file);
        }

        public File(byte[] edfBytes)
        {
            ReadAll(edfBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (iReader != null)
            {
                iReader.Dispose();
                iReader = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edfBase64"></param>
        public void ReadBase64(string edfBase64)
        {
            byte[] edfBytes = System.Convert.FromBase64String(edfBase64);
            ReadAll(edfBytes);
        }

        /// <summary>
        /// Open the given EDF file, read its header and allocate corresponding Signal objects.
        /// </summary>
        /// <param name="edfFilePath"></param>
        public void Open(string edfFilePath, IFile file)
        {
            // Open file
            iReader = new Reader(file.Open(edfFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read));
            // Read headers
            Header = iReader.ReadHeader();
            // Allocate signals
            Signals = iReader.AllocateSignals(Header);
        }

        /// <summary>
        /// Read the signal at the given index.
        /// </summary>
        /// <param name="aIndex"></param>
        public void ReadSignal(int aIndex)
        {
            iReader.ReadSignal(Header, Signals[aIndex]);
        }

        /// <summary>
        /// Read the signal matching the given name.
        /// </summary>
        /// <param name="aContains"></param>
        /// <returns></returns>
        public Signal ReadSignal(string aMatch)
        {
            var signal = Signals.FirstOrDefault(s => s.Label.Value.Equals(aMatch));
            if (signal == null)
            {
                return null;
            }

            iReader.ReadSignal(Header, signal);
            return signal;
        }

        /// <summary>
        /// Read the whole file into memory
        /// </summary>
        /// <param name="edfFilePath"></param>
        public void ReadAll(string edfFilePath, IFile file)
        {
            using (var reader = new Reader(file.Open(edfFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read)))
            {
                Header = reader.ReadHeader();
                Signals = reader.ReadSignals(Header);
               // AnnotationSignals = reader.ReadAnnotationSignals(Header);
            }
        }

        /// <summary>
        /// Read a whole EDF file from a memory buffer. 
        /// </summary>
        /// <param name="edfBytes"></param>
        public void ReadAll(byte[] edfBytes)
        {
            using (var r = new Reader(edfBytes))
            {
                Header = r.ReadHeader();
                Signals = r.ReadSignals(Header);
            }
        }

        public void Save(string edfFilePath, IFile file)
        {
            if (Header == null) return;

            using (var writer = new Writer(file.Open(edfFilePath, System.IO.FileMode.Create)))
            {
                writer.WriteEDF(this, edfFilePath);
            }
        }
    }
}
