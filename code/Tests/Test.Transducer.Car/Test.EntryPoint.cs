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
        Undefined, None, LightsIndicatorOn, LightsIndicatorOff,
        GearIndicatorDrive, GearIndicatorReverse, GearIndicatorPark,
    }

    class Test {

        readonly Transducer<CarState, CarSignal, CarOutput> transducer = new();

        void Add(
            CarSignal signal, CarState startState, CarState finishState,
            CarOutput output = CarOutput.None, string transitionText = null)
        {
            transducer.AddStateTransitionFunctionPart(signal, startState,
                (state, input) => finishState);
            transducer.AddOutputFunctionPart(signal, finishState,
                (state, input) => output);
            if (!string.IsNullOrEmpty(transitionText))
                transducer.AddValidStateTransition(
                    startState, finishState,
                    (start, finish) => Console.WriteLine(transitionText)); 
                
        } //Add

        void Populate() {            
            Add(CarSignal.BrakePedalPress, CarState.Off,
                CarState.Breaks);            
            Add(CarSignal.StartEngine, CarState.Breaks,
                CarState.Idle, CarOutput.None, "Starting Engine...");

            Add(CarSignal.ShiftToDrive, CarState.Idle, CarState.Drive);
            Add(CarSignal.ShiftToReverse, CarState.Idle, CarState.Reverse);
            //
            Add(CarSignal.ShiftToDrive, CarState.IdleLight, CarState.DriveLight,
                CarOutput.GearIndicatorDrive, "Shifting to drive...");
            //Add(CarSignal.ShiftToReverse, CarState.IdleLight, CarState.ReverseLight,
            //    CarOutput.GearIndicatorReverse);


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
                Add(CarSignal.LighsOn, startState,
                    CarState.Light | startState, CarOutput.LightsIndicatorOn);
                Add(CarSignal.LightsOff, startState | CarState.Light,
                    startState, CarOutput.LightsIndicatorOff);
            } //loop
        } //Populate

        void Work() {
            Populate();
            Console.WriteLine(transducer.Signal(CarSignal.BrakePedalPress));
            Console.WriteLine(transducer.Signal(CarSignal.StartEngine));
            Console.WriteLine(transducer.Signal(CarSignal.LighsOn));
            Console.WriteLine(transducer.Signal(CarSignal.ShiftToDrive));
            Console.WriteLine(transducer.Signal(CarSignal.LightsOff));
        } //Work

        static void Main() {
            new Test().Work();
        } //Main

    } //class Test

}
