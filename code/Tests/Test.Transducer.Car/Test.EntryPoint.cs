/*
    Generic Transducer, test

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using Console = System.Console;

    enum CarState : byte {
        Off,  Breaks, Idle, 
        Drive, Reverse,
        Light = 0xF0, BreaksLight = Light | Breaks, IdleLight = Light | Idle,
        DriveLight = Light | Drive, ReverseLight = Light | Reverse,
    }

    enum CarSignal {
        StartEngine, StopEngine, BrakePedalPress, BrakePedalRelease,
        ShiftToDrive, ShiftToReverse, ShiftToPark,
        LighsOn, LightsOff,
    }

    enum CarOutput {
        Undefined, Fine, LightsOn, LightsOff,
    }

    class Test {

        static void Add(
            Transducer<CarState, CarSignal, CarOutput> transducer,
            CarSignal signal, CarState startState, CarState finishState, CarOutput output) {
                transducer.AddStateTransitionFunctionPart(signal, startState,
                    (state, input) => finishState);
                transducer.AddOutputFunctionPart(signal, finishState,
                    (state, input) => output);
            } //Add

        static Transducer<CarState, CarSignal, CarOutput> Populate() {
            Transducer<CarState, CarSignal, CarOutput> transducer = new();
            Add(transducer, CarSignal.BrakePedalPress, CarState.Off,
                CarState.Breaks, CarOutput.Fine);            
            Add(transducer, CarSignal.StartEngine, CarState.Breaks,
                CarState.Idle, CarOutput.Fine);
            Add(transducer, CarSignal.ShiftToDrive, CarState.Idle,
                CarState.Drive, CarOutput.Fine);
            Add(transducer, CarSignal.ShiftToReverse, CarState.Idle,
                CarState.Reverse, CarOutput.Fine);
            //
            transducer.AddStateTransitionFunctionPart(CarSignal.ShiftToDrive, CarState.IdleLight,
                (state, input) => CarState.DriveLight);
            transducer.AddStateTransitionFunctionPart(CarSignal.ShiftToReverse, CarState.IdleLight,
                (state, input) => CarState.ReverseLight);
            transducer.AddStateTransitionFunctionPart(CarSignal.ShiftToDrive, CarState.Reverse,
                (state, input) => CarState.Drive);
            transducer.AddStateTransitionFunctionPart(CarSignal.ShiftToReverse, CarState.Drive,
                (state, input) => CarState.Reverse);
            transducer.AddStateTransitionFunctionPart(CarSignal.ShiftToDrive, CarState.ReverseLight,
                (state, input) => CarState.DriveLight);
            transducer.AddStateTransitionFunctionPart(CarSignal.ShiftToReverse, CarState.DriveLight,
                (state, input) => CarState.ReverseLight);
            transducer.AddStateTransitionFunctionPart(CarSignal.ShiftToPark, CarState.Breaks,
                (state, input) => CarState.Idle);
            transducer.AddStateTransitionFunctionPart(CarSignal.ShiftToPark, CarState.BreaksLight,
                (state, input) => CarState.IdleLight);
            transducer.AddStateTransitionFunctionPart(CarSignal.BrakePedalPress, CarState.Drive,
                (state, input) => CarState.Breaks);
            transducer.AddStateTransitionFunctionPart(CarSignal.BrakePedalPress, CarState.Reverse,
                (state, input) => CarState.Breaks);
            transducer.AddStateTransitionFunctionPart(CarSignal.BrakePedalPress, CarState.DriveLight,
                (state, input) => CarState.BreaksLight);
            transducer.AddStateTransitionFunctionPart(CarSignal.BrakePedalPress, CarState.ReverseLight,
                (state, input) => CarState.BreaksLight);
            transducer.AddStateTransitionFunctionPart(CarSignal.StopEngine, CarState.Breaks,
                (state, input) => CarState.Off);
            transducer.AddStateTransitionFunctionPart(CarSignal.StopEngine, CarState.BreaksLight,
                (state, input) => CarState.Light);
            //
            foreach (var startState in new CarState[] { CarState.Off,  CarState.Breaks, CarState.Idle, CarState.Drive, CarState.Reverse }) {
                Add(transducer, CarSignal.LighsOn, startState,
                    CarState.Light | startState, CarOutput.LightsOn);
                Add(transducer, CarSignal.LightsOff, startState | CarState.Light,
                    startState, CarOutput.LightsOff);
            } //loop
            return transducer;
        } //Populate

        static void Main() {
            var transducer = Populate();
            Console.WriteLine(transducer.Signal(CarSignal.BrakePedalPress));
            Console.WriteLine(transducer.Signal(CarSignal.StartEngine));
            Console.WriteLine(transducer.Signal(CarSignal.LighsOn));
            Console.WriteLine(transducer.Signal(CarSignal.ShiftToDrive));
            Console.WriteLine(transducer.Signal(CarSignal.LightsOff));
        } //Main

    } //class Test

}
