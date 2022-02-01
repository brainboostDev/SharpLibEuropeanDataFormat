using System.Collections.Generic;
using System.Linq;

namespace SharpLib.EuropeanDataFormat
{
    public interface IBaseSignal<T>
    {
        /// <summary>
        /// Index of that signal in the EDF file it belongs to.
        /// </summary>
        int Index { get; set; }
        FixedLengthString Label { get; }
        FixedLengthString TransducerType { get; }
        FixedLengthString PhysicalDimension { get; }
        FixedLengthDouble PhysicalMinimum { get; }
        FixedLengthDouble PhysicalMaximum { get; }
        FixedLengthInt DigitalMinimum { get; }
        FixedLengthInt DigitalMaximum { get; }
        FixedLengthString Prefiltering { get; }
        FixedLengthInt SampleCountPerRecord { get; }
        FixedLengthString Reserved { get; }
        List<T> Samples { get; set; }
    }

    public class Signal : IBaseSignal<short>
    {
        public int Index { get; set; }

        public FixedLengthString Label { get; } = new FixedLengthString(HeaderItems.Label);

        public FixedLengthString TransducerType { get; } = new FixedLengthString(HeaderItems.TransducerType);

        public FixedLengthString PhysicalDimension { get; } = new FixedLengthString(HeaderItems.PhysicalDimension);

        public FixedLengthDouble PhysicalMinimum { get; } = new FixedLengthDouble(HeaderItems.PhysicalMinimum);

        public FixedLengthDouble PhysicalMaximum { get; } = new FixedLengthDouble(HeaderItems.PhysicalMaximum);

        public FixedLengthInt DigitalMinimum { get; } = new FixedLengthInt(HeaderItems.DigitalMinimum);

        public FixedLengthInt DigitalMaximum { get; } = new FixedLengthInt(HeaderItems.DigitalMaximum);

        public FixedLengthString Prefiltering { get; } = new FixedLengthString(HeaderItems.Prefiltering);

        public FixedLengthInt SampleCountPerRecord { get; } = new FixedLengthInt(HeaderItems.NumberOfSamplesInDataRecord);

        public FixedLengthString Reserved { get; } = new FixedLengthString(HeaderItems.SignalsReserved);

        public List<short> Samples { get; set; } = new List<short> { };

        /// <summary>
        /// Provided sample value after scaling.
        /// </summary>
        /// <param name="aIndex"></param>
        /// <returns></returns>
        public double ScaledSample(int aIndex) { return Samples[aIndex] * ScaleFactor(); }

        /// <summary>
        /// Provide sample scaling factor.
        /// </summary>
        /// <returns></returns>
        public double ScaleFactor() { return (PhysicalMaximum.Value - PhysicalMinimum.Value)/(DigitalMaximum.Value - DigitalMinimum.Value); }

        public override string ToString()
        {
            return Label.Value + " " + SampleCountPerRecord.Value.ToString() + "/" + Samples.Count().ToString() + " [" 
                + string.Join(",", Samples.Skip(0).Take(10).ToArray()) + " ...]";
        }
    }


}
