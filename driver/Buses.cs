using SME;

namespace MicDriver {
    
    [InitializedBus]
    public interface PMOD : IBus {
        [InitialValue(true)]
        bool chip_select { get; set; }
        bool data { get; set; }
    }

    [InitializedBus]
    public interface Digital : IBus {
        short audio { get; set; }
    }

    [InitializedBus]
    public interface Control : IBus {
        bool read { get; set; }
        bool rdy { get; set; }
        bool rst { get; set; }
    }

}
