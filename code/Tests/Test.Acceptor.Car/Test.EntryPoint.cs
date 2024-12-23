/*
    Generic Acceptor, test

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

    class Test {

        static Acceptor<CarState, CarSignal> Populate() {
            Acceptor<CarState, CarSignal> acceptor = new();
            acceptor.AddStateTransitionFunctionPart(CarSignal.BrakePedalPress, CarState.Off,
                (state, input) => CarState.Breaks);
            acceptor.AddStateTransitionFunctionPart(CarSignal.StartEngine, CarState.Breaks,
                (state, input) => CarState.Idle);
            acceptor.AddStateTransitionFunctionPart(CarSignal.ShiftToDrive, CarState.Idle,
                (state, input) => CarState.Drive);
            acceptor.AddStateTransitionFunctionPart(CarSignal.ShiftToReverse, CarState.Idle,
                (state, input) => CarState.Reverse);
            acceptor.AddStateTransitionFunctionPart(CarSignal.ShiftToDrive, CarState.IdleLight,
                (state, input) => CarState.DriveLight);
            acceptor.AddStateTransitionFunctionPart(CarSignal.ShiftToReverse, CarState.IdleLight,
                (state, input) => CarState.ReverseLight);
            acceptor.AddStateTransitionFunctionPart(CarSignal.ShiftToDrive, CarState.Reverse,
                (state, input) => CarState.Drive);
            acceptor.AddStateTransitionFunctionPart(CarSignal.ShiftToReverse, CarState.Drive,
                (state, input) => CarState.Reverse);
            acceptor.AddStateTransitionFunctionPart(CarSignal.ShiftToDrive, CarState.ReverseLight,
                (state, input) => CarState.DriveLight);
            acceptor.AddStateTransitionFunctionPart(CarSignal.ShiftToReverse, CarState.DriveLight,
                (state, input) => CarState.ReverseLight);
            acceptor.AddStateTransitionFunctionPart(CarSignal.ShiftToPark, CarState.Breaks,
                (state, input) => CarState.Idle);
            acceptor.AddStateTransitionFunctionPart(CarSignal.ShiftToPark, CarState.BreaksLight,
                (state, input) => CarState.IdleLight);
            acceptor.AddStateTransitionFunctionPart(CarSignal.BrakePedalPress, CarState.Drive,
                (state, input) => CarState.Breaks);
            acceptor.AddStateTransitionFunctionPart(CarSignal.BrakePedalPress, CarState.Reverse,
                (state, input) => CarState.Breaks);
            acceptor.AddStateTransitionFunctionPart(CarSignal.BrakePedalPress, CarState.DriveLight,
                (state, input) => CarState.BreaksLight);
            acceptor.AddStateTransitionFunctionPart(CarSignal.BrakePedalPress, CarState.ReverseLight,
                (state, input) => CarState.BreaksLight);
            acceptor.AddStateTransitionFunctionPart(CarSignal.StopEngine, CarState.Breaks,
                (state, input) => CarState.Off);
            acceptor.AddStateTransitionFunctionPart(CarSignal.StopEngine, CarState.BreaksLight,
                (state, input) => CarState.Light);
            foreach (var startState in new CarState[] { CarState.Off,  CarState.Breaks, CarState.Idle, CarState.Drive, CarState.Reverse }) {
                acceptor.AddStateTransitionFunctionPart(CarSignal.LighsOn, startState,
                    (state, input) => CarState.Light | startState);
                acceptor.AddStateTransitionFunctionPart(CarSignal.LightsOff, startState | CarState.Light,
                    (state, input) => startState);
            } //loop
            return acceptor;
        } //Populate

        static void Main() {
            var acceptor = Populate();
            Console.WriteLine(acceptor.TransitionSignal(CarSignal.BrakePedalPress));
            Console.WriteLine(acceptor.TransitionSignal(CarSignal.StartEngine));
            Console.WriteLine(acceptor.TransitionSignal(CarSignal.LighsOn));
            Console.WriteLine(acceptor.TransitionSignal(CarSignal.ShiftToDrive));
            Console.WriteLine(acceptor.TransitionSignal(CarSignal.LightsOff));
        } //Main

    } //class Test

}
