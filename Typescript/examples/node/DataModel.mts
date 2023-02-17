export class State
{
    FID!: number;
    Program!: string | undefined;
    State_Code: string | undefined;
    State_Name: string | undefined;
    Flowing_St: string | undefined;
    FID_1!: number;
    Data: Array<StateData> | undefined;
}

export class StateData
{
    Year!: number;
    Value!: number;
    Variable!: string;
    DateOfRecording!: Date;
}

// export class Point {

// }