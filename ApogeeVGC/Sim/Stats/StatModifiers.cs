//namespace ApogeeVGC.Sim.Stats;

//public class StatModifiers
//{
//    public int Atk
//    {
//        get;
//        set
//        {
//            if (!IsValidStage(value))
//            {
//                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
//            }

//            field = value;
//        }
//    } = 0;

//    public int Def
//    {
//        get;
//        set
//        {
//            if (!IsValidStage(value))
//            {
//                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
//            }

//            field = value;
//        }
//    } = 0;

//    public int SpA
//    {
//        get;
//        set
//        {
//            if (!IsValidStage(value))
//            {
//                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
//            }

//            field = value;
//        }
//    } = 0;

//    public int SpD
//    {
//        get;
//        set
//        {
//            if (!IsValidStage(value))
//            {
//                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
//            }

//            field = value;
//        }
//    } = 0;

//    public int Spe
//    {
//        get;
//        set
//        {
//            if (!IsValidStage(value))
//            {
//                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
//            }

//            field = value;
//        }
//    } = 0;

//    public int Accuracy
//    {
//        get;
//        set
//        {
//            if (!IsValidStage(value))
//            {
//                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
//            }

//            field = value;
//        }
//    } = 0;

//    public int Evasion
//    {
//        get;
//        set
//        {
//            if (!IsValidStage(value))
//            {
//                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
//            }

//            field = value;
//        }
//    } = 0;

//    public double AtkMultiplier => CalculateRegularStatMultiplier(Atk);
//    public double DefMultiplier => CalculateRegularStatMultiplier(Def);
//    public double SpAMultiplier => CalculateRegularStatMultiplier(SpA);
//    public double SpDMultiplier => CalculateRegularStatMultiplier(SpD);
//    public double SpeMultiplier => CalculateRegularStatMultiplier(Spe);
//    public double AccuracyMultiplier => CalculateAccuracyStatMultiplier(Accuracy);
//    public double EvasionMultiplier => CalculateEvasionStatMultiplier(Evasion);

//    public StatModifiers Copy()
//    {
//        return new StatModifiers
//        {
//            Atk = Atk,
//            Def = Def,
//            SpA = SpA,
//            SpD = SpD,
//            Spe = Spe,
//            Accuracy = Accuracy,
//            Evasion = Evasion,
//        };
//    }

//    private static bool IsValidStage(int stage) => stage is >= -6 and <= 6;

//    private static double CalculateRegularStatMultiplier(int stage)
//    {
//        if (!IsValidStage(stage))
//        {
//            throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.");
//        }

//        return stage switch
//        {
//            -6 => 2.0 / 8.0,
//            -5 => 2.0 / 7.0,
//            -4 => 2.0 / 6.0,
//            -3 => 2.0 / 5.0,
//            -2 => 2.0 / 4.0,
//            -1 => 2.0 / 3.0,
//            0 => 2.0 / 2.0,
//            1 => 3.0 / 2.0,
//            2 => 4.0 / 2.0,
//            3 => 5.0 / 2.0,
//            4 => 6.0 / 2.0,
//            5 => 7.0 / 2.0,
//            6 => 8.0 / 2.0,
//            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.")
//        };
//    }

//    private static double CalculateAccuracyStatMultiplier(int stage)
//    {
//        if (!IsValidStage(stage))
//        {
//            throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.");
//        }

//        return stage switch
//        {
//            -6 => 3.0 / 9.0,
//            -5 => 3.0 / 8.0,
//            -4 => 3.0 / 7.0,
//            -3 => 3.0 / 6.0,
//            -2 => 3.0 / 5.0,
//            -1 => 3.0 / 4.0,
//            0 => 3.0 / 3.0,
//            1 => 4.0 / 3.0,
//            2 => 5.0 / 3.0,
//            3 => 6.0 / 3.0,
//            4 => 7.0 / 3.0,
//            5 => 8.0 / 3.0,
//            6 => 9.0 / 3.0,
//            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.")
//        };
//    }

//    private static double CalculateEvasionStatMultiplier(int stage)
//    {
//        if (!IsValidStage(stage))
//        {
//            throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.");
//        }

//        return stage switch
//        {
//            -6 => 9.0 / 3.0,
//            -5 => 8.0 / 3.0,
//            -4 => 7.0 / 3.0,
//            -3 => 6.0 / 3.0,
//            -2 => 5.0 / 3.0,
//            -1 => 4.0 / 3.0,
//            0 => 3.0 / 3.0,
//            1 => 3.0 / 4.0,
//            2 => 3.0 / 5.0,
//            3 => 3.0 / 6.0,
//            4 => 3.0 / 7.0,
//            5 => 3.0 / 8.0,
//            6 => 3.0 / 9.0,
//            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.")
//        };
//    }
//}