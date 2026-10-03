namespace BuildingBlock.Kernel.Results;

/// <summary>Represents the absence of a value (a "void" success).</summary>
public readonly struct Unit : IEquatable<Unit>
{
    #region Value

    public static Unit Value => default;

    #endregion

    #region Equality

    public bool Equals(Unit other) => true;
    public override bool Equals(object? obj) => obj is Unit;
    public override int GetHashCode() => 0;

    public static bool operator ==(Unit left, Unit right) => true;
    public static bool operator !=(Unit left, Unit right) => false;

    #endregion

    #region ToString

    public override string ToString() => "()";

    #endregion
}