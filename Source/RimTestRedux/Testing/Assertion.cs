using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RimTestRedux;

/// <summary>
/// Used when the condition expected from an assertion is not reached.
/// </summary>
/// <remarks>Throwing one indicates a test failure.</remarks>
[Serializable]
public class AssertionException : Exception
{
    /// <summary>
    /// constructor directly based on the base Exception
    /// </summary>
    public AssertionException() { }

    /// <summary>
    /// constructor directly based on the base Exception
    /// </summary>
    public AssertionException(string message)
        : base(message) { }

    /// <summary>
    /// constructor directly based on the base Exception
    /// </summary>
    public AssertionException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// constructor directly based on the base Exception
    /// </summary>
    protected AssertionException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}

/// <summary>
/// Generic assertion grammar object.
/// </summary>
/// <remarks>
/// Usage: Assert(value)[.To|.Do|.Be|.Is|.Not].Comparison([Parameters...])
/// </remarks>
public class Assertion
{
    /// <summary>
    /// Entry point for value based assertions
    /// </summary>
    /// <param name="thing">A comparable value</param>
    /// <returns>An AssertValue, specialized in asserting values</returns>
    public static AssertValue Assert([AllowNull] IComparable thing) => new(thing);

    /// <summary>
    /// Entry point for Function based assertions
    /// </summary>
    /// <param name="thing">A function</param>
    /// <returns>An AssertFunc, specialized in asserting Functions</returns>
    public static AssertFunc AssertFunc(Func<dynamic> thing) =>
        thing == null ? throw new ArgumentNullException(nameof(thing)) : new AssertFunc(thing);

    /// <summary>
    /// Entry point for Action based assertions
    /// </summary>
    /// <param name="thing">An Action</param>
    /// <returns>An AssertAction, specialized in asserting Actions</returns>
    public static AssertAction AssertFunc(Action thing) =>
        thing == null ? throw new ArgumentNullException(nameof(thing)) : new AssertAction(thing);

    /// <summary>
    /// Negation flag, can be double negated with multiple uses of the .Not grammar, used to negate a check expectation. (i.e. Assert(1).Not.To.Be.EqualTo(2))
    /// </summary>
    protected bool Negated { get; set; }
}

/// <summary>
/// Specialized Assertion for Values
/// </summary>
/// <remarks>Allows the EqualTo, LessThan, GreaterThan, BetweenInclusive, BetweenExclusive, TheSame, Null, True and False checks.</remarks>
/// <remarks>
/// base constructor
/// </remarks>
/// <param name="thing">The value to check</param>
public class AssertValue([AllowNull] IComparable thing) : Assertion()
{
    private readonly IComparable? thing = thing;

    /// <summary>
    /// Returns the asserted value, or throws an AssertionException if it's null.
    /// </summary>
    /// <remarks>Used by every check other than Null(), which are meaningless for a null value.</remarks>
    private IComparable RequireNonNull() =>
        thing ?? throw new AssertionException("RimTestRedux.Assertion.NonNullRequired".Translate());

    /// <summary>
    /// Negation grammar link, negates the current assertion.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertValue Not
    {
        get
        {
            Negated = !Negated;
            return this;
        }
    }

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertValue To => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertValue Is => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertValue Be => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertValue Do => this;

    /// <summary>
    /// Check: asserted value EQUALS TO otherThing
    /// </summary>
    /// <param name="otherThing">check value</param>
    public void EqualTo(IComparable otherThing)
    {
        var result = RequireNonNull().CompareTo(otherThing) == 0;
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated ? "RimTestRedux.Assertion.NotEqualTo" : "RimTestRedux.Assertion.EqualTo"
                ).Translate(thing!.ToString(), otherThing?.ToString())
            );
        }
    }

    /// <summary>
    /// Check: asserted value LESS THAN otherThing
    /// </summary>
    /// <param name="otherThing">check value</param>
    public void LessThan(IComparable otherThing)
    {
        var result = RequireNonNull().CompareTo(otherThing) < 0;
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated
                        ? "RimTestRedux.Assertion.NotLessThan"
                        : "RimTestRedux.Assertion.LessThan"
                ).Translate(thing!.ToString(), otherThing?.ToString())
            );
        }
    }

    /// <summary>
    /// Check: asserted value GREATER THAN otherThing
    /// </summary>
    /// <param name="otherThing">check value</param>
    public void GreaterThan(IComparable otherThing)
    {
        var result = RequireNonNull().CompareTo(otherThing) > 0;
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated
                        ? "RimTestRedux.Assertion.NotGreaterThan"
                        : "RimTestRedux.Assertion.GreaterThan"
                ).Translate(thing!.ToString(), otherThing?.ToString())
            );
        }
    }

    /// <summary>
    /// Check: minThing LESS THAN or EQUAL TO assertd value AND asserted value LESS THAN or EQUAL TO maxThing
    /// </summary>
    /// <param name="minThing">min check value</param>
    /// <param name="maxThing">max check value</param>
    public void BetweenInclusive(IComparable minThing, IComparable maxThing)
    {
        var value = RequireNonNull();
        var result = value.CompareTo(minThing) >= 0 && value.CompareTo(maxThing) <= 0;
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated
                        ? "RimTestRedux.Assertion.NotBetweenInclusive"
                        : "RimTestRedux.Assertion.BetweenInclusive"
                ).Translate(thing!.ToString(), minThing?.ToString(), maxThing?.ToString())
            );
        }
    }

    /// <summary>
    /// Check: minThing LESS THAN assertd value AND asserted value LESS THAN maxThing
    /// </summary>
    /// <param name="minThing">min check value</param>
    /// <param name="maxThing">max check value</param>
    public void BetweenExclusive(IComparable minThing, IComparable maxThing)
    {
        var value = RequireNonNull();
        var result = value.CompareTo(minThing) > 0 && value.CompareTo(maxThing) < 0;
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated
                        ? "RimTestRedux.Assertion.NotBetweenExclusive"
                        : "RimTestRedux.Assertion.BetweenExclusive"
                ).Translate(thing!.ToString(), minThing?.ToString(), maxThing?.ToString())
            );
        }
    }

    /// <summary>
    /// Check: asserted value SAME AS otherThing
    /// </summary>
    /// <param name="otherThing">check value</param>
    public void TheSame(IComparable otherThing)
    {
        var result = RequireNonNull().Equals(otherThing);
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated ? "RimTestRedux.Assertion.NotTheSame" : "RimTestRedux.Assertion.TheSame"
                ).Translate(thing!.ToString(), otherThing?.ToString())
            );
        }
    }

    /// <summary>
    /// Check: thing IS null
    /// </summary>
    public void Null()
    {
        var result = thing == null;
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated ? "RimTestRedux.Assertion.NotNull" : "RimTestRedux.Assertion.Null"
                ).Translate(thing?.ToString() ?? "null")
            );
        }
    }

    /// <summary>
    /// Check: thing IS true
    /// </summary>
    public void True()
    {
        var result = RequireNonNull().Equals(true);
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated ? "RimTestRedux.Assertion.NotTrue" : "RimTestRedux.Assertion.True"
                ).Translate(thing!.ToString())
            );
        }
    }

    /// <summary>
    /// Check: thing IS false
    /// </summary>
    public void False()
    {
        var result = RequireNonNull().Equals(false);
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated ? "RimTestRedux.Assertion.NotFalse" : "RimTestRedux.Assertion.False"
                ).Translate(thing!.ToString())
            );
        }
    }
}

/// <summary>
/// Specialized Assertion for Functions
/// </summary>
/// <remarks>Allows the Throw check.</remarks>
public class AssertFunc : Assertion
{
    private readonly Func<dynamic> func;

    /// <summary>
    /// base constructor
    /// </summary>
    /// <param name="thing">The function to check</param>
    public AssertFunc(Func<dynamic> thing)
        : base()
    {
        func = thing ?? throw new ArgumentNullException(nameof(thing));
    }

    /// <summary>
    /// Negation grammar link, negates the current assertion.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertFunc Not
    {
        get
        {
            Negated = !Negated;
            return this;
        }
    }

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertFunc To => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertFunc Is => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertFunc Be => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertFunc Do => this;

    /// <summary>
    /// Internal constructor for AssertAction usage
    /// </summary>
    /// <remarks>Do NOT use manually</remarks>
#pragma warning disable CS8618
    protected AssertFunc()
#pragma warning restore CS8618
        : base() { }

    /// <summary>
    /// Check: executing asserted callable throws an error.
    /// </summary>
    public void Throw()
    {
        try
        {
            func();
        }
        catch (Exception temp)
        {
            if (Negated)
            {
                throw new AssertionException("RimTestRedux.Assertion.ThrowNot".Translate(), temp);
            }
            else
            {
                return;
            }
        }
        if (Negated)
        {
            return;
        }

        throw new AssertionException("RimTestRedux.Assertion.ThrowExpected".Translate());
    }
}

/// <summary>
/// Specialized Assertion for Actions
/// </summary>
/// <remarks>Allows the Throw check.</remarks>
/// <remarks>
/// base constructor
/// </remarks>
/// <param name="thing">The function to check</param>
public class AssertAction(Action thing) : AssertFunc()
{
    private readonly Action action = thing ?? throw new ArgumentNullException(nameof(thing));

    /// <summary>
    /// Negation grammar link, negates the current assertion.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public new AssertAction Not
    {
        get
        {
            Negated = !Negated;
            return this;
        }
    }

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public new AssertAction To => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public new AssertAction Is => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public new AssertAction Be => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public new AssertAction Do => this;

    /// <summary>
    /// Check: executing asserted callable throws an error.
    /// </summary>
    public new void Throw()
    {
        try
        {
            action();
        }
        catch (Exception temp)
        {
            if (Negated)
            {
                throw new AssertionException("RimTestRedux.Assertion.ThrowNot".Translate(), temp);
            }
            else
            {
                return;
            }
        }
        if (Negated)
        {
            return;
        }

        throw new AssertionException("RimTestRedux.Assertion.ThrowExpected".Translate());
    }
}
