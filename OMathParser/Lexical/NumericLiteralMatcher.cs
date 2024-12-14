using OMathParser.Utils;

namespace OMathParser.Lexical;

public class NumericLiteralMatcher
{
    private enum State : uint
    {
        START,
        INTEGRAL,
        FRACTIONAL_START,
        FRACTIONAL,
        EXPONENT,
        EXPONENT_SIGN,
        EXPONENT_DIGITS,
        FRACTIONAL_ONLY,
        FRACTIONAL_ONLY_LEADING_ZERO,
        FRACTIONAL_ONLY_DIGITS
    }

    private static readonly char[] digits = "0123456789".ToCharArray();
    private static readonly char[] nonZeroDigits = "123456789".ToCharArray();

    private Dictionary<State, Dictionary<char, State>> transitionMaps = [];
    private HashSet<State> acceptableStates;
    private bool eIsIndependent;
    private bool exponentSignIsIndependent;

    private int parsedCount;
    private State currentState;
    private Dictionary<char, State> availableTransitions;

    public NumericLiteralMatcher(ParseProperties properties)
    {
        eIsIndependent = false;
        exponentSignIsIndependent = false;

        Init(properties);
        Reset();

        currentState = State.START;
        availableTransitions = transitionMaps[State.START];
    }

    public bool TryMatch(string s, out string matched) => TryMatch(s, 0, out matched);

    public bool TryMatch(string s, int startIndex, out string matched)
    {
        Reset();
        for (int input = startIndex; input < s.Length; input++)
        {
            char inputChar = s.ElementAt(input);
            if (!availableTransitions.TryGetValue(inputChar, out State newState))
            {
                return NoMoreTransitions(s, startIndex, out matched);
            }
            else
            {
                parsedCount++;
                SwitchState(newState);
            }
        }

        return NoMoreTransitions(s, startIndex, out matched);
    }

    private bool NoMoreTransitions(string s, int startIndex, out string matched)
    {
        if (currentState == State.EXPONENT && eIsIndependent)
        {
            matched = s.Substring(startIndex, parsedCount - 1);
            return true;
        }
        else if (currentState == State.EXPONENT_SIGN && exponentSignIsIndependent)
        {
            matched = s.Substring(startIndex, parsedCount - 2);
            return true;
        }
        else if (acceptableStates.Contains(currentState))
        {
            matched = s.Substring(startIndex, parsedCount);
            return true;
        }
        else
        {
            matched = null;
            return false;
        }
    }

    private void Reset()
    {
        parsedCount = 0;
        SwitchState(State.START);
    }

    private void SwitchState(State newState)
    {
        if (newState != currentState)
        {
            currentState = newState;
            availableTransitions = transitionMaps[currentState];
        }
    }

    private void Init(ParseProperties properties)
    {
        InitAcceptableStates(properties);
        InitTransitionMaps();
    }

    private void InitAcceptableStates(ParseProperties properties)
    {
        acceptableStates =
        [
            State.INTEGRAL,
            State.FRACTIONAL,
            State.EXPONENT_DIGITS,
            State.FRACTIONAL_ONLY,
            State.FRACTIONAL_ONLY_DIGITS,
        ];

        if (properties.IsConstant("e") ||
            properties.IsConstant("E") ||
            properties.IsVariable("e") ||
            properties.IsVariable("E"))
        {
            eIsIndependent = true;
            exponentSignIsIndependent = true;
            acceptableStates.Add(State.EXPONENT);
            acceptableStates.Add(State.EXPONENT_SIGN);
        }

        if (properties.IsFunctionName("e") ||
                 properties.IsFunctionName("E"))
        {
            eIsIndependent = true;
            exponentSignIsIndependent = false;
            acceptableStates.Remove(State.EXPONENT_SIGN);
            acceptableStates.Add(State.EXPONENT);
        }
    }

    private void InitTransitionMaps()
    {
        this.transitionMaps = [];

        //
        // START transitions
        //
        Dictionary<char, State> START_transitions = [];
        foreach (char d in nonZeroDigits)
        {
            START_transitions.Add(d, State.INTEGRAL);
        }
        START_transitions.Add('0', State.FRACTIONAL_ONLY);
        START_transitions.Add('.', State.FRACTIONAL_ONLY_LEADING_ZERO);
        transitionMaps.Add(State.START, START_transitions);

        //
        // INTEGRAL transitions
        //
        Dictionary<char, State> INTEGRAL_transitions = [];
        foreach (char d in digits)
        {
            INTEGRAL_transitions.Add(d, State.INTEGRAL);
        }
        INTEGRAL_transitions.Add('.', State.FRACTIONAL_START);
        INTEGRAL_transitions.Add('e', State.EXPONENT);
        INTEGRAL_transitions.Add('E', State.EXPONENT);
        transitionMaps.Add(State.INTEGRAL, INTEGRAL_transitions);

        //
        // FRACTIONAL_START transitions
        //
        Dictionary<char, State> FRACTIONAL_START_transitions = [];
        foreach (char d in digits)
        {
            FRACTIONAL_START_transitions.Add(d, State.FRACTIONAL);
        }
        transitionMaps.Add(State.FRACTIONAL_START, FRACTIONAL_START_transitions);

        //
        // FRACTIONAL transitions
        //
        Dictionary<char, State> FRACTIONAL_transitions = [];
        foreach (char d in digits)
        {
            FRACTIONAL_transitions.Add(d, State.FRACTIONAL);
        }
        FRACTIONAL_transitions.Add('e', State.EXPONENT);
        FRACTIONAL_transitions.Add('E', State.EXPONENT);
        transitionMaps.Add(State.FRACTIONAL, FRACTIONAL_transitions);

        //
        // FRACTIONAL_ONLY transitions
        //
        Dictionary<char, State> FRACTIONAL_ONLY_transitions = new()
        {
            { '.', State.FRACTIONAL_ONLY_LEADING_ZERO }
        };
        transitionMaps.Add(State.FRACTIONAL_ONLY, FRACTIONAL_ONLY_transitions);

        //
        // FRACTIONAL_ONLY_LEADING_ZERO transitions
        //
        Dictionary<char, State> FOLZ_transitions = [];
        foreach (char d in nonZeroDigits)
        {
            FOLZ_transitions.Add(d, State.FRACTIONAL_ONLY_DIGITS);
        }
        FOLZ_transitions.Add('0', State.FRACTIONAL_ONLY_LEADING_ZERO);
        transitionMaps.Add(State.FRACTIONAL_ONLY_LEADING_ZERO, FOLZ_transitions);

        //
        // FRACTIONAL_ONLY_DIGITS transitions
        //
        Dictionary<char, State> FOD_transitions = [];
        foreach (char d in digits)
        {
            FOD_transitions.Add(d, State.FRACTIONAL_ONLY_DIGITS);
        }
        FOD_transitions.Add('e', State.EXPONENT);
        FOD_transitions.Add('E', State.EXPONENT);
        transitionMaps.Add(State.FRACTIONAL_ONLY_DIGITS, FOD_transitions);

        //
        // EXPONENT transitions
        //
        Dictionary<char, State> EXPONENT_transitions = [];
        foreach (char d in nonZeroDigits)
        {
            EXPONENT_transitions.Add(d, State.EXPONENT_DIGITS);
        }
        EXPONENT_transitions.Add('+', State.EXPONENT_SIGN);
        EXPONENT_transitions.Add('-', State.EXPONENT_SIGN);
        transitionMaps.Add(State.EXPONENT, EXPONENT_transitions);

        //
        // EXPONENT_SIGN transitions
        //
        Dictionary<char, State> EXPONENT_SIGN_transitions = [];
        foreach (char d in nonZeroDigits)
        {
            EXPONENT_SIGN_transitions.Add(d, State.EXPONENT_DIGITS);
        }
        transitionMaps.Add(State.EXPONENT_SIGN, EXPONENT_SIGN_transitions);

        //
        // EXPONENT_DIGITS transitions
        //
        Dictionary<char, State> EXPONENT_DIGITS_transitions = [];
        foreach (char d in digits)
        {
            EXPONENT_DIGITS_transitions.Add(d, State.EXPONENT_DIGITS);
        }
        transitionMaps.Add(State.EXPONENT_DIGITS, EXPONENT_DIGITS_transitions);
    }
}
