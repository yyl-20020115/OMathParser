using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Utils;

namespace OMathParser.Lexical
{
    public class NumericLiteralMatcher
    {
        private enum State
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

        private static char[] digits = "0123456789".ToCharArray();
        private static char[] nonZeroDigits = "123456789".ToCharArray();

        private Dictionary<State, Dictionary<char, State>> transitionMaps;
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

            init(properties);
            reset();

            currentState = State.START;
            availableTransitions = transitionMaps[State.START];
        }

        public bool tryMatch(String s, out String matched)
        {
            return tryMatch(s, 0, out matched);
        }

        public bool tryMatch(String s, int startIndex, out String matched)
        {
            reset();
            for (int input = startIndex; input < s.Length; input++)
            {
                char inputChar = s.ElementAt(input);
                State newState;
                if (!availableTransitions.TryGetValue(inputChar, out newState))
                {
                    return noMoreTransitions(s, startIndex, out matched);
                }
                else
                {
                    parsedCount++;
                    switchState(newState);
                }
            }

            return noMoreTransitions(s, startIndex, out matched);
        }

        private bool noMoreTransitions(String s, int startIndex, out String matched)
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

        private void reset()
        {
            parsedCount = 0;
            switchState(State.START);
        }

        private void switchState(State newState)
        {
            if (newState != currentState)
            {
                currentState = newState;
                availableTransitions = transitionMaps[currentState];
            }
        }

        private void init(ParseProperties properties)
        {
            initAcceptableStates(properties);
            initTransitionMaps();           
        }

        private void initAcceptableStates(ParseProperties properties)
        {
            acceptableStates = new HashSet<State>();
            acceptableStates.Add(State.INTEGRAL);
            acceptableStates.Add(State.FRACTIONAL);
            acceptableStates.Add(State.EXPONENT_DIGITS);
            acceptableStates.Add(State.FRACTIONAL_ONLY);
            acceptableStates.Add(State.FRACTIONAL_ONLY_DIGITS);

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

        private void initTransitionMaps()
        {
            this.transitionMaps = new Dictionary<State, Dictionary<char, State>>();

            //
            // START transitions
            //
            Dictionary<char, State> START_transitions = new Dictionary<char, State>();
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
            Dictionary<char, State> INTEGRAL_transitions = new Dictionary<char, State>();
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
            Dictionary<char, State> FRACTIONAL_START_transitions = new Dictionary<char, State>();
            foreach (char d in digits)
            {
                FRACTIONAL_START_transitions.Add(d, State.FRACTIONAL);
            }
            transitionMaps.Add(State.FRACTIONAL_START, FRACTIONAL_START_transitions);

            //
            // FRACTIONAL transitions
            //
            Dictionary<char, State> FRACTIONAL_transitions = new Dictionary<char, State>();
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
            Dictionary<char, State> FRACTIONAL_ONLY_transitions = new Dictionary<char, State>();
            FRACTIONAL_ONLY_transitions.Add('.', State.FRACTIONAL_ONLY_LEADING_ZERO);
            transitionMaps.Add(State.FRACTIONAL_ONLY, FRACTIONAL_ONLY_transitions);

            //
            // FRACTIONAL_ONLY_LEADING_ZERO transitions
            //
            Dictionary<char, State> FOLZ_transitions = new Dictionary<char, State>();
            foreach (char d in nonZeroDigits)
            {
                FOLZ_transitions.Add(d, State.FRACTIONAL_ONLY_DIGITS);
            }
            FOLZ_transitions.Add('0', State.FRACTIONAL_ONLY_LEADING_ZERO);
            transitionMaps.Add(State.FRACTIONAL_ONLY_LEADING_ZERO, FOLZ_transitions);

            //
            // FRACTIONAL_ONLY_DIGITS transitions
            //
            Dictionary<char, State> FOD_transitions = new Dictionary<char, State>();
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
            Dictionary<char, State> EXPONENT_transitions = new Dictionary<char, State>();
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
            Dictionary<char, State> EXPONENT_SIGN_transitions = new Dictionary<char, State>();
            foreach (char d in nonZeroDigits)
            {
                EXPONENT_SIGN_transitions.Add(d, State.EXPONENT_DIGITS);
            }
            transitionMaps.Add(State.EXPONENT_SIGN, EXPONENT_SIGN_transitions);

            //
            // EXPONENT_DIGITS transitions
            //
            Dictionary<char, State> EXPONENT_DIGITS_transitions = new Dictionary<char, State>();
            foreach (char d in digits)
            {
                EXPONENT_DIGITS_transitions.Add(d, State.EXPONENT_DIGITS);
            }
            transitionMaps.Add(State.EXPONENT_DIGITS, EXPONENT_DIGITS_transitions);
        }
    }
}
