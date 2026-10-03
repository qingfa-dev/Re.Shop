namespace BuildingBlock.Kernel.Results;

/// <summary>Represents the absence of a value (a "void" success).</summary>
public readonly struct Unit : IEquatable<Unit>
{
    #region Value

    /// <summary>The singleton unit value.</summary>
    public static Unit Value => default;

    #endregion

    #region Equality

    /// <summary>Always returns <see langword="true"/> because <see cref="Unit"/> has no state.</summary>
    /// <param name="other">The other unit.</param>
    /// <returns><see langword="true"/>.</returns>
    public bool Equals(Unit other) => true;
    /// <summary>Returns <see langword="true"/> when <paramref name="obj"/> is a <see cref="Unit"/>.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when <paramref name="obj"/> is a <see cref="Unit"/>.</returns>
    public override bool Equals(object? obj) => obj is Unit;
    /// <summary>Returns 0, as all <see cref="Unit"/> instances are equal.</summary>
    /// <returns>Zero.</returns>
    public override int GetHashCode() => 0;

    /// <summary>Always returns <see langword="true"/>.</summary>
    public static bool operator ==(Unit left, Unit right) => true;
    /// <summary>Always returns <see langword="false"/>.</summary>
    public static bool operator !=(Unit left, Unit right) => false;

    #endregion

    #region ToString

    /// <summary>Returns the string representation "()".</summary>
    /// <returns>"()".</returns>
    public override string ToString() => "()";

    #endregion
}