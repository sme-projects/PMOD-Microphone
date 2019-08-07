using SME;

namespace MicDriver {
    class MainClass {
        public static void Main(string[] args) {
            
            using (new Simulation()) {
                var ctrl = Scope.CreateBus<Control>();
                var pmod = Scope.CreateBus<PMOD>();
                var dgtl = Scope.CreateBus<Digital>();

                var controller = new PMOD_Controller();
                var tester = new Tester();

                controller.ctrl_in = ctrl;
                controller.pmod_in = pmod;
                controller.ctrl_out = ctrl;
                controller.pmod_out = pmod;
                controller.dgtl = dgtl;

                tester.ctrl_in = ctrl;
                tester.pmod_in = pmod;
                tester.ctrl_out = ctrl;
                tester.pmod_out = pmod;
                tester.dgtl = dgtl;

                Simulation.Current.AddTopLevelInputs(ctrl, pmod);
                Simulation.Current.AddTopLevelOutputs(ctrl, pmod, dgtl);

                Simulation.Current
                    .BuildCSVFile()
                    .BuildVHDL()
                    .Run();
            }
            
        }
    }
}
