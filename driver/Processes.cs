using System;
using System.Threading.Tasks;
using SME;

namespace MicDriver {
    
    [ClockedProcess]
    public class PMOD_Controller : StateProcess {
        [InputBus] public Control ctrl_in;
        [InputBus] public PMOD pmod_in;

        [OutputBus] public Control ctrl_out;
        [OutputBus] public PMOD pmod_out;
        [OutputBus] public Digital dgtl;

        protected override async Task OnTickAsync() {
            while (!ctrl_in.read) 
                await ClockAsync();

            pmod_out.chip_select = false;
            
            for (int i = 0; i < 4; i++)
                await ClockAsync(); // First 4 is zeros from device

            short tmp = 0;
            for (int i = 0; i < 12; i++) {
                int data = pmod_in.data ? 1 : 0;
                tmp |= (short) (data << i);
                await ClockAsync();
            }
            pmod_out.chip_select = true;

            while (!ctrl_in.rst) {
                dgtl.audio = tmp;
                ctrl_out.rdy = true;
                await ClockAsync();
            }
            ctrl_out.rdy = false;
            dgtl.audio = 0;
        }
    }

    public class Tester : SimulationProcess {
        [InputBus] public Control ctrl_in;
        [InputBus] public PMOD pmod_in;
        [InputBus] public Digital dgtl;

        [OutputBus] public Control ctrl_out;
        [OutputBus] public PMOD pmod_out;

        public override async Task Run() {
            Random rnd = new Random();
            int[] values = new int[100];
            for (int i = 0; i < values.Length; i++)
                values[i] = rnd.Next() & 0xFFF; // Only 12 bits are read by the controller

            for (int j = 0; j < values.Length; j++) {
                int val = values[j];
                Console.Write("\r{0}/{1}", j, values.Length);

                while (pmod_in.chip_select) {
                    ctrl_out.read = true;
                    await ClockAsync();
                }
                ctrl_out.read = false;

                for (int i = 0; i < 2; i++) { // only 2 due to clocked synchronization
                    pmod_out.data = false;
                    await ClockAsync();
                }

                for (int i = 0; i < 12; i++) {
                    pmod_out.data = ((val >> i) & 0x1) == 1;
                    await ClockAsync();
                }
                
                while (!ctrl_in.rdy)
                    await ClockAsync();

                System.Diagnostics.Debug.Assert(dgtl.audio == val, string.Format("Expected {0} got {1}", val, dgtl.audio));
                ctrl_out.rst = true;
                await ClockAsync();
                ctrl_out.rst = false;
            }
        }
    }

}
