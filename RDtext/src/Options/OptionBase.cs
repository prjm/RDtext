using System;
using System.Collections.Generic;
using RDtext.Attributes;

namespace RDtext.Options {

    /// <summary>
    ///     base class for an option
    /// </summary>
    [Mutable]
    public abstract class OptionBase : IEquatable<OptionBase>, IComparable<OptionBase> {

        /// <summary>
        ///     create a new option
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="parent">parent option</param>
        protected OptionBase(string name, OptionBase? parent) {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(string.Empty, nameof(name));

            Name = name;
            Parent = parent;
        }

        /// <summary>
        ///     option name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     parent option
        /// </summary>
        public OptionBase? Parent { get; }

        /// <summary>
        ///     compare to another option
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool Equals(OptionBase? other);

        /// <summary>
        ///     check if two options have the same name
        /// </summary>
        /// <param name="other"></param>
        /// <returns><c>true</c> if the options have the same name</returns>
        public bool SameName(OptionBase other) {
            if (other is null)
                throw new ArgumentNullException(nameof(other));
            return string.Equals(Name, other.Name, StringComparison.Ordinal);
        }

        /// <summary>
        ///     compare to another object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
            => obj is OptionBase option && Equals(option);

        /// <summary>
        ///     compute a hash code
        /// </summary>
        /// <returns></returns>
        public abstract override int GetHashCode();

        /// <summary>
        ///     compare to another option
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(OptionBase? other) {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            return StringComparer.Ordinal.Compare(Name, other.Name);
        }

        /// <summary>
        ///     compare for equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(OptionBase? left, OptionBase? right) {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }

        /// <summary>
        ///     compare for inequality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(OptionBase? left, OptionBase? right)
            => !(left == right);

        /// <summary>
        ///     compare
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(OptionBase? left, OptionBase? right)
            => left is null ? right is not null : left.CompareTo(right) < 0;

        /// <summary>
        ///     compare
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(OptionBase? left, OptionBase? right)
            => left is null || left.CompareTo(right) <= 0;

        /// <summary>
        ///     compare
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(OptionBase? left, OptionBase? right)
            => left is not null && left.CompareTo(right) > 0;

        /// <summary>
        ///     compare
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(OptionBase left, OptionBase right)
            => left is null ? right is null : left.CompareTo(right) >= 0;
    }

    /// <summary>
    ///     generic base class for an option
    /// </summary>
    /// <typeparam name="TOption"></typeparam>
    public abstract class OptionBase<TOption> : OptionBase {

        /// <summary>
        ///     create a new option base
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="parent">parent option</param>
        /// <param name="defaultValue">default value</param>
        protected OptionBase(string name, OptionBase<TOption>? parent, TOption defaultValue) : base(name, parent) {
            Parent = parent;
            value = defaultValue;
            this.defaultValue = defaultValue;
        }

        /// <summary>
        ///     parent option
        /// </summary>
        public new OptionBase<TOption>? Parent { get; private set; }

        /// <summary>
        ///     option value
        /// </summary>
        private TOption value;

        /// <summary>
        ///     default value
        /// </summary>
        private readonly TOption defaultValue;

        /// <summary>
        ///     <c>true</c> if the option is set
        /// </summary>
        public bool IsSet { get; protected set; }

        /// <summary>
        ///     reset this value
        /// </summary>
        public void Reset()
            => IsSet = false;

        /// <summary>
        ///     option value
        /// </summary>
        public TOption Value {
            get {
                if (IsSet)
                    return value;

                if (Parent != default)
                    return Parent.Value;

                return defaultValue;
            }
            set {
                IsSet = true;
                this.value = value;
            }
        }


        /// <summary>
        ///     compare to another option
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(OptionBase? other) {
            if (other is not OptionBase<TOption> o)
                return false;

            return
                SameName(o) &&
                EqualityComparer<TOption>.Default.Equals(Value, o.Value); ;
        }

        /// <summary>
        ///     compute a hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            var hc = new HashCode();
            hc.Add(Name, StringComparer.Ordinal);
            hc.Add(Value);
            return hc.ToHashCode();
        }
    }
}
