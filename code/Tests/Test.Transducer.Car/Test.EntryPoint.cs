/*
    Generic Transducer, test

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using Console = System.Console;

    enum CarState : long {
        Off = 0,
        Breaks = 1 << 6, // Drive gear position, engine is off
        DriveBreaks, ParkBreaks, ReverseBreaks, // engine is on
        Drive = 1, Park = 2, Reverse = 3,
        //
        Lights = 1 << 7, // Drive gear position, engine is off
        BreaksLights = Breaks|Lights,
        ParkBreaksLights = Park|Breaks|Lights,
        DriveBreaksLights = Drive|Breaks|Lights,
        ReverseBreaksLights = Reverse|Breaks|Lights,
        ParkLights = Park|Lights, DriveLights = Drive|Lights, ReverseLights = Reverse|Lights,
    }

    enum CarSignal {
        StartEngine, StopEngine, BrakePedalPress, BrakePedalRelease,
        ShiftToDrive, ShiftToReverse, ShiftToPark,
        LightsOn, LightsOff,
    }

    enum CarOutput {
        Undefined, None, LightsIndicatorOn, LightsIndicatorOff,
        GearIndicatorDrive, GearIndicatorReverse, GearIndicatorPark,
    }

    class Test {

        readonly Transducer<CarState, CarSignal, CarOutput> transducer = new();

        void Add(
            CarSignal signal, CarSignal reverseSignal, CarState startState, CarState finishState,
            string transitionText = null, string reverseTransitionText = null,
            CarOutput output = CarOutput.None)
        {
            void AddFunction(CarSignal signal, CarState startState, CarState finishState) {
                transducer.AddStateTransitionFunctionPart(signal, startState,
                    (state, input) => finishState);
                transducer.AddOutputFunctionPart(signal, startState,
                    (state, input) => output);
            } //AddFunction
            void AddText(CarState startState, CarState finishState, string text) {
                if (!string.IsNullOrEmpty(text))
                    transducer.AddValidStateTransition(
                        startState, finishState,
                        (start, finish) => Console.WriteLine($"      ({text}...)")); 
            }
            AddFunction(signal, startState, finishState);
            AddFunction(reverseSignal, finishState, startState);
            AddText(startState, finishState, transitionText);
            AddText(finishState, startState, reverseTransitionText);
            if (signal != CarSignal.LightsOn && signal != CarSignal.LightsOff) {
                AddFunction(signal, startState | CarState.Lights, finishState | CarState.Lights);
                AddFunction(reverseSignal, finishState | CarState.Lights, startState | CarState.Lights);
                AddText(startState | CarState.Lights, finishState | CarState.Lights, transitionText);
                AddText(finishState | CarState.Lights, startState | CarState.Lights, reverseTransitionText);                
            } //if
        } //Add

        void Report(Transducer<CarState, CarSignal, CarOutput>.SignalResult result) {
            if (result.OutputSuccess && result.TransitionResult.Success)
                Console.WriteLine($"State: {result.TransitionResult.State}, Output: {result.Output}");
            else if (!result.TransitionResult.Success && result.OutputSuccess)
                Console.WriteLine($"State: {result.TransitionResult.State},\n{result.TransitionResult.Comment}");
            else if (result.TransitionResult.Success && !result.OutputSuccess)
                Console.WriteLine($"State: {result.TransitionResult.State},\n{result.OutputComment}");
            else
                Console.WriteLine($"State: {result.TransitionResult.State},\nTransition issue: {result.TransitionResult.Comment},\nOutput function issue: {result.OutputComment}");
        } //Report

        void Populate() {
            Add(CarSignal.BrakePedalPress, CarSignal.BrakePedalRelease, CarState.Off, CarState.Breaks,
                "Engaging breaks and allowing the engine to start",
                "Disengaging breaks and blocking the engine from starting"
            );
            Add(CarSignal.StartEngine, CarSignal.StopEngine, CarState.Breaks, CarState.ParkBreaks,
                "Starting Engine",
                "Stopping Engine"
            );
            Add(CarSignal.ShiftToDrive, CarSignal.ShiftToPark, CarState.ParkBreaks, CarState.DriveBreaks,
                "Shifting gearbox to drive",
                "Shifting gearbox to park"
            );
            Add(CarSignal.ShiftToReverse, CarSignal.ShiftToPark, CarState.ParkBreaks, CarState.ReverseBreaks,
                "Shifting gearbox to reverse",
                "Shifting gearbox to park"
            );
            Add(CarSignal.BrakePedalRelease, CarSignal.BrakePedalPress, CarState.DriveBreaks, CarState.Drive,
                "Moving forward",
                "Stopping forward motion"
            );
            Add(CarSignal.BrakePedalRelease, CarSignal.BrakePedalPress, CarState.ReverseBreaks, CarState.Reverse,
                "Moving in reverse",
                "Stopping in reverse motion"
            );
            foreach (CarState state in new CarState[] { CarState.Off, CarState.Breaks, CarState.ParkBreaks, CarState.DriveBreaks, CarState.ReverseBreaks, CarState.Park, CarState.Drive, CarState.Reverse}) {
                Add(CarSignal.LightsOn, CarSignal.LightsOff, state, state | CarState.Lights,
                "Turning lights on", "Turning lights off");
            } //loop
            foreach (CarState state in new CarState[] { CarState.Drive, CarState.Reverse, CarState.ReverseLights, CarState.DriveLights }) {
                transducer.AddInvalidInput(CarSignal.ShiftToDrive, state, (input, state) =>
                "Please don't shifts gears to the opposite direction while driving");
                transducer.AddInvalidInput(CarSignal.ShiftToReverse, state, (input, state) =>
                "Please don't shifts gears to the opposite direction while driving");
            } //loop
        } //Populate

        void Work() {
            Populate();
            Report(transducer.Signal(CarSignal.BrakePedalPress));
            Report(transducer.Signal(CarSignal.BrakePedalRelease));
            Report(transducer.Signal(CarSignal.BrakePedalPress));
            Report(transducer.Signal(CarSignal.StartEngine));
            Report(transducer.Signal(CarSignal.LightsOn));
            Report(transducer.Signal(CarSignal.ShiftToDrive));
            Report(transducer.Signal(CarSignal.BrakePedalRelease));
            Report(transducer.Signal(CarSignal.ShiftToReverse)); // invalid signal
            Report(transducer.Signal(CarSignal.BrakePedalPress));
            Report(transducer.Signal(CarSignal.ShiftToPark));
            Report(transducer.Signal(CarSignal.LightsOff));
            Report(transducer.Signal(CarSignal.ShiftToReverse));
            Report(transducer.Signal(CarSignal.BrakePedalRelease));
            Report(transducer.Signal(CarSignal.BrakePedalPress));
            Report(transducer.Signal(CarSignal.ShiftToPark));
            Report(transducer.Signal(CarSignal.StopEngine));
        } //Work

        static void Main() {
            new Test().Work();
        } //Main

    } //class Test

}
