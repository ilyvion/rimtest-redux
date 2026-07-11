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
    /// Entry point for collection/enumerable based assertions
    /// </summary>
    /// <param name="thing">A collection</param>
    /// <returns>An AssertCollection, specialized in asserting collections</returns>
    public static AssertCollection AssertCollection([AllowNull] IEnumerable thing) => new(thing);

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

    /// <summary>
    /// Base constructor, restricted to the specialized Assertion subtypes defined in this assembly.
    /// </summary>
    /// <remarks>Not meant to be instantiated directly; use <see cref="Assert"/>, <see cref="AssertCollection"/>, or <see cref="AssertFunc(Func{dynamic})"/> instead.</remarks>
    private protected Assertion() { }
}

/// <summary>
/// Specialized Assertion for Values
/// </summary>
/// <remarks>Allows the EqualTo, LessThan, GreaterThan, BetweenInclusive, BetweenExclusive, SameValueAs, SameReferenceAs, Null, True and False checks.</remarks>
public class AssertValue : Assertion
{
    private readonly IComparable? thing;

    /// <summary>
    /// base constructor
    /// </summary>
    /// <param name="thing">The value to check</param>
    internal AssertValue([AllowNull] IComparable thing)
    {
        this.thing = thing;
    }

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
    /// Check: asserted value is EQUAL (via <see cref="object.Equals(object?)"/>) to otherThing
    /// </summary>
    /// <param name="otherThing">check value</param>
    public void SameValueAs(IComparable otherThing)
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
                    Negated
                        ? "RimTestRedux.Assertion.NotSameValueAs"
                        : "RimTestRedux.Assertion.SameValueAs"
                ).Translate(thing!.ToString(), otherThing?.ToString())
            );
        }
    }

    /// <summary>
    /// Check: asserted value is the SAME REFERENCE (via <see cref="object.ReferenceEquals(object?, object?)"/>) as otherThing
    /// </summary>
    /// <param name="otherThing">check value</param>
    public void SameReferenceAs(IComparable otherThing)
    {
        var result = ReferenceEquals(RequireNonNull(), otherThing);
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated
                        ? "RimTestRedux.Assertion.NotSameReferenceAs"
                        : "RimTestRedux.Assertion.SameReferenceAs"
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
/// Specialized Assertion for Collections
/// </summary>
/// <remarks>Allows the Contains, Empty and Count checks.</remarks>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public class AssertCollection : Assertion
#pragma warning restore CA1711
{
    private readonly IEnumerable? thing;

    /// <summary>
    /// base constructor
    /// </summary>
    /// <param name="thing">The collection to check</param>
    internal AssertCollection([AllowNull] IEnumerable thing)
    {
        this.thing = thing;
    }

    /// <summary>
    /// Returns the asserted collection, or throws an AssertionException if it's null.
    /// </summary>
    private IEnumerable RequireNonNull() =>
        thing ?? throw new AssertionException("RimTestRedux.Assertion.NonNullRequired".Translate());

    private static string Describe(IEnumerable collection) =>
        $"[{string.Join(", ", collection.Cast<object>().Select(item => item?.ToString() ?? "null"))}]";

    /// <summary>
    /// Negation grammar link, negates the current assertion.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertCollection Not
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
    public AssertCollection To => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertCollection Is => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertCollection Be => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertCollection Do => this;

    /// <summary>
    /// Grammar link, doesn't have any effect.
    /// </summary>
    /// <remarks>All grammar links can be chained as needed.</remarks>
    public AssertCollection Have => this;

    /// <summary>
    /// Check: asserted collection CONTAINS item
    /// </summary>
    /// <param name="item">the item to look for</param>
    public void Contain(object item)
    {
        var collection = RequireNonNull();
        var result = collection.Cast<object>().Any(element => Equals(element, item));
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated
                        ? "RimTestRedux.Assertion.NotContains"
                        : "RimTestRedux.Assertion.Contains"
                ).Translate(Describe(collection), item?.ToString() ?? "null")
            );
        }
    }

    /// <summary>
    /// Check: asserted collection is EMPTY
    /// </summary>
    public void Empty()
    {
        var collection = RequireNonNull();
        var result = !collection.Cast<object>().Any();
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated ? "RimTestRedux.Assertion.NotEmpty" : "RimTestRedux.Assertion.Empty"
                ).Translate(Describe(collection))
            );
        }
    }

    /// <summary>
    /// Check: asserted collection has exactly expectedCount elements
    /// </summary>
    /// <param name="expectedCount">the expected number of elements</param>
    public void Count(int expectedCount)
    {
        var collection = RequireNonNull();
        var actualCount = collection.Cast<object>().Count();
        var result = actualCount == expectedCount;
        if (Negated)
        {
            result = !result;
        }

        if (!result)
        {
            throw new AssertionException(
                (
                    Negated ? "RimTestRedux.Assertion.NotCount" : "RimTestRedux.Assertion.Count"
                ).Translate(
                    Describe(collection),
                    expectedCount.ToString(CultureInfo.InvariantCulture),
                    actualCount.ToString(CultureInfo.InvariantCulture)
                )
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
    internal AssertFunc(Func<dynamic> thing)
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
    /// Constructor for AssertAction usage
    /// </summary>
#pragma warning disable CS8618
    private protected AssertFunc()
#pragma warning restore CS8618
    { }

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
public class AssertAction : AssertFunc
{
    private readonly Action action;

    /// <summary>
    /// base constructor
    /// </summary>
    /// <param name="thing">The function to check</param>
    internal AssertAction(Action thing)
    {
        action = thing ?? throw new ArgumentNullException(nameof(thing));
    }

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
